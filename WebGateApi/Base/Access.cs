
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebGateApi.Base
{
    // DAL провайдер представляет абстракцию связанную с типом Т на базе которого строятся интерфейсы доступа
    // В частности База, таблица 
    #region DalProv
    public class DalProv<TSource> where TSource : class
    {
        protected TSource _source;
        public TSource Source { get { return this._source; } set { _source = value; } }

        public DalProv() : this(null) { }
        public DalProv(TSource source)
        {
            _source = source;
        }
    }
    #endregion

    // DalProvAccess
    // Реализация IDalAcess c удержанием ссылки на произвольную абстракцию
    #region DalProvAccess

    // IDalAccess<TEntity> Это чистый интерфейс контроллера ничего не знающего о DB контексте 
    public class DalProvAccess<TEntity, TSource> : DalProv<TSource>, IDalAccess<TEntity> where TEntity : class where TSource : class
    {
        public IDalReader<TEntity> Reader => GetAccessor<IDalReader<TEntity>>(ReaderFunc);
        public IDalInserter<TEntity> Inserter => GetAccessor<IDalInserter<TEntity>>(InserterFunc);
        public IDalUpdater<TEntity> Updater => GetAccessor<IDalUpdater<TEntity>>(UpdaterFunc);
        public IDalRemover<TEntity> Remover => GetAccessor<IDalRemover<TEntity>>(RemoverFunc);

        public Func<TSource, IDalReader<TEntity>> ReaderFunc { get; set; }  //where TContext : DbContext;
        public Func<TSource, IDalInserter<TEntity>> InserterFunc { get; set; }
        public Func<TSource, IDalUpdater<TEntity>> UpdaterFunc { get; set; }
        public Func<TSource, IDalRemover<TEntity>> RemoverFunc { get; set; }

        private T GetAccessor<T>(Func<TSource, T> accfunc) where T : class
        {
            T ret = null;
            if (Source == null)
            {
                throw new ArgumentException();
            }

            if (accfunc != null)
            {
                ret = accfunc(Source);
            }
            return ret;
        }
    }
    #endregion DalAccess

    //Хелперы 
    //Базовый
    #region DalHelperBase<TEntity> 
    /// <summary>
    /// IDalReader хелпер
    /// </summary>
    public class DalHelperBase<TEntity, TSource> : IDisposable where TSource : class
    {
        public TSource Source { get; set; }

        public DalHelperBase(TSource source)
        {
            Source = source;
        }

        public virtual void Dispose()
        {
            Source = null;
        }

        // --------------------------------------------------------------------------------------------
        // TOOLS
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
    #endregion

    // IDalReader хелпер через функции доступа
    #region DalHelperFuncReader<TEntity> 
    /// <summary>
    /// Реализует IDalReader хелпер через функции доступа
    /// </summary>
    public class DalHelperFuncReader<TEntity, TSource> : DalHelperBase<TEntity, TSource>, IDalReader<TEntity> where TSource : class
    {
        public DalHelperFuncReader(TSource source, Func<TSource
            , IEnumerable<TEntity>> getAllFunc
            , Func<TSource, object, TEntity> findFunc
            , Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> getFunc = null
            ) : base(source)
        {
            GetAllFunc = getAllFunc;
            FindFunc = findFunc;
            GetFunc = getFunc ?? ((a, b) => new List<TEntity>());
        }

        public TEntity Find(object key) => FindFunc(Source, key);
        public IEnumerable<TEntity> GetAll() => GetAllFunc(Source);
        public IEnumerable<TEntity> Get(IEnumerable<KeyValuePair<string, object>> pars) => GetFunc(Source, pars);

        public Func<TSource, IEnumerable<TEntity>> GetAllFunc { get; set; }
        public Func<TSource, object, TEntity> FindFunc { get; set; }
        public Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> GetFunc { get; set; }


        public override void Dispose()
        {
            base.Dispose();
            GetAllFunc = null;
            FindFunc = null;
            GetFunc = null;
        }

    }
    #endregion

    //IDalReader<TEntity>  через  DbSet<TEntity>
    #region  DalHelperDbSetReader<TEntity>
    /// <summary>
    /// обертка  IDalReader<TEntity> на DbSet<TEntity>
    /// </summary>
    public class DalHelperDbSetReader<TEntity> : DalHelperBase<TEntity, DbSet<TEntity>>, IDalReader<TEntity> where TEntity : class
    {
        private DbSetHelper<TEntity> DbSetHelper;

        public DalHelperDbSetReader(DbSet<TEntity> source) : base(source)
        {
            DbSetHelper = new DbSetHelper<TEntity>(source);
        }

        public TEntity Find(object key)
        {
            var t = getKeyType();
            return Source.Find(new object[] { t == null ? key : Convert.ChangeType(key, t) });
        }

        /// <summary>
        ///  Базовая реализация равенство 
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> Get(IEnumerable<KeyValuePair<string, object>> pars)
        {
            return DbSetHelper.SelectAsEqual(pars); //ToDo
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Source;
        }
    }

    #endregion

    //IDalInserter через функции доступа
    #region DalHelperFuncInserter<TEntity> 
    /// <summary>
    /// Реализует IDalInserter через функции доступа
    /// 091018  record template functions
    /// </summary>
    public class DalHelperFuncInserter<TEntity, TSource> : DalHelperBase<TEntity, TSource>, IDalInserter<TEntity>  where TSource : class  //where TEntity : class
    { 
        public TEntity Add(TEntity item) => AddFunc(Source, item);
        public TEntity GetTemplate() =>  TemplateFunc(Source);

        //public Action<TSource, TEntity> AddFunc { get; set; }
        public Func<TSource, TEntity, TEntity> AddFunc { get; set; }
        public Func<TSource, TEntity>   TemplateFunc { get; set; }

        //public DalHelperFuncInserter(TSource source, Action<TSource, TEntity> addFunc, TEntity recTemplate) : this(source, addFunc, (TSource s) => recTemplate )
        public DalHelperFuncInserter(TSource source, Func<TSource, TEntity, TEntity> addFunc, TEntity recTemplate) : this(source, addFunc, (TSource s) => recTemplate)
        {
            AddFunc = addFunc;
        }

        //public DalHelperFuncInserter(TSource source, Action<TSource, TEntity> addFunc, Func<TSource, TEntity> templateFunc ) : base(source)
        public DalHelperFuncInserter(TSource source, Func<TSource, TEntity, TEntity> addFunc, Func<TSource, TEntity> templateFunc) : base(source)
        {
            AddFunc = addFunc;
            TemplateFunc = templateFunc;
        }

        public override void Dispose()
        {
            base.Dispose();
            AddFunc = null;
        }
    }
    #endregion

    //IDalInserter<TEntity> через  DbSet<TEntity>
    #region  DalHelperDbSetInserter<TEntity>
    /// <summary>
    /// обертка  IDalInserter<TEntity> на DbSet<TEntity>
    /// </summary>
    public class DalHelperDbSetInserter<TEntity> : DalHelperBase<TEntity, DbSet<TEntity>>, IDalInserter<TEntity> where TEntity : class
    {
        public DalHelperDbSetInserter(DbSet<TEntity> source) : base(source) {}

        // ТОДО валидаторы, хуидаторы....
        public TEntity Add(TEntity item) => Source.Add(item).Entity;
        public TEntity GetTemplate() => (TEntity)Activator.CreateInstance(typeof(TEntity));   //У бля

    }




    #endregion

    //IDalUpdater через функции доступа
    #region DalHelperFuncUpdater<TEntity>
    /// <summary>
    /// Реализует IDalUpdater через функции доступа
    /// </summary>
    public class DalHelperFuncUpdater<TEntity, TSource> : DalHelperBase<TEntity, TSource>, IDalUpdater<TEntity> where TSource : class
    {
        public void Update(TEntity item) => UpdateFunc(Source, item);
        public Action<TSource, TEntity> UpdateFunc { get; set; }

        public DalHelperFuncUpdater(TSource source, Action<TSource, TEntity> updateFunc) : base(source)
        {
            UpdateFunc = updateFunc;
        }
        public override void Dispose()
        {
            base.Dispose();
            UpdateFunc = null;
        }
    }
    #endregion

    //IDalUpdater<TEntity> через  DbSet<TEntity>
    #region  DalHelperDbSetUpdater<TEntity>
    /// <summary>
    /// обертка  IDalUpdater<TEntity> на DbSet<TEntity>
    /// </summary>
    public class DalHelperDbSetUpdater<TEntity> : DalHelperBase<TEntity, DbSet<TEntity>>, IDalUpdater<TEntity> where TEntity : class
    {
        public DalHelperDbSetUpdater(DbSet<TEntity> source) : base(source)
        { }

        // ТОДО валидаторы, хуидаторы....
        public void Update(TEntity item) => Source.Update(item);
    }
    #endregion

    //IDalRemover через функции доступа
    #region DalHelperFuncRemover<TEntity> 
    /// <summary>
    /// Реализует IDalRemover через функции доступа
    /// </summary>
    public class DalHelperFuncRemover<TEntity, TSource> : DalHelperBase<TEntity, TSource>, IDalRemover<TEntity> where TSource : class
    {
        public TEntity Remove(object key) => RemoveFunc(Source, key);
        public Func<TSource, object, TEntity> RemoveFunc { get; set; }

        public DalHelperFuncRemover(TSource source, Func<TSource, object, TEntity> removeFunc) : base(source)
        {
            RemoveFunc = removeFunc;
        }
        public override void Dispose()
        {
            base.Dispose();
            RemoveFunc = null;
        }

    }
    #endregion

    //IDalRemover<TEntity> через  DbSet<TEntity>
    #region  DalHelperDbSetRemover<TEntity>
    /// <summary>
    /// обертка  IDalUpdater<TEntity> на DbSet<TEntity>
    /// </summary>
    public class DalHelperDbSetRemover<TEntity> : DalHelperBase<TEntity, DbSet<TEntity>>, IDalRemover<TEntity> where TEntity : class
    {
        public DalHelperDbSetRemover(DbSet<TEntity> source) : base(source)
        { }
        // ТОДО валидаторы, хуидаторы....
        public TEntity Remove(object key)
        {
            TEntity ret = null;
            using (var v = new DalHelperDbSetReader<TEntity>(Source))
            {
                ret = v.Find(key);
                if (ret != null)
                {
                    Source.Remove(ret);
                }
            }
            return ret;
        }
    }
    #endregion

    // Реализация  IDalAccessBuilder<TEntity, TContext> АКЦЕСС БИЛДЕР
    #region DalProvAccessBuilder<TEntity, TSource>
    public class DalProvAccessBuilder<TEntity, TSource> : IDalProvAccessBuilder<TEntity, TSource> where TEntity : class where TSource : class
    {

        private DalProvAccess<TEntity, TSource> _dalAccessor = new DalProvAccess<TEntity, TSource>();

        public DalProvAccessBuilder()
        {
        }

        public IDalAccess<TEntity> Build()
        {
            return _dalAccessor;
        }

        public IDalProvAccessBuilder<TEntity, TSource> SetSource(TSource source)
        {
            _dalAccessor.Source = source;
            return this;
        }

        #region ReadAccess
        public IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IDalReader<TEntity>> funcReader)
        {
            _dalAccessor.ReaderFunc = funcReader;
            return this;
        }

        public IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcGetAll, Func<TSource, object, TEntity> funcFind)
        {
            _dalAccessor.ReaderFunc = (src => new DalHelperFuncReader<TEntity, TSource>(src, funcGetAll, funcFind));
            return this;
        }

        public IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind, Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet)
        {
            _dalAccessor.ReaderFunc = (src => new DalHelperFuncReader<TEntity, TSource>(src, funcReadAll, funcFind, funcGet));
            return this;
        }

        public IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind, Func<IEnumerable<TEntity>, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet)
        {
            Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> raFunc = (src, pars) => funcGet( funcReadAll(src), pars );
            this.SetReadAccess(funcReadAll, funcFind, raFunc);
            return this;
        }

        #endregion

        #region InsertAccess
        public IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, IDalInserter<TEntity>> inserter)
        {
            _dalAccessor.InserterFunc = inserter;
            return this;
        }

        // 101018 TEntity recTemplate 231018 Add with retval
        //public IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Action<TSource, TEntity> funcAdd, TEntity recTemplate )
        public IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, TEntity, TEntity> funcAdd, TEntity recTemplate)
        {
            _dalAccessor.InserterFunc = (src => new DalHelperFuncInserter<TEntity, TSource>(src, funcAdd, recTemplate));
            return this;
        }

        // 101018  Func<TSource, TEntity> templateFunc
        //public IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Action<TSource, TEntity> funcAdd, Func<TSource, TEntity> templateFunc )
        public IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, TEntity, TEntity> funcAdd, Func<TSource, TEntity> templateFunc)
        {
            _dalAccessor.InserterFunc = (src => new DalHelperFuncInserter<TEntity, TSource>(src, funcAdd, templateFunc));
            return this;
        }

        #endregion

        #region UpdateAccess
        public IDalProvAccessBuilder<TEntity, TSource> SetUpdateAccess(Func<TSource, IDalUpdater<TEntity>> updater)
        {
            _dalAccessor.UpdaterFunc = updater;
            return this;
        }

        public IDalProvAccessBuilder<TEntity, TSource> SetUpdateAccess(Action<TSource, TEntity> funcUpdate)
        {
            _dalAccessor.UpdaterFunc = (src => new DalHelperFuncUpdater<TEntity, TSource>(src, funcUpdate));
            return this;
        }
        #endregion

        #region RemoveAccess
        public IDalProvAccessBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, IDalRemover<TEntity>> remover)
        {
            _dalAccessor.RemoverFunc = remover;
            return this;
        }

        public IDalProvAccessBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, object, TEntity> funcRemove)
        {
            _dalAccessor.RemoverFunc = (src => new DalHelperFuncRemover<TEntity, TSource>(src, funcRemove));
            return this;
        }

        #endregion
    }

    #endregion

    // Реализация  IDalProvAccessBuilderFactory 
    #region DalProvAccessBuilderFactory 
    // Это чистый интерфейс контроллера ничего не знающего о DB контексте 
    public class DalProvAccessBuilderFactory : IDalProvAccessBuilderFactory
    {
        public IDalProvAccessBuilder<TEntity, TSource> Create<TEntity, TSource>() where TEntity : class where TSource : class
        {
            return new DalProvAccessBuilder<TEntity, TSource>();
        }
    }
    #endregion

    // Реализация  IDalProvAccessBuilderFactory 
    #region DalProvAccessBuilderFactory 
    ////
    //public class DalProvAccessBuilderFactory_BUILD<TEntity, TSource> : Factory<IDalProvAccessBuilder<TEntity, TSource>> where TEntity : class where TSource : class
    //{
    //    public DalProvAccessBuilderFactory_BUILD() : base(() => new DalProvAccessBuilder<TEntity, TSource>()) { }
    //}

    #endregion
}
