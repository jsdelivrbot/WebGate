using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebGateApi.Base
{
    //Sundbox
    // Основа типа делегат 
    public delegate IEnumerable<TEntity> SelectFunc<TEntity>(IEnumerable<TEntity> data, IEnumerable<KeyValuePair<string, object>> pars) ;

    public static class ParSelMath
    {
        public static SelectFunc<TEntity> And<TEntity>(SelectFunc<TEntity> f1, SelectFunc<TEntity> f2)
        {
            SelectFunc<TEntity> ret = (a, b) => f2(f1(a, b), b);
            return ret;
        }
    }

    // Неформализированный прототип обработчика выборки 
    #region IntervalParameters
    /// <summary>
    /// Обработчик выборки по диапазону дат
    /// </summary>
    public class IntervalParameters
    {
        public const string FromStr = @"From";
        public const string ToStr = @"To";

        public DateTime? From { get; set; }
        public DateTime? To { get; set; }


        public IntervalParameters()
        {
            From = null;
            To = null;
        }
        public IntervalParameters(IEnumerable<KeyValuePair<string, object>> parsSet) : this()
        {

            foreach (var kvp in parsSet)
            {
                if (kvp.Key == FromStr)
                {
                    From = DateTime.Parse(kvp.Value.ToString());
                }
                if (kvp.Key == ToStr)
                {
                    To = DateTime.Parse(kvp.Value.ToString());
                }
            }
        }


        /// <summary>
        /// Эта статическая шняга должна вернуть функцию отбора значений по интервалу
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="fromFunc"></param>
        /// <param name="toFunc"></param>
        /// <returns></returns>
        public static SelectFunc<TEntity>
            GetSelectFunction<TEntity>(Func<TEntity, DateTime?> rowDateFunc)
            where TEntity : class
        {
            /// 1 только from все с даты фром и до конца  
            /// 2 только to 
            /// 

            Func<IEnumerable<TEntity>, DateTime?, IEnumerable<TEntity>> fFunc = ((a, b) => a.Where(row => rowDateFunc(row) >= b).Select(x => x));
            Func<IEnumerable<TEntity>, DateTime?, IEnumerable<TEntity>> tFunc = ((a, e) => a.Where(row => rowDateFunc(row) < e).Select(x => x));
            Func<IEnumerable<TEntity>, DateTime?, DateTime?, IEnumerable<TEntity>> ftFunc = ((a, b, e) => a.Where(row => rowDateFunc(row) >= b && rowDateFunc(row) < e).Select(x => x));

            return (a, b) =>
            {
                var ip = new IntervalParameters(b);
                if (ip.From != null)
                {
                    return (ip.To != null) ? ftFunc(a, ip.From, ip.To) : fFunc(a, ip.From);
                }
                else if (ip.To != null)
                {
                    return tFunc(a, ip.To);
                }
                return new List<TEntity>();
            };

        }
    }
    #endregion 

    #region EntityParameters
    /// <summary>
    /// Обработчик выборки по значениям полей
    /// </summary>
    public class EntityParHelper
    {

        public static SelectFunc<TEntity> GetSelectFunction<TEntity>()
        {
            SelectFunc<TEntity> ret = (a, b) => a.Where<TEntity>(x => EntityParHelper.CheckRowValues<TEntity>(x,b) ).Select(x => x);
            return ret;
        }

        /// <summary>
        /// Конвертирует внутренний тип EntityParameters-а : SelectFunc<TEntity> в тип интерфейса Dal-ридера    
        /// </summary>
        /// <typeparam name="TSource"> исходная SelectFunc<TEntity> функция </typeparam>
        /// <typeparam name="TEntity"> функция селекта  IEnumerable<TEntity>> из источника данных </typeparam>
        /// <param name="func"></param>
        /// <param name="selfromSourceFunc"></param>
        /// <returns></returns>
        public static Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> ToBaseTypeFromSource<TSource, TEntity>(SelectFunc<TEntity> func, Func<TSource, IEnumerable<TEntity>> selfromSourceFunc )
        {
            return (src,pars) => func(selfromSourceFunc(src), pars);
        }

        /// <summary>
        /// Конвертирует внутренний тип EntityParameters-а : SelectFunc<TEntity> в тип интерфейса Dal-ридера    
        /// </summary>
        /// <typeparam name="TSource"> исходная SelectFunc<TEntity> функция </typeparam>
        /// <typeparam name="TEntity"> функция селекта  IEnumerable<TEntity>> из источника данных </typeparam>
        /// <param name="func"></param>
        /// <param name="selfromSourceFunc"></param>
        /// <returns></returns>
        public static Func<IEnumerable<TEntity>, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> ToBaseType<TEntity>(SelectFunc<TEntity> selFunc)
        {
            return (src, pars) => selFunc(src, pars);
        }

        private static bool CheckRowValues<TEntity>(TEntity item, IEnumerable<KeyValuePair<string, object>> fieldsValues)
        {
            bool ret = true;
            foreach (var x in fieldsValues)
            {
                var p = typeof(TEntity).GetProperty(x.Key);
                if (p != null)
                {
                    var vl = p.GetValue(item);
                    if (vl.ToString() != x.Value.ToString())
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;

        }
    }
    #endregion
    
    /// Помошник DbSet-а
    #region DbSetHelper
    public class DbSetHelper<TEntity> where TEntity : class
    {
        public DbSet<TEntity> Source { get; set; }


        public DbSetHelper(DbSet<TEntity> source)
        {
            Source = source;
        }

        #region SelectAsEqual селектор
        /// <summary>
        /// Выбирает значения удовлетворяющие набору имя поля - значение 
        /// </summary>
        /// <param name="fieldsValues"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> SelectAsEqual(IEnumerable<KeyValuePair<string, object>> fieldsValues)
        {
            return Source.Where<TEntity>(BuildWhereFunc(fieldsValues)).Select(x => x);
            //return Source;
        }

        protected Func<TEntity, bool> BuildWhereFunc(IEnumerable<KeyValuePair<string, object>> fieldsValues)
        {
            Func<TEntity, bool> ret = (x => this.CheckRowValues(x, fieldsValues));
            return ret;
        }

        /// <summary>
        /// Check item on equals values
        /// </summary>
        protected bool CheckRowValues(TEntity item, IEnumerable<KeyValuePair<string, object>> fieldsValues)
        {
            bool ret = true;
            foreach (var x in fieldsValues)
            {
                var p = typeof(TEntity).GetProperty(x.Key);
                if (p != null)
                {
                    var vl = p.GetValue(item);
                    if (vl.ToString() != x.Value.ToString())
                    {
                        ret = false;
                        break;
                    }
                }
            }
            return ret;
        }

        #endregion

    }
    #endregion

}
