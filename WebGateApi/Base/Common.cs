using System;
using System.Collections.Generic;
using System.Text;

namespace WebGateApi.Base
{
      
    #region simpleFactory

    public class UniFactory<I> : IUniFactory<I>
    {
        protected Func<I> _buildFunction;

        public UniFactory(Func<I> buildFunction)
        {
            _buildFunction = buildFunction;
        }

        public I Create()
        {
            return _buildFunction();
        }
    }


    public class UniFactorySet : IUniFactorySet     /// <summary>
    {
        public IDictionary<Type, object> _factorySet = new Dictionary<Type, object>();

        public T Create<T>() where T : class
        {
            T ret = null;
            if (_factorySet.ContainsKey(ret.GetType()))
            {
                var o = _factorySet[typeof(T)] as IUniFactory<T>;
                if (o != null)
                {
                    ret = o.Create();
                }
            }
            return ret;
        }

        public void Add<T>(IUniFactory<T> typeFactory)
        {
            _factorySet.Add(typeof(T), typeFactory);
        }

        public void Add<T>(Func<T> createFunc)
        {
            this.Add<T>(new UniFactory<T>(createFunc));
        }

    }



    #endregion

}
