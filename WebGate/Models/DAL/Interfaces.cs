using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Models.DAL
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

    public interface IDalInserter<TEntity>
    {
        void Add(TEntity item);
    }

    public interface IDalUpdater<TEntity>
    {
        void Update(TEntity item);
    }

    public interface IDalRemover<TEntity>
    {
        TEntity Remove(object key);
    }

    public interface IDal<TEntity> : IDalReader<TEntity>, IDalInserter<TEntity>, IDalUpdater<TEntity>, IDalRemover<TEntity>
    {

    }

    #endregion DAL GRUD Typed <TEntity> interfaces

    // Интерфейс предоставляющий набор GRUD объектов
    #region IDalAccess<TEntity>
    public interface IDalAccess<TEntity>
    {
        IDalReader<TEntity> Reader { get; }
        IDalInserter<TEntity> Inserter { get; }
        IDalUpdater<TEntity> Updater { get; }
        IDalRemover<TEntity> Remover { get; }
    }
    #endregion

    // IDalProvAccessBuilder<TEntity, TContext> 
    #region IDalProvAccessBuilder<TEntity, TContext>                            
    public interface IDalProvAccessBuilder<TEntity, TSource> where TEntity : class where TSource : class
    {
        IDalProvAccessBuilder<TEntity, TSource> SetSource(TSource source);

        IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IDalReader<TEntity>> reader);
        IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind);
        IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(
            Func<TSource, IEnumerable<TEntity>> funcReadAll
            , Func<TSource, object, TEntity> funcFind
            , Func<TSource, IEnumerable<KeyValuePair<string, object>>, IEnumerable<TEntity>> funcGet);

        IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Func<TSource, IDalInserter<TEntity>> inserter);
        IDalProvAccessBuilder<TEntity, TSource> SetInsertAccess(Action<TSource, TEntity> funcAdd);

        IDalProvAccessBuilder<TEntity, TSource> SetUpdateAccess(Func<TSource, IDalUpdater<TEntity>> updater);
        IDalProvAccessBuilder<TEntity, TSource> SetUpdateAccess(Action<TSource, TEntity> funcUpdate);

        IDalProvAccessBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, IDalRemover<TEntity>> updater);
        IDalProvAccessBuilder<TEntity, TSource> SetRemoveAccess(Func<TSource, object, TEntity> funcUpdate);

        IDalAccess<TEntity> Build();
    }
    #endregion

    // Фабрика Билдеров
    #region IDalProvAccessBuilderFactory               
    public interface IDalProvAccessBuilderFactory
    {
        IDalProvAccessBuilder<TEntity, TSource> Create<TEntity, TSource>() where TEntity : class where TSource : class;
    }

    #endregion

    
}
