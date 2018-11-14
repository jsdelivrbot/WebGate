using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Models
{

    /// <summary>
    /// Творение T из ничего
    /// </summary>
    public interface ICreator<T>
    {
        T Create();
    }

    /// <summary>
    /// Создание T из Tpar 
    /// </summary>
    public interface IBuilder<Tpar, T>
    {
        T Build(Tpar parameters);
    }


    public interface IMFactory<I> 
    {
        I Create();
    }


    public class Factory<I> : IMFactory<I>
    {
        protected Func<I> _buildFunction;

        public Factory(Func<I> buildFunction)
        {
            _buildFunction = buildFunction;
        }

        public I Create()
        {
            return _buildFunction();
        }
    }

    


}
