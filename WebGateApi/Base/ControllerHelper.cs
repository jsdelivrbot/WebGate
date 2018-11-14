
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace WebGateApi.Base
{
    /// В связи с .... функционал этого статик хелпера перетекает в инжектируемый класс DataContextHelper  
    /// 
    /// Помошник КОНТРОЛЛЕРА
    ///  
    ///
    #region ControllerHelper
    public class ControllerHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> ToKeyValuePair<TEntity>(TEntity source) where TEntity : class
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            TEntity df = (TEntity)Activator.CreateInstance(typeof(TEntity));

            if (df != null)
            {
                foreach (var p in typeof(TEntity).GetProperties())
                {
                    var pInf = typeof(TEntity).GetProperty(p.Name);

                    if (pInf.GetValue(df) != pInf.GetValue(source))
                    {
                        ret.Add(p.Name, pInf.GetValue(source));
                    }
                }
            }
            return ret;
        }
        public static IEnumerable<KeyValuePair<string, object>> AddKeyValuePair(IEnumerable<KeyValuePair<string, object>> source, KeyValuePair<string, object> val )
        {
            var l = new List<KeyValuePair<string, object>>();
            foreach (var a in source)
            {
                l.Add(a);
            }
            l.Add(val);
            return l ;
        }

        /// <summary>
        ///  ВЫДЕЛЯЕТ ПАРАМЕТРЫ ПО МАСКЕ ТИПА
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> ToKeyValuePair<TEntity>(HttpRequest source) where TEntity : class
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();

            foreach (var p in typeof(TEntity).GetProperties())
            {
                var pInf = typeof(TEntity).GetProperty(p.Name);
                if (source.Query.ContainsKey(p.Name))
                {
                    ret.Add(p.Name, source.Query[p.Name]);
                }
            }

            return ret;
        }

        /// <summary>
        ///  ВЫДЕЛЯЕТ ПАРАМЕТРЫ ПО МАСКЕ ТИПА
        ///  300518
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TEntity ToEntyty<TEntity>(HttpRequest source) where TEntity : class
        {
            TEntity ret = (TEntity)Activator.CreateInstance(typeof(TEntity));

            foreach (var p in typeof(TEntity).GetProperties())
            {
                var pInf = typeof(TEntity).GetProperty(p.Name);
                if (source.Query.ContainsKey(p.Name) 
                    || source.Query.ContainsKey("\""+p.Name+"\"") ) 
                {
                    var val = source.Query.ContainsKey(p.Name) ? source.Query[p.Name] : source.Query["\"" + p.Name + "\""];
                    ret.GetType().InvokeMember(p.Name,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty,
                        Type.DefaultBinder, ret, val);
                }
            }

            return ret;
        }


        /// <summary>
        ///  ВЫДЕЛЯЕТ ПАРАМЕТРЫ ПО МАСКЕ ТИПА
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<string, object>> ToKeyValuePair<TEntity>(TEntity source, HttpRequest request) where TEntity : class
        {

            Dictionary<string, object> ret = new Dictionary<string, object>();

            foreach (var p in typeof(TEntity).GetProperties())
            {
                var pInf = typeof(TEntity).GetProperty(p.Name);

                if (request.Query.ContainsKey(p.Name))
                {
                    ret.Add(p.Name, pInf.GetValue(source));
                }
            }

            return ret;
        }


    }
    #endregion

}
