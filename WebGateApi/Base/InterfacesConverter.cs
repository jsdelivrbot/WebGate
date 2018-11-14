using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WebGateApi.Base
{

    #region IModelEntityConverter<TEntity>    
    /// <summary>
    /// Интерфейс конвертеров TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IModelEntityConverter<TEntity>                            
    {
        IModelEntityFromConverter<T, TEntity> GetFromConverter<T>();            
        IModelEntityToConverter<T, TEntity> GetToConverter<T>();                

        IModelEntityListFromConverter<T, TEntity> GetListFromConverter<T>();
        IModelEntityListToConverter<T, TEntity> GetListToConverter<T>();
    }
    #endregion

    #region IModelEntityConverterBuilder<TEntity,TBuilder>     Абстрактный Интерфейс билдера конверторов
    /// <summary>
    /// Абстрактный Интерфейс билдера конверторов  TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IModelEntityConverterBuilder<TEntity,TBuilder> 
    {
        TBuilder AddFrom<T>(IModelEntityFromConverter<T, TEntity> converter);
        TBuilder AddTo<T>(IModelEntityToConverter<T, TEntity> converter);
        TBuilder AddListFrom<T>(IModelEntityListFromConverter<T, TEntity> converter);
        TBuilder AddListTo<T>(IModelEntityListToConverter<T, TEntity> converter);

        TBuilder AddFromFunc<T>(Func<T, TEntity> convFunc);
        TBuilder AddToFunc<T>(Func<TEntity, T> convFunc) where T : class;
        TBuilder AddListFromFunc<T>(Func<T, IEnumerable<TEntity>> convFunc);
        TBuilder AddListToFunc<T>(Func<IEnumerable<TEntity>, T> convFunc) where T : class;
    }
    #endregion

    #region IModelEntityConverterMaker<TEntity> Прикладной интерфейс билдера конверторов
    /// <summary>
    /// Прикладной интерфейс билдера конверторов  TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IModelEntityConverterBuilder<TEntity> : IModelEntityConverterBuilder<TEntity, IModelEntityConverterBuilder<TEntity>>, IBuilder<IModelEntityConverter<TEntity>>
    {
    }
    #endregion

    #region IModelEntityConverterMaker<TEntity> Прикладной интерфейс билдера конверторов
    /// <summary>
    /// Прикладной интерфейс конвертор-мэйкера (для работы в рантайме на дале)  TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IModelEntityConverterMaker<TEntity> : IModelEntityConverterBuilder<TEntity, bool> 
    {
    }
    #endregion

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

}
