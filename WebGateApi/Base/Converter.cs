using System;
using System.Collections.Generic;
using System.Text;

namespace WebGateApi.Base
{
    // Классы конверторы базовые
    #region  ModelEntityTransform implementation: IModelEntityBuilder<T, TEntity>,  IModelEntityConverter<T, TEntity> ....
    /// <summary>
    ///  Класс акумулирующий функцию преобразования
    /// </summary>
    /// <typeparam name="TFrom"></typeparam>
    /// <typeparam name="TTo"></typeparam>
    public class ModelEntityTransform<TFrom, TTo> where TTo : class
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

    public class ModelEntityFromConverter<T, TEntity> : ModelEntityTransform<T, TEntity>, IModelEntityFromConverter<T, TEntity> where TEntity : class // where T : class
    {
        public ModelEntityFromConverter(Func<T, TEntity> func) : base(func) { }
        public TEntity Convert(T fromObject) => this.Transform(fromObject);
    }

    public class ModelEntityToConverter<T, TEntity> : ModelEntityTransform<TEntity, T>, IModelEntityToConverter<T, TEntity> where T : class // where TEntity : class
    {
        public ModelEntityToConverter(Func<TEntity, T> func) : base(func) { }
        public T Convert(TEntity fromObject) => this.Transform(fromObject);
    }

    public class ModelEntityListFromConverter<T, TEntity> : ModelEntityTransform<T, IEnumerable<TEntity>>, IModelEntityListFromConverter<T, TEntity> where TEntity : class // where T : class
    {
        public ModelEntityListFromConverter(Func<T, IEnumerable<TEntity>> func) : base(func) { }
        public IEnumerable<TEntity> Convert(T fromObject) => this.Transform(fromObject);
    }

    public class ModelEntityListToConverter<T, TEntity> : ModelEntityTransform<IEnumerable<TEntity>, T>, IModelEntityListToConverter<T, TEntity> where T : class // where TEntity : class
    {
        public ModelEntityListToConverter(Func<IEnumerable<TEntity>, T> func) : base(func) { }
        public T Convert(IEnumerable<TEntity> fromObject) => this.Transform(fromObject);
    }



    #endregion

    // Базовая реализация IModelEntityConverter  
    #region ModelEntityConverter<TEntity> implementation: IModelEntityConverters<TEntity> 

    /// <summary>
    ///  Набор конверторов 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ModelEntityConverter<TEntity> : IModelEntityConverter<TEntity>, IModelEntityConverterMaker<TEntity>, IDisposable where TEntity : class
    {
        enum ConvType { From, To, FromList, ToList };

        private Dictionary<ConvType, Dictionary<Type, object>> _converters = new Dictionary<ConvType, Dictionary<Type, object>>();

        public ModelEntityConverter()
        {
            _converters.Add(ConvType.From, new Dictionary<Type, object>());
            _converters.Add(ConvType.To, new Dictionary<Type, object>());
            _converters.Add(ConvType.FromList, new Dictionary<Type, object>());
            _converters.Add(ConvType.ToList, new Dictionary<Type, object>());
        }


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

        private Func<Action, bool> toTrue = (a => true);

        /// <summary>
        /// Методы построения ()
        /// </summary>
        public bool AddFrom<T>(IModelEntityFromConverter<T, TEntity> converter) { _converters[ConvType.From].Add(typeof(T), converter);  return true;}
        public bool AddTo<T>(IModelEntityToConverter<T, TEntity> converter) { _converters[ConvType.To].Add(typeof(T), converter); return true; }
        public bool AddListFrom<T>(IModelEntityListFromConverter<T, TEntity> converter){ _converters[ConvType.FromList].Add(typeof(T), converter);  return true;}
        public bool AddListTo<T>(IModelEntityListToConverter<T, TEntity> converter) { _converters[ConvType.ToList].Add(typeof(T), converter);  return true;}


        public bool AddFromFunc<T>(Func<T, TEntity> convFunc) => this.AddFrom(new ModelEntityFromConverter<T, TEntity>(convFunc));
        public bool AddToFunc<T>(Func<TEntity, T> convFunc) where T : class => this.AddTo(new ModelEntityToConverter<T, TEntity>(convFunc));
        public bool AddListFromFunc<T>(Func<T, IEnumerable<TEntity>> convFunc) => this.AddListFrom(new ModelEntityListFromConverter<T, TEntity>(convFunc));
        public bool AddListToFunc<T>(Func<IEnumerable<TEntity>, T> converter) where T : class => this.AddListTo(new ModelEntityListToConverter<T, TEntity>(converter));

    }
    #endregion

    // Базовая реализация IModelEntityConverterBuilder<TEntity>
    #region Билдер конвертера
    public class ModelEntityConverterBuilder<TEntity> : IModelEntityConverterBuilder<TEntity> where TEntity : class
    {
        private ModelEntityConverter<TEntity> convertObj = new ModelEntityConverter<TEntity>();

        #region  interface IModelEntityConverterBuilder implement
        public IModelEntityConverterBuilder<TEntity> AddFrom<T>(IModelEntityFromConverter<T, TEntity> converter)
        {
            convertObj.AddFrom<T>(converter);  return this;
        }

        public IModelEntityConverterBuilder<TEntity> AddFromFunc<T>(Func<T, TEntity> convFunc)
        {
            convertObj.AddFromFunc<T>(convFunc); return this;
        }

        public IModelEntityConverterBuilder<TEntity> AddListFrom<T>(IModelEntityListFromConverter<T, TEntity> converter)
        {
            convertObj.AddListFrom<T>(converter); return this;
        }

        public IModelEntityConverterBuilder<TEntity> AddListFromFunc<T>(Func<T, IEnumerable<TEntity>> convFunc)
        {
            convertObj.AddListFromFunc<T>(convFunc); return this;
        }

        public IModelEntityConverterBuilder<TEntity> AddListTo<T>(IModelEntityListToConverter<T, TEntity> converter)
        {
            convertObj.AddListTo<T>(converter); return this;
        }

        public IModelEntityConverterBuilder<TEntity> AddListToFunc<T>(Func<IEnumerable<TEntity>, T> convFunc) where T : class
        {
            convertObj.AddListToFunc<T>(convFunc); return this;
        }

        public IModelEntityConverterBuilder<TEntity> AddTo<T>(IModelEntityToConverter<T, TEntity> converter)
        {
            convertObj.AddTo<T>(converter); return this;
        }

        public IModelEntityConverterBuilder<TEntity> AddToFunc<T>(Func<TEntity, T> convFunc) where T : class
        {
            convertObj.AddToFunc<T>(convFunc); return this;
        }
        #endregion

        public IModelEntityConverter<TEntity> Build()
        {
            return convertObj;
        }
    }
    #endregion

}
