using System;
using System.Collections.Generic;
using System.Text;

namespace WebGateApi.Base
{

    


    #region IBuilder<T>()
    public interface IBuilder<T>
    {
        T Build();
    }
    #endregion


    #region IUniFactory<I>
    /// <summary>
    /// Универсальная фабрика типа
    /// </summary>
    /// <typeparam name="I"></typeparam>
    public interface IUniFactory<I>
    {
        I Create();
    }
    #endregion


    #region IFactorySet
    /// <summary>
    /// Универсальная фабрика
    /// </summary>
    /// <typeparam name="I"></typeparam>
    public interface IUniFactorySet 
    {
        T Create<T>() where T : class ;
    }
    #endregion






}
