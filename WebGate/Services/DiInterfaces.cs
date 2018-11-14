using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Services
{
    #region IServiceBootStrapper
    public interface IServiceBootStrapper
    {
        void ConfigureServices(IServiceCollection services);
    }
    #endregion IServiceBootStrapper

    #region IConfigurationManager tutoral
    public interface IConfigurationManager 
    {
        string this[string key] { get; set; }
        object Get   (string name, Type type ,  object instance);
        T      Get<T>(string name,              object instance);
        void   Set<T>(T val, string name,       object instance);
    }
    #endregion IConfigurationManager

}
