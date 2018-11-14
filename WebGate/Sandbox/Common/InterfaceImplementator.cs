using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Common
{
    public interface IinterfaceImplementator
    {
        Type CreateType(Type interfaceType, Type dynamicProxyBaseType);
    }

    public class InterfaceImplementator
    {
    }

}
