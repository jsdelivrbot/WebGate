using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Models.Logic
{
    // Классы конверторы базовые
    #region  ModelEntityTransform implementation: IModelEntityBuilder<T, TEntity>,  IModelEntityConverter<T, TEntity> ....
    /// <summary>
    ///  Класс акумулирующий функцию преобразования
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public class ModelEntityTransform<TFrom, TTo>  where TTo : class
    {
        protected Func<TFrom, TTo> _func { get; set; }

        protected ModelEntityTransform(Func<TFrom, TTo> func)
        {
            _func = func;
        }

        protected TTo Transform(TFrom fromObject)
        {
            return (_func != null) ? _func(fromObject) : null;
        }
    }

    public class ModelEntityFromConverter<T, TEntity> : ModelEntityTransform<T, TEntity> , IModelEntityFromConverter<T, TEntity>  where TEntity : class // where T : class
    {
        public ModelEntityFromConverter(Func<T, TEntity> func) : base(func) { }
        public TEntity Convert(T fromObject) => this.Transform(fromObject);
    }

    public class ModelEntityToConverter<T, TEntity> : ModelEntityTransform<TEntity,T> , IModelEntityToConverter<T, TEntity> where T : class // where TEntity : class
    {
        public ModelEntityToConverter(Func<TEntity, T> func) : base(func) { }
        public T Convert(TEntity fromObject) => this.Transform(fromObject);
    }

    public class ModelEntityListFromConverter<T, TEntity> : ModelEntityTransform<T, IEnumerable<TEntity>>, IModelEntityListFromConverter<T, TEntity> where TEntity : class // where T : class
    {
        public ModelEntityListFromConverter(Func<T, IEnumerable<TEntity> > func) : base(func) { }
        public IEnumerable<TEntity> Convert(T fromObject) => this.Transform(fromObject);
    }

    public class ModelEntityListToConverter<T, TEntity> : ModelEntityTransform<IEnumerable<TEntity>, T> , IModelEntityListToConverter<T, TEntity> where T : class // where TEntity : class
    {
        public ModelEntityListToConverter(Func<IEnumerable<TEntity>, T> func) : base(func) { }
        public T Convert(IEnumerable<TEntity> fromObject) => this.Transform(fromObject);
    }



    #endregion

    // Базовая реализация IModelEntityHelper - а
    #region  ModelEntityHelper 
    // реализация IModelEntityHelper  
    public class ModelEntityHelper<TEntity> : IModelEntityHelper<TEntity>, IDisposable where TEntity : class
    {
        public IModelEntityConverters<TEntity> Converters { get; }
        public IModelEntityConverterMaker<TEntity> ConverterMaker { get { return (IModelEntityConverterMaker<TEntity>)this.Converters; } }

        public ModelEntityHelper()
        {
            Converters = new ModelEntityConverters<TEntity>();
        }

        public void Dispose()
        {
            var cnv = Converters as IDisposable;
            if (cnv != null)
            {
                cnv.Dispose();
            }
        }
    }
    #endregion

    // Базовая реализация IModelEntityConverters - а  ( элемент IModelEntityHelper )
    #region ModelEntityConverters<TEntity> implementation: IModelEntityConverters<TEntity> 
    
    /// <summary>
    ///  Набор конверторов 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ModelEntityConverters<TEntity> : IModelEntityConverters<TEntity>, IModelEntityConverterMaker<TEntity> , IDisposable  where TEntity : class
    {
        enum ConvType { From, To, FromList, ToList };

        private Dictionary<ConvType, Dictionary<Type, object>> _converters = new Dictionary<ConvType, Dictionary<Type, object>>();

        public ModelEntityConverters()
        {
            _converters.Add(ConvType.From, new Dictionary<Type, object>());
            _converters.Add(ConvType.To, new Dictionary<Type, object>());
            _converters.Add(ConvType.FromList, new Dictionary<Type, object>());
            _converters.Add(ConvType.ToList, new Dictionary<Type, object>());
        }

        //public ModelEntityHelper(Dictionary<Type, object> bilders, Dictionary<Type, object> converters)
        //{
        //    //_bilders = bilders;
        //    //_converters = converters;
        //}

        public void Dispose()
        {
            _converters[ConvType.From] = null;
            _converters[ConvType.To] = null;
            _converters[ConvType.FromList] = null;
            _converters[ConvType.ToList] = null;
            _converters = null;
        }

        public IModelEntityFromConverter<T, TEntity> GetFromConverter<T>()
        {
            return _converters[ConvType.From].ContainsKey(typeof(T)) ? (IModelEntityFromConverter<T, TEntity>)_converters[ConvType.From][typeof(T)] : null;
        }

        public IModelEntityToConverter<T, TEntity> GetToConverter<T>()
        {
            return _converters[ConvType.To].ContainsKey(typeof(T)) ? (IModelEntityToConverter<T, TEntity>)_converters[ConvType.To][typeof(T)] : null;
        }

        public IModelEntityListFromConverter<T, TEntity> GetListFromConverter<T>()
        {
            return _converters[ConvType.FromList].ContainsKey(typeof(T)) ? (IModelEntityListFromConverter<T, TEntity>)_converters[ConvType.FromList][typeof(T)] : null;
        }

        public IModelEntityListToConverter<T, TEntity> GetListToConverter<T>()
        {
            return _converters[ConvType.ToList].ContainsKey(typeof(T)) ? (IModelEntityListToConverter<T, TEntity>)_converters[ConvType.ToList][typeof(T)] : null;
        }

        /// <summary>
        /// Методы построения ()
        /// </summary>
        public void AddFrom<T>(IModelEntityFromConverter<T, TEntity> converter) => _converters[ConvType.From].Add(typeof(T), converter);
        public void AddTo<T>(IModelEntityToConverter<T, TEntity> converter) => _converters[ConvType.To].Add(typeof(T), converter);

        public void AddListFrom<T>(IModelEntityListFromConverter<T, TEntity> converter) => _converters[ConvType.FromList].Add(typeof(T), converter);
        public void AddListTo<T>(IModelEntityListToConverter<T, TEntity> converter) => _converters[ConvType.ToList].Add(typeof(T), converter);

        public void AddFromFunc<T>(Func<T, TEntity> convFunc)                                   => this.AddFrom(new ModelEntityFromConverter<T, TEntity>(convFunc));
        public void AddToFunc<T>(Func<TEntity, T> convFunc) where T : class                     => this.AddTo(new ModelEntityToConverter<T, TEntity>(convFunc));

        public void AddListFromFunc<T>(Func<T, IEnumerable<TEntity>> convFunc)                  => this.AddListFrom(new ModelEntityListFromConverter<T, TEntity>(convFunc));
        public void AddListToFunc<T>(Func<IEnumerable<TEntity>, T> converter) where T : class   => this.AddListTo(new ModelEntityListToConverter<T, TEntity>(converter));
        
    }
    #endregion



    #region ModelEntityHelperBuilder implementation: IModelEntityHelperBuilder<TEntity> 
    public class ModelEntityHelperBuilder<TEntity> : IDisposable, IModelEntityHelperBuilder<TEntity>   where TEntity : class
    {
        protected ModelEntityHelper<TEntity> helper = new ModelEntityHelper<TEntity>();

        public IModelEntityHelperBuilder<TEntity> AddFromConverter<T>(IModelEntityFromConverter<T, TEntity> converter)
        {
            helper.ConverterMaker.AddFrom<T>(converter);
            return this ;
        }

        public IModelEntityHelperBuilder<TEntity> AddFromConverterFunc<T>(Func<T, TEntity> convFunc)
        {
            helper.ConverterMaker.AddFromFunc<T>(convFunc);
            return this;
        }

        public IModelEntityHelperBuilder<TEntity> AddListFromConverter<T>(IModelEntityListFromConverter<T, TEntity> converter)
        {
            helper.ConverterMaker.AddListFrom<T>(converter);
            return this;
        }

        public IModelEntityHelperBuilder<TEntity> AddListFromConverterFunc<T>(Func<T, IEnumerable<TEntity>> convFunc)
        {
            helper.ConverterMaker.AddListFromFunc(convFunc);
            return this;
        }

        public IModelEntityHelperBuilder<TEntity> AddListToConverter<T>(IModelEntityListToConverter<T, TEntity> converter)
        {
            helper.ConverterMaker.AddListTo<T>(converter); 
            return this;
        }

        public IModelEntityHelperBuilder<TEntity> AddListToConverterFunc<T>(Func<IEnumerable<TEntity>, T> convFunc) where T : class
        {
            helper.ConverterMaker.AddListToFunc<T>(convFunc);
            return this;
        }

        public IModelEntityHelperBuilder<TEntity> AddToConverter<T>(IModelEntityToConverter<T, TEntity> converter)
        {
            helper.ConverterMaker.AddTo<T>(converter);
            return this;
        }

        public IModelEntityHelperBuilder<TEntity> AddToConverterFunc<T>(Func<TEntity, T> convFunc) where T : class
        {
            helper.ConverterMaker.AddToFunc<T>(convFunc);
            return this;
        }

        public IModelEntityHelper<TEntity> Build()
        {
            return helper;
        }

        public void Dispose()
        {
            helper.Dispose();
        }
      
    }
    #endregion


    #region ModelEntityHelperBuilderFactory implementation: IModelEntityHelperBuilderFactory
    public class ModelEntityHelperBuilderFactory : IModelEntityHelperBuilderFactory
    {
        public IModelEntityHelperBuilder<TEntity> Create<TEntity>() where TEntity : class
        {
            return new ModelEntityHelperBuilder<TEntity>();
        }

        //public IModelEntityBuilder<T, TEntity> CreateBulder<T, TEntity>(Func<T, TEntity> func) where TEntity : class
        //{
        //    return new ModelEntityBuilder<T, TEntity>(func);
        //}
       
        //public IModelEntityConverter<T, TEntity> CreateConverter<T, TEntity>(Func<TEntity, T> func) where T: class
        //{
        //    return new ModelEntityConverter<T,TEntity>(func);
        //}
    }
    #endregion

}
