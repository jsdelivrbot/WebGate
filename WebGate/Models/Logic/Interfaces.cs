using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebGate.Models.Logic
{

    // Интерфейс помошника сущности логической (физической) модели 
    // Пока реализует билдер и конвертер

    public interface IModelEntityConverters<TEntity>                            //where TEntity : class 
    {
        IModelEntityFromConverter<T, TEntity> GetFromConverter<T>();               //where T : class; 
        IModelEntityToConverter<T, TEntity> GetToConverter<T>();                 //where T : class; 

        IModelEntityListFromConverter<T, TEntity> GetListFromConverter<T>();
        IModelEntityListToConverter<T, TEntity> GetListToConverter<T>();

    }

    public interface IModelEntityConverterMaker<TEntity>                            //where TEntity : class
    {
        void AddFrom<T>(IModelEntityFromConverter<T, TEntity> converter);
        void AddTo<T>(IModelEntityToConverter<T, TEntity> converter);
        void AddListFrom<T>(IModelEntityListFromConverter<T, TEntity> converter);
        void AddListTo<T>(IModelEntityListToConverter<T, TEntity> converter);

        void AddFromFunc<T>(Func<T, TEntity> convFunc);
        void AddToFunc<T>(Func<TEntity, T> convFunc) where T : class; 
        void AddListFromFunc<T>(Func<T, IEnumerable<TEntity>> convFunc);
        void AddListToFunc<T>(Func<IEnumerable<TEntity>, T> convFunc) where T : class;

    }



    public interface IModelEntityHelper<TEntity> 
    {
        IModelEntityConverters<TEntity> Converters { get; }
    }

    #region Base Conversion interfaces
    /// <summary>
    /// Create TEntity-object from T type 
    /// </summary>
    public interface IModelEntityFromConverter<T,TEntity> //where TEntity : class           ///where T: class
    {
        TEntity Convert(T fromObject);
    }

    /// <summary>
    /// Create IEnumerable<TEntity>-object from T type 
    /// </summary>
    public interface IModelEntityListFromConverter<T, TEntity> //where TEntity : class           ///where T: class
    {
        IEnumerable<TEntity> Convert(T fromObject);
    }


    /// <summary>
    /// Convert  TEntity-object to T type
    /// </summary>
    public interface IModelEntityToConverter<T, TEntity> //where TEntity : class where T : class
    {
        T Convert(TEntity toObject);
    }

    /// <summary>
    /// Convert  IEnumerable<TEntity>-object to T type
    /// </summary>
    public interface IModelEntityListToConverter<T, TEntity> //where TEntity : class where T : class
    {
        T Convert(IEnumerable<TEntity> toObject);
    }
    #endregion


    // прикладной интерфейс стороны логики для формирования ModelEntity - хелпера
    // Интерфейс создания IModelEntityHelper
    public interface IModelEntityHelperBuilder<TEntity> 
    {
        // блять типом идентифицировать или имя добавить ?

        IModelEntityHelperBuilder<TEntity> AddFromConverter<T>(IModelEntityFromConverter<T, TEntity> converter);
        IModelEntityHelperBuilder<TEntity> AddFromConverterFunc<T>(Func<T, TEntity> convFunc);

        IModelEntityHelperBuilder<TEntity> AddToConverter<T>(IModelEntityToConverter<T, TEntity> converter);     
        IModelEntityHelperBuilder<TEntity> AddToConverterFunc<T>(Func<TEntity,T> convFunc)                                  where T : class;

        IModelEntityHelperBuilder<TEntity> AddListFromConverter<T>(IModelEntityListFromConverter<T, TEntity> converter);
        IModelEntityHelperBuilder<TEntity> AddListFromConverterFunc<T>(Func<T, IEnumerable<TEntity>> convFunc);

        IModelEntityHelperBuilder<TEntity> AddListToConverter<T>(IModelEntityListToConverter<T, TEntity> converter);
        IModelEntityHelperBuilder<TEntity> AddListToConverterFunc<T>(Func<IEnumerable<TEntity>, T> converter)               where T : class;

     IModelEntityHelper<TEntity> Build();
    }



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
        IModelEntityHelperBuilder<TEntity> Create<TEntity>() where TEntity : class ;

        /// <summary>
        /// IModelEntityBuilder<T,TEntity> - factory
        /// </summary>
        //IModelEntityBuilder<T,TEntity> CreateBulder<T,TEntity>(Func<T, TEntity> func) where TEntity : class ;

        /// <summary>
        /// IModelEntityConverter<T,TEntity> - factory
        /// </summary>
        //IModelEntityConverter<T,TEntity> CreateConverter<T,TEntity>(Func<TEntity,T> func) where T : class ; 
    }
    #endregion

}
