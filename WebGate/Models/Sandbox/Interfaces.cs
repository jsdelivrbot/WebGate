using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Models.Sandbox
{
    // Простейщему CRUD контроллеру надо че та типа  IDal<TEntity>
    // Проблема в том что данный интерфейс идентифицируется только по TEntity 
    // и идентифицировать в контексте базы его нельзя (может быть несколько объектов одной структуры TEntity), 
    // не говоря уже о идентификации базы.
    //  
    // воще тупому контроллеру должно быть достаточно IDal<TEntity>
    // Для инъекции нужного сета надо либо строже типизировать его на контроллере и в сервис пуле,
    // либо хранить где-то ссылку на контроллер ? 

    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // На IServiceCollection вешаются синглтоны : 
    // IDalAccessBuilderFactory, DB-Контексты, 
    // Трансиенты : DB-СЕРВИСЫ
    // КОНТРОЛЛЕР -> CЕРВИС -> DAL -> БАЗА 
    // Контроллер инектируется  CЕРВИСОМ со свойствами реализующими интерфейс IDalAccess который представляет набор 
    // GRUID интерфейсов (IDalReader<TEntity>  IDalInserter<TEntity> IDalUpdater<TEntity> IDalRemover<TEntity>)
    // CЕРВИС инектируется БАЗОЙ (dbcontext) и Фабрикой акцес билдеров (IDalAccessBuilderFactory)
    // и должен реализовывать логическую модель и обеспечить взаимодействие с КОНТРОЛЛЕРОМ 
    // реализуя IDalAccess. 


    // Это базовые интерфейсы контроллера.
    #region DAL GRUD Typed <TEntity> interfaces

    public interface IDalReader<TEntity>
    {
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
        IDalReader<TEntity>     Reader      { get; }
        IDalInserter<TEntity>   Inserter    { get; }
        IDalUpdater<TEntity>    Updater     { get; }
        IDalRemover<TEntity>    Remover     { get; }
    }
    #endregion


    // IDalAccessBuilder<TEntity, TContext> 
    #region IDalAccessBuilder<TEntity, TContext>                            
    public interface IDalAccessBuilder<TEntity, TContext> where TEntity : class where TContext : DbContext
    {
        IDalAccessBuilder<TEntity, TContext> SetDbContext(TContext context);
        IDalAccessBuilder<TEntity, TContext> SetReadAccess(DbSet<TEntity> dbset);
        IDalAccessBuilder<TEntity, TContext> SetReadAccess(Func<TContext, IDalReader<TEntity>> funcReader);
        IDalAccessBuilder<TEntity, TContext> SetReadAccess(Func<TContext, IEnumerable<TEntity>> funcReadAll, Func<TContext,object,TEntity> funcFind );

        IDalAccess<TEntity> Build();
    }
    #endregion

    // Фабрика Билдеров
    #region IDalAccessBuilderFactory               
    public interface IDalAccessBuilderFactory
    {
        IDalAccessBuilder<TEntity, TContext> Create<TEntity, TContext>() where TEntity : class where TContext : DbContext;
    }
    #endregion

    // IDalProvAccessBuilder<TEntity, TContext> 
    #region IDalProvAccessBuilder<TEntity, TContext>                            
    public interface IDalProvAccessBuilder<TEntity, TSource> where TEntity : class where TSource : class
    {
        IDalProvAccessBuilder<TEntity, TSource> SetSource(TSource source);
        IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IDalReader<TEntity>> funcReader);
        IDalProvAccessBuilder<TEntity, TSource> SetReadAccess(Func<TSource, IEnumerable<TEntity>> funcReadAll, Func<TSource, object, TEntity> funcFind);

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

    // Нужен некий интерфейс работающий без фиксации объекта, но позволяющий реализовывать DAL интерфейсы  
    // типа : ридер   dbcontext -> IEnumerable<TEntity>  аля  GetAll();
    //                dbcontext -> TEntity               аля  Find(object key);  

    //IDalReaderProvider
    #region IDalDbReader
    public interface IDalDbReader<TEntity>
    {
        IEnumerable<TEntity> GetAll(DbContext context);
        TEntity Find( DbContext context, object key);

        IDalReader<TEntity> Reader(DbContext context);  
    }
    #endregion IDalDbReader



    #region DAL GRUD Common interfaces

    public interface IDalReader
    {
        IEnumerable<TEntity> GetAll<TEntity>();
        TEntity Find<TEntity>(object key);
    }

    public interface IDalInserter : IDalReader
    {
        void Add<TEntity>(TEntity item);
    }

    public interface IDalEditor : IDalInserter
    {
        TEntity Remove<TEntity>(object key);
        void Update<TEntity>(TEntity item);
    }

    #endregion 
}


