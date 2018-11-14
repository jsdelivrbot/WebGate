using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebGate.Models.Sandbox
{
    #region Dal
    public class Dal
    {

    }
    #endregion

    // DBSET<TEntity>  Реализация IDalReader<TEntity> Editor, Remover, Updater  
    #region DalReader<TEntity>
    public class DalBase<TEntity> : Dal where TEntity : class
    {
        public DbSet<TEntity> Source { get; }

        public DalBase(DbSet<TEntity> source)
        {
            Source = source;
        }

        // типы ключей
        protected IEnumerable<Type> getKeyTypes()
        {
            return typeof(TEntity).GetMembers()
                .Where(x => x.MemberType == MemberTypes.Property && x.GetCustomAttribute(typeof(KeyAttribute)) != null)
                .Select(x => ((PropertyInfo)x).PropertyType).ToList();
        }

        /// Тип первого ключа 
        protected Type getKeyType()
        {
            var e = getKeyTypes();
            return e.Count() > 0 ? e.First() : null;
        }
    }

    // DAL Reader
    public class DalReader<TEntity> : DalBase<TEntity>, IDalReader<TEntity> where TEntity : class
    {
        public DalReader(DbSet<TEntity> source) : base(source)
        {
        }

        public TEntity Find(object key)
        {
            var t = getKeyType();
            return Source.Find(new object[] { t == null ? key : Convert.ChangeType(key, getKeyType()) });
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Source;
        }
    }
      

    public class Dal<TEntity> : DalReader<TEntity>, IDal<TEntity> where TEntity : class
    {
        public Dal(DbSet<TEntity> source) : base(source)
        {
        }

        public void Add(TEntity item)
        {
            Source.Add(item);
        }

        public TEntity Remove(object key)
        {
            TEntity ret = Find(key);
            if (ret != null)
            {
                Source.Remove(Find(key));
            }
            return ret;
        }

        public void Update(TEntity item)
        {
            Source.Update(item);
        }
    }
    #endregion DalReader<TEntity>

    // Func Реализация IDalReader<TEntity>
    #region Func  IDalReader<TEntity>
    //public class DalReaderFunc<TContext,TEntity> : DalBase<TEntity>, IDalReader<TEntity> where TEntity : class 
    //{
    //    private Func<TContext, IEnumerable<TEntity>> _funcReadAll;
    //    private Func<TContext, object, TEntity> _funcFind;

    //    public DalReaderFunc(Func<TContext, IEnumerable<TEntity>> funcReadAll, Func<TContext, object, TEntity> funcFind)
    //    {
    //        _funcReadAll = funcReadAll;
    //        _funcFind = funcFind;
    //    }

    //    public TEntity Find(object key)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerable<TEntity> GetAll()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}        
    #endregion

    // TODO 
    // Реализация IDalAcess c удержанием ссылки на базу 
    #region DalAccess
    public class DalAccessBase : Dal
    {

    }

    // IDalAccess<TEntity> Это чистый интерфейс контроллера ничего не знающего о DB контексте 
    public class DalAccess<TEntity, TContext> : DalAccessBase, IDalAccess<TEntity> where TEntity : class  where TContext : DbContext
    {

        public IDalReader<TEntity>   Reader    => GetAccessor<IDalReader<TEntity>>(ReaderFumc);
        public IDalInserter<TEntity> Inserter  => GetAccessor<IDalInserter<TEntity>>(InserterFumc);
        public IDalUpdater<TEntity>  Updater   => GetAccessor<IDalUpdater<TEntity>>(UpdaterFumc);
        public IDalRemover<TEntity>  Remover   => GetAccessor<IDalRemover<TEntity>>(RemoverFumc);

        public Func<TContext,  IDalReader<TEntity>>     ReaderFumc     { get; set; }  //where TContext : DbContext;
        public Func<TContext, IDalInserter<TEntity>>   InserterFumc   { get; set; }
        public Func<TContext, IDalUpdater<TEntity>>    UpdaterFumc    { get; set; }
        public Func<TContext, IDalRemover<TEntity>>    RemoverFumc    { get; set; }

        public TContext  Context { get; set; }

        private T GetAccessor<T>(Func<TContext,T> accfunc) where T: class
        {
            T ret = null;
            if (Context == null)
            {
                throw new ArgumentException();
            }

            if (accfunc != null)
            {
                ret = accfunc(Context);
            }
            return ret;
        }
    }
    #endregion DalAccess

    // Реализация  IDalAccessBuilder<TEntity, TContext> АКЦЕСС БИЛДЕР
    #region DalAccessBuilder<TEntity, TContext>
    // Это чистый интерфейс контроллера ничего не знающего о DB контексте 
    public class DalAccessBuilder<TEntity, TContext> : IDalAccessBuilder<TEntity, TContext> where TEntity : class where TContext : DbContext
    {
        private DalAccess<TEntity, TContext> _dalAccessor = new DalAccess<TEntity, TContext>();
        /// <summary>
        /// Set DBContext 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public IDalAccessBuilder<TEntity, TContext> SetDbContext(TContext context)
        {
            _dalAccessor.Context = context;
            return this;
        }

        public IDalAccess<TEntity> Build()
        {
            return _dalAccessor;
        }

        #region ReadAccess 
        /// <summary>
        /// СКРЫТАЯ ЗАВИСИМОСТЬ !!!
        /// </summary>
        /// <param name="dbset"></param>
        /// <returns></returns>
        public IDalAccessBuilder<TEntity, TContext> SetReadAccess(DbSet<TEntity> dbset)
        {
            _dalAccessor.ReaderFumc = ( ctx => new DalReader<TEntity>(dbset) );   // СКРЫТАЯ ЗАВ"""
            return this;
        }

        public IDalAccessBuilder<TEntity, TContext> SetReadAccess(Func<TContext, IDalReader<TEntity>> funcReader)
        {
            _dalAccessor.ReaderFumc = funcReader;
            return this;
        }

        public IDalAccessBuilder<TEntity, TContext> SetReadAccess(Func<TContext, IEnumerable<TEntity>> funcReadAll, Func<TContext, object, TEntity> funcFind)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    #endregion 

    // Реализация  IDalAccessBuilderFactory 
    #region IDalAccessBuilderFactory 
    // Это чистый интерфейс контроллера ничего не знающего о DB контексте 
    public class DalAccessBuilderFactory : IDalAccessBuilderFactory
    {
        public IDalAccessBuilder<TEntity, TContext> Create<TEntity, TContext>() where TEntity : class  where TContext : DbContext
        {
            return new DalAccessBuilder<TEntity, TContext>(); 
        }
    }

    #endregion

   
    #region IDalDbReader<TEntity>
    public class DalDbReader<TEntity> : Dal, IDalDbReader<TEntity> where TEntity : class
    {
        public DalDbReader( ):base()
        {
        }

        public Func<DbContext, object, TEntity> FuncFind { get; set;}
        public Func<DbContext, IEnumerable<TEntity>> FuncGetAll { get; set; }

        
        public TEntity Find(DbContext context, object key)
        {
            if (FuncFind == null)
            {
                throw new NotImplementedException();
            }
            return FuncFind(context, key);
        }

        public IEnumerable<TEntity> GetAll(DbContext context)
        {
            if (FuncFind == null)
            {
                throw new NotImplementedException();
            }
            return FuncGetAll(context);
        }

        public IDalReader<TEntity> Reader(DbContext context)
        {
            throw new NotImplementedException();
        }
    }
    #endregion IDalDbReader<TEntity>
    
    #region DalReader TODO
    //public class DalReader : IDalReader  //where TEntity : class
    //{
    //    public DbContext Context { get; }

    //    public DalReader( DbContext context) 
    //    {
    //        Context = context;
    //    }

    //    public TEntity Find<TEntity>(object key)
    //    {
    //        //return _source.Find(new object[] { key });
    //    }

    //    public IEnumerable<TEntity> GetAll<TEntity>()
    //    {
    //        //return _source;
    //    }
    //}
    #endregion DalReader<TEntity>

    #region IModelsManager
    public interface IModelsManager
    {
                
    }    
    #endregion
}
