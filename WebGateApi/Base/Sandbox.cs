using System;
using System.Collections.Generic;
using System.Text;

namespace WebGateApi.Base.Sandbox
{
    #region  IModelEntityHelper<TEntity> 
    /// <summary>
    ///  Хелпер TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IModelEntityHelper<TEntity>
    {
        IModelEntityConverter<TEntity> Converters { get; }
    }
    #endregion

    #region IModelEntityHelperBuilder<TEntity> 
    // абстрактный интерфейс стороны логики для формирования ModelEntity - хелпера
    // Интерфейс создания IModelEntityHelper
    public interface IModelEntityHelperBuilder<TEntity, TBuilder>
    {
        // блять типом идентифицировать или имя добавить ?
        TBuilder AddFromConverter<T>(IModelEntityFromConverter<T, TEntity> converter);
        TBuilder AddFromConverterFunc<T>(Func<T, TEntity> convFunc);

        TBuilder AddToConverter<T>(IModelEntityToConverter<T, TEntity> converter);
        TBuilder AddToConverterFunc<T>(Func<TEntity, T> convFunc) where T : class;

        TBuilder AddListFromConverter<T>(IModelEntityListFromConverter<T, TEntity> converter);
        TBuilder AddListFromConverterFunc<T>(Func<T, IEnumerable<TEntity>> convFunc);

        TBuilder AddListToConverter<T>(IModelEntityListToConverter<T, TEntity> converter);
        TBuilder AddListToConverterFunc<T>(Func<IEnumerable<TEntity>, T> converter) where T : class;
    }
    #endregion

    #region IModelEntityHelperBuilder<TEntity> 
    // прикладной интерфейс стороны логики для формирования ModelEntity - хелпера
    // Интерфейс создания IModelEntityHelper
    public interface IModelEntityHelperBuilder<TEntity> : IModelEntityHelperBuilder<TEntity, IModelEntityHelperBuilder<TEntity>>, IBuilder<IModelEntityHelper<TEntity>>
    {
        // блять типом идентифицировать или имя добавить ?
        //IModelEntityHelperBuilder<TEntity> AddFromConverter<T>(IModelEntityFromConverter<T, TEntity> converter);
        //IModelEntityHelperBuilder<TEntity> AddFromConverterFunc<T>(Func<T, TEntity> convFunc);

        //IModelEntityHelperBuilder<TEntity> AddToConverter<T>(IModelEntityToConverter<T, TEntity> converter);
        //IModelEntityHelperBuilder<TEntity> AddToConverterFunc<T>(Func<TEntity, T> convFunc) where T : class;

        //IModelEntityHelperBuilder<TEntity> AddListFromConverter<T>(IModelEntityListFromConverter<T, TEntity> converter);
        //IModelEntityHelperBuilder<TEntity> AddListFromConverterFunc<T>(Func<T, IEnumerable<TEntity>> convFunc);

        //IModelEntityHelperBuilder<TEntity> AddListToConverter<T>(IModelEntityListToConverter<T, TEntity> converter);
        //IModelEntityHelperBuilder<TEntity> AddListToConverterFunc<T>(Func<IEnumerable<TEntity>, T> converter) where T : class;
    }
    #endregion

    #region IModelEntityHelperBuilderFactory
    /// <summary>
    /// Helper primitives factory
    /// Набор типовых билдеров 
    /// </summary>
    public interface IModelEntityHelperBuilderFactory
    {
        /// <summary>
        /// IModelEntityHelperBuilder<TEntity> - factory  ( Билдер  ModelEntity - хелпера ) 
        /// </summary>
        IModelEntityHelperBuilder<TEntity> Create<TEntity>() where TEntity : class;
    }
    #endregion



    // Базовая реализация IModelEntityHelper - а
    #region  ModelEntityHelper 
    // реализация IModelEntityHelper  
    //public class ModelEntityHelper<TEntity> : IModelEntityHelper<TEntity>, IDisposable where TEntity : class
    //{
    //    public IModelEntityConverter<TEntity> Converters { get; }
    //    public IModelEntityConverterMaker<TEntity> ConverterMaker { get { return (IModelEntityConverterMaker<TEntity>)this.Converters; } }

    //    public ModelEntityHelper()
    //    {
    //        Converters = new ModelEntityConverter<TEntity>();
    //    }

    //    public void Dispose()
    //    {
    //        var cnv = Converters as IDisposable;
    //        if (cnv != null)
    //        {
    //            cnv.Dispose();
    //        }
    //    }
    //}
    #endregion



    #region ModelEntityHelperBuilder implementation: IModelEntityHelperBuilder<TEntity> 
    //public class ModelEntityHelperBuilder<TEntity> : IDisposable, IModelEntityHelperBuilder<TEntity> where TEntity : class
    //{
    //    //protected ModelEntityHelper<TEntity> helper = new ModelEntityHelper<TEntity>();

