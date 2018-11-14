//using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGateApi.Base
{
    //  IDalReader<TEntity>,  IDalInserter<TEntity>, IDalUpdater<TEntity>, IDalRemover<TEntity> - в фасад  IDalAccess<TEntity>

    // Это базовые интерфейсы контроллера.
    #region DAL GRUD Typed <TEntity> interfaces

    public interface IDalReader<TEntity> 
    {
        IEnumerable<TEntity> Get(IEnumerable<KeyValuePair<string, object>> pars);
        IEnumerable<TEntity> GetAll();
        TEntity Find(object key);
    }

    public interface IDalInserter<TEntity> //where TEntity : class
    {
        //void  Add(TEntity item);
        TEntity Add(TEntity item);
        // 091012 Extend interface for wock with key and rec template
        // object  AddGetKey(TEntity item);
        TEntity GetTemplate();
    }

    public interface IDalUpdater<TEntity>
    {
        void Update(TEntity item);
    }

    public interface IDalRemover<TEntity>
    {
        TEntity Remove(object key);
    }

    //public interface IDal<TEntity> : IDalReader<TEntity>, IDalInserter<TEntity>, IDalUpdater<TEntity>, IDalRemover<TEntity>
    //{
    //
    //}

    #endregion DAL GRUD Typed <TEntity> interfaces

    // Интерфейс предоставляющий набор GRUD объектов
    #region IDalAccess<TEntity>
    public interface IDalAccess<TEntity> where TEntity : class
    {
        IDalReader<TEntity> Reader { get; }
        IDalInserter<TEntity> Inserter { get; }
        IDalUpdater<TEntity> Updater { get; }
        IDalRemover<TEntity> Remover { get; }
    }
    #endregion

    // Прикладной интерфейс акцесс - билдера 
    #region IDalProvAccessBuilder<TEntity, TContext>                            
    public interface IDalProvAccessBuilder<TEntity, TSource> : IDalAccessBuilder<TEntity, TSource, IDalProvAccessBuilder<TEntity, TSource>>, IBuilder<IDalAccess<TEntity>> where TEntity : class //where TEntity : class where TSource : class
    {

    }
    #region old
    //public interface IDalProvAccessBuilder<TEntity, TSource > : IBuilder<IDalAccess<TEntity>>  //where TEntity : class where TSource : class
    //{
    //    IDalProvAccessBuilder<TEntity, TSource> SetSource(TSource source);
    //    IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IDalReader<TEntity>> reader);
    //    IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind);
    //    IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(
    //        Func<TSource, IEnumerable<TEntity>> funcReadAll
    //        , Func<TSource, object, TEntity> funcFind
    //        , Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet);

    //    IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, IDalInserter<TEntity>> inserter);
    //    IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Action<TSource, TEntity> funcAdd);

    //    IDalProvAccessBuilder<TEntity, TSource> SetUpdateAccess(Func<TSource, IDalUpdater<TEntity>> updater);
    //    IDalProvAccessBuilder<TEntity, TSource> SetUpdateAccess(Action<TSource, TEntity> funcUpdate);

    //    IDalProvAccessBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, IDalRemover<TEntity>> updater);
    //    IDalProvAccessBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, object, TEntity> funcUpdate);

    //    //IDalAccess<TEntity> Build();
    //}
    #endregion
    #endregion

    // Абстрактный интерфейс акцесс - билдера 
    #region IDalAccessBuilder<TEntity, TContext>                            
    public interface IDalAccessBuilder<TEntity, TSource, TBuilder> //where TEntity : class //where TEntity : class where TSource : class
    {
        TBuilder SetSource(TSource source);
        TBuilder SetReadAccess(Func<TSource, IDalReader<TEntity>> reader);
        TBuilder SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind);

        TBuilder SetReadAccess(
            Func<TSource, IEnumerable<TEntity>> funcReadAll
            , Func<TSource, object, TEntity> funcFind
            , Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet);

        //220218  
        TBuilder SetReadAccess(
            Func<TSource, IEnumerable<TEntity>> funcReadAll
            , Func<TSource, object, TEntity> funcFind
            , Func< IEnumerable<TEntity>,IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet);

        TBuilder SetInsertAccess(Func<TSource, IDalInserter<TEntity>> inserter);
        //TBuilder SetInsertAccess(Action<TSource, TEntity> funcAdd, TEntity recTemplate);
        //TBuilder SetInsertAccess(Action<TSource, TEntity> funcAdd, Func<TSource, TEntity> templateFunc);
        TBuilder SetInsertAccess(Func<TSource, TEntity, TEntity> funcAdd, TEntity recTemplate);
        TBuilder SetInsertAccess(Func<TSource, TEntity, TEntity> funcAdd, Func<TSource, TEntity> templateFunc);


        TBuilder SetUpdateAccess(Func<TSource, IDalUpdater<TEntity>> updater);
        TBuilder SetUpdateAccess(Action<TSource, TEntity> funcUpdate);

        TBuilder SetRemoveAccess(Func<TSource, IDalRemover<TEntity>> updater);
        TBuilder SetRemoveAccess(Func<TSource, object, TEntity> funcUpdate);
    }
    #endregion

    // Фабрика Акцесс  Билдеров
    #region IDalProvAccessBuilderFactory               
    public interface IDalProvAccessBuilderFactory
    {
        IDalProvAccessBuilder<TEntity, TSource> Create<TEntity, TSource>() where TEntity : class where TSource : class;
        //AccessBuilder
    }
    #endregion
    
    
}
