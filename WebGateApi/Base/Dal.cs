using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebGateApi.Base
{
    // Комплексный DAL-интерфейс билдерится в логике доступ к интерфейсу в контроллере 
    public interface IDal<TEntity> where TEntity : class
    {
        IDalAccess<TEntity> Access { get; }
        IModelEntityConverter<TEntity> Convert { get; }
        IMetadatas Metadatas { get; }
    }

    // Абстрактный интерфейс Dal (для наследования)
    public interface IDalBuilder<TEntity, TSource, TBuilder> : IDalAccessBuilder<TEntity, TSource, TBuilder>, IModelEntityConverterBuilder<TEntity, TBuilder> where TEntity : class { }

    //Прикладной интерфейс Dal
    public interface IDalBuilder<TEntity, TSource> : IDalBuilder<TEntity, TSource, IDalBuilder<TEntity, TSource>>, IBuilder<IDal<TEntity>> where TEntity : class  { }

    // Для фабрики билдера универсальный интерфейс

    // Жесткая фабрика с жестким интерфейсом
    public interface IDalBuilderFactory 
    {
        IDalBuilder<TEntity, TSource> Create<TEntity, TSource>() where TEntity : class where TSource : class;
    }
    public class DalBuilderFactory : IDalBuilderFactory
    {
        public IDalBuilder<TEntity, TSource> Create<TEntity, TSource>() where TEntity : class where TSource : class
        {
            return new DalBuilder<TEntity, TSource>();
        }
    }


    // Фабрика на базе UniFactory не прижилась...
    //public class DalBuilderFactory<TEntity, TSource> : UniFactory<IDalBuilder<TEntity, TSource>> where TEntity : class where TSource : class
    //{
    //    public DalBuilderFactory() : base(() => new DalBuilder<TEntity, TSource>())  { }
    //}

    #region DalSimple  
    public class DalSimple<TEntity,TSource> : IDal<TEntity> , IDisposable where TEntity : class where TSource : class
    {
        private IDalAccess<TEntity> _accesser = null;
        private IModelEntityConverter<TEntity> _converter  = null;
        private IMetadatas _metadatas = null;

        public IDalAccess<TEntity> Access => _accesser;
        public IModelEntityConverter<TEntity> Convert => _converter;
        public IMetadatas Metadatas => _metadatas;

        /// <summary>
        /// инициализирует акцессор и конвертер
        /// </summary>
        public DalSimple() : this(getDefaultAccesser(), getDefaultConverter()) { }
        /// <summary>
        ///  принимает акцессор инициализирует конвертер
        /// </summary>
        public DalSimple(IDalAccess<TEntity> accesser):this(accesser , getDefaultConverter()) { }

        /// <summary>
        /// принимает конвертер инициализирует акцессор
        /// </summary>
        public DalSimple(IModelEntityConverter<TEntity> converter) : this(getDefaultAccesser(), converter) { }


        public DalSimple(IDalAccess<TEntity> accesser, IModelEntityConverter<TEntity> converter) : this(accesser, converter, getDefaultMetadatas<TEntity>()) { }


        public DalSimple(IDalAccess<TEntity> accesser, IModelEntityConverter<TEntity> converter, IMetadatas metadatas)
        {
            _accesser = accesser;
            _converter = converter;
            _metadatas = metadatas;
        }

        public void Dispose()
        {
            Action<object> dis = o => { if (o is IDisposable a) { a.Dispose(); } } ;

            if (_accesser  != null) { dis(_accesser);  _accesser  = null;}
            if (_converter != null) { dis(_converter); _converter = null;}
            if (_metadatas != null) { dis(_metadatas); _metadatas = null;}
        }

        public static IDalAccess<TEntity> getDefaultAccesser()  => new  DalProvAccess<TEntity,TSource>();
        public static IModelEntityConverter<TEntity> getDefaultConverter() => new ModelEntityConverter<TEntity>();
        public static IMetadatas getDefaultMetadatas<T>() => new EntytyMetadatasHelper().GetMetadataByAttr(typeof(T) );          /// Скрытая связанность !!!!
    }
    #endregion

    #region DalBuilder
    public class DalBuilder<TEntity, TSource> : IDalBuilder<TEntity, TSource> where TEntity : class where TSource : class
    {
        public static IDalProvAccessBuilder<TEntity, TSource> getDefaultAccessBuilder() => new DalProvAccessBuilder<TEntity, TSource>();
        public static IModelEntityConverterBuilder<TEntity> getDefaultConvertBuilder() => new ModelEntityConverterBuilder<TEntity>();

        private IDalProvAccessBuilder<TEntity, TSource> accBuilder = getDefaultAccessBuilder();
        private IModelEntityConverterBuilder<TEntity> cnvBuilder = getDefaultConvertBuilder();


        public IDalBuilder<TEntity, TSource> SetSource(TSource source)
        {
            accBuilder.SetSource(source); return this;
        }

        #region read
        public IDalBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IDalReader<TEntity>> reader)
        {
            accBuilder.SetReadAccess(reader); return this;
        }

        public IDalBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind)
        {
            accBuilder.SetReadAccess(funcReadAll, funcFind); return this;
        }

        public IDalBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind, Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet)
        {
            accBuilder.SetReadAccess(funcReadAll, funcFind, funcGet); return this;
        }

        public IDalBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind, Func<IEnumerable<TEntity>, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet)
        {
            this.SetReadAccess(funcReadAll, funcFind, (src,p)=> funcGet(funcReadAll( src) , p  ));  return this;
        }

        #endregion

        #region update delete
        public IDalBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, IDalInserter<TEntity>> inserter)
        {
            accBuilder.SetInsertAccess(inserter); return this;
        }

        public IDalBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, TEntity, TEntity> funcAdd, TEntity template)
        {
            accBuilder.SetInsertAccess(funcAdd, template); return this;
        }


        public IDalBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, TEntity, TEntity> funcAdd, Func<TSource, TEntity> templateFunc)
        {
             accBuilder.SetInsertAccess(funcAdd , templateFunc); return this;
        }


        public IDalBuilder<TEntity, TSource> SetUpdateAccess(Func<TSource, IDalUpdater<TEntity>> updater)
        {
            accBuilder.SetUpdateAccess(updater); return this;
        }

        public IDalBuilder<TEntity, TSource> SetUpdateAccess(Action<TSource, TEntity> funcUpdate)
        {
            accBuilder.SetUpdateAccess(funcUpdate); return this;
        }

        public IDalBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, IDalRemover<TEntity>> updater)
        {
            accBuilder.SetRemoveAccess(updater); return this;
        }

        public IDalBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, object, TEntity> funcUpdate)
        {
            accBuilder.SetRemoveAccess(funcUpdate); return this;
        }
        #endregion

        #region conversion
        public IDalBuilder<TEntity, TSource> AddFrom<T>(IModelEntityFromConverter<T, TEntity> converter)
        {
            cnvBuilder.AddFrom<T>(converter); return this;
        }

        public IDalBuilder<TEntity, TSource> AddTo<T>(IModelEntityToConverter<T, TEntity> converter)
        {
            cnvBuilder.AddTo<T>(converter); return this;
        }

        public IDalBuilder<TEntity, TSource> AddListFrom<T>(IModelEntityListFromConverter<T, TEntity> converter)
        {
            cnvBuilder.AddListFrom<T>(converter); return this;
        }

        public IDalBuilder<TEntity, TSource> AddListTo<T>(IModelEntityListToConverter<T, TEntity> converter)
        {
            cnvBuilder.AddListTo<T>(converter); return this;
        }

        public IDalBuilder<TEntity, TSource> AddFromFunc<T>(Func<T, TEntity> convFunc)
        {
            cnvBuilder.AddFromFunc<T>(convFunc); return this;
        }

        public IDalBuilder<TEntity, TSource> AddToFunc<T>(Func<TEntity, T> convFunc) where T : class
        {
            cnvBuilder.AddToFunc<T>(convFunc); return this;
        }

        public IDalBuilder<TEntity, TSource> AddListFromFunc<T>(Func<T, IEnumerable<TEntity>> convFunc)
        {
            cnvBuilder.AddListFromFunc<T>(convFunc); return this;
        }

        public IDalBuilder<TEntity, TSource> AddListToFunc<T>(Func<IEnumerable<TEntity>, T> convFunc) where T : class
        {
            cnvBuilder.AddListToFunc<T>(convFunc); return this;
        }
        #endregion 

        public IDal<TEntity> Build()
        {
            return new DalSimple<TEntity, TSource>(accBuilder.Build(), cnvBuilder.Build());
        }

    }
    #endregion

}
