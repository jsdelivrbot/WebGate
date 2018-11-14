using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Bus
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBus
    {
        /// Регистрирует обект с интерфейсами
        IResult Register(IAtom obj, List<Type> interfaces = null);
        string  Info    { get; }
    }

    public interface IBusClient
    {
        IAtom Name { get;}  
    }

    public interface IResult
    {
        bool IsError { get; }
        string Message { get; }
        IAtom Owner { get; }
    }

    public interface IResult<T> : IResult
    {
        T      Data    { get; } 
    }

    public interface IAtomDescriptor
    {
        IAtom Atom { get; }
    }

    public interface IAtom
    {
        object Id   { get;}
        string Name { get; }
        string Description { get; set; }
        //string ToString { get; }
    }

    
}