    //    public IModelEntityHelperBuilder<TEntity> AddFromConverter<T>(IModelEntityFromConverter<T, TEntity> converter)
    //    {
    //        helper.ConverterMaker.AddFrom<T>(converter);
    //        return this;
    //    }

    //    public IModelEntityHelperBuilder<TEntity> AddFromConverterFunc<T>(Func<T, TEntity> convFunc)
    //    {
    //        helper.ConverterMaker.AddFromFunc<T>(convFunc);
    //        return this;
    //    }

    //    public IModelEntityHelperBuilder<TEntity> AddListFromConverter<T>(IModelEntityListFromConverter<T, TEntity> converter)
    //    {
    //        helper.ConverterMaker.AddListFrom<T>(converter);
    //        return this;
    //    }

    //    public IModelEntityHelperBuilder<TEntity> AddListFromConverterFunc<T>(Func<T, IEnumerable<TEntity>> convFunc)
    //    {
    //        helper.ConverterMaker.AddListFromFunc(convFunc);
    //        return this;
    //    }

    //    public IModelEntityHelperBuilder<TEntity> AddListToConverter<T>(IModelEntityListToConverter<T, TEntity> converter)
    //    {
    //        helper.ConverterMaker.AddListTo<T>(converter);
    //        return this;
    //    }

    //    public IModelEntityHelperBuilder<TEntity> AddListToConverterFunc<T>(Func<IEnumerable<TEntity>, T> convFunc) where T : class
    //    {
    //        helper.ConverterMaker.AddListToFunc<T>(convFunc);
    //        return this;
    //    }

    //    public IModelEntityHelperBuilder<TEntity> AddToConverter<T>(IModelEntityToConverter<T, TEntity> converter)
    //    {
    //        helper.ConverterMaker.AddTo<T>(converter);
    //        return this;
    //    }

    //    public IModelEntityHelperBuilder<TEntity> AddToConverterFunc<T>(Func<TEntity, T> convFunc) where T : class
    //    {
    //        helper.ConverterMaker.AddToFunc<T>(convFunc);
    //        return this;
    //    }

    //    public IModelEntityHelper<TEntity> Build()
    //    {
    //        return helper;
    //    }

    //    public void Dispose()
    //    {
    //        helper.Dispose();
    //    }

    //}
    #endregion

    #region ModelEntityHelperBuilderFactory implementation: IModelEntityHelperBuilderFactory
    //public class ModelEntityHelperBuilderFactory : IModelEntityHelperBuilderFactory
    //{
    //    public IModelEntityHelperBuilder<TEntity> Create<TEntity>() where TEntity : class
    //    {
    //        return new ModelEntityHelperBuilder<TEntity>();
    //    }

    //    //public IModelEntityBuilder<T, TEntity> CreateBulder<T, TEntity>(Func<T, TEntity> func) where TEntity : class
    //    //{
    //    //    return new ModelEntityBuilder<T, TEntity>(func);
    //    //}

    //    //public IModelEntityConverter<T, TEntity> CreateConverter<T, TEntity>(Func<TEntity, T> func) where T: class
    //    //{
    //    //    return new ModelEntityConverter<T,TEntity>(func);
    //    //}
    //}
    #endregion


    #region IEntityModel
    /// <summary>
    /// Интерфейс представления энтити модели на стороне контроллера
    /// </summary>
    public interface IEntityModelProvider<TEntity> where TEntity : class
    {
        IDalAccess<TEntity> Dal { get; }
        IModelEntityConverter<TEntity> Converters { get; }
    }

    #endregion

    #region IEntityModelProviderBuilder
    public interface IEntityModelProviderBuilder<TEntity>
    {

    }

    #endregion


    /// <summary>
    /// представления энтити модели на стороне контроллера
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    class EntityModelProvider<TEntity> : IEntityModelProvider<TEntity> where TEntity : class
    {
        private IDalAccess<TEntity> dal = null;
        private IModelEntityConverter<TEntity> converters = null;

        public IDalAccess<TEntity> Dal => dal;
        public IModelEntityConverter<TEntity> Converters => converters;


        public EntityModelProvider() : this(null, null) { }
        public EntityModelProvider(IModelEntityConverter<TEntity> _converters) : this(null, _converters) { }
        public EntityModelProvider(IDalAccess<TEntity> _dal) : this(_dal, null) { }

        public EntityModelProvider(IDalAccess<TEntity> _dal, IModelEntityConverter<TEntity> _converters)
        {
            dal = _dal;
            converters = _converters;
        }
    }

}
