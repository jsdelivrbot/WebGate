using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebGateApi.Base
{
    //
    // Интерфейсы метаданных
    //

    /// <summary>
    /// Transitives Metadata collection
    /// </summary>
    /// <typeparam name="TRet">Type of metadata values </typeparam>
    /// <typeparam name="TKey">Type of metadata key</typeparam>
    public interface IMetadataBase<TRet, TKey>  
    {
        IEnumerable<KeyValuePair<TKey, TRet>> GetAll();
        IEnumerable<KeyValuePair<TKey, TRet>> Get(IEnumerable<TKey> values);
        TRet Find(TKey metadataKey);
    }

    /// <summary>
    /// Indexed transitives Metadata collection
    /// </summary>
    /// <typeparam name="TRet">Type of metadata values</typeparam>
    /// <typeparam name="TKey">Type of metadata key</typeparam>
    /// <typeparam name="TIdx">Type of metadata index</typeparam>
    public interface IMetadataBaseIdx<TRet, TKey, TIdx>
    {
        IMetadataBase<TRet, TKey> this[TIdx idx] { get; }
    }

    public interface IMetadatasBase<TRet, TKey, TIdx>
    {
        IMetadataBase<TRet, TKey> Metadata { get; }
        IMetadataBaseIdx<TRet, TKey, TIdx> MetadataField { get;}
    }
    
    public interface IMetadata : IMetadataBase<object, string>
    {
    }

    public interface IMetadataIdx : IMetadataBaseIdx<object, string, string>
    {
    }

    public interface IMetadatas : IMetadatasBase<object, string, string>
    {
    }


    //public interface IMetadatas
    //{
    //    IMetadata Metadata();
    //    IMetadata MetadataField(string fieldName);
    //}


    #region  Builders
    /// <summary>
    /// Абстрактный Интерфейс билдера метаданных  TEntity
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IMetadatasBuilder<TBuilder> 
    {
        TBuilder Add(string key, object val );
        TBuilder AddField(string fieldName , string key, object val);
    }
    #endregion

}
