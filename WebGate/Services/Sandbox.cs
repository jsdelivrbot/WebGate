using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Services
{
    #region ConfigurationManager  tutoral

    public class ConfigurationManager : IConfigurationManager
    {
        IConfiguration _configuration;

        public const string rootName = @"App";
        public const char devider = ':';

        public string this[string key] { get => _configuration[key]; set => _configuration[key] = value; }

        public ConfigurationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public object Get(string name, Type type, object instance)
        {
            return _configuration.GetValue(type, BuildKey(name, instance));
        }

        public T Get<T>(string name, object instance = null)
        {
            return _configuration.GetValue<T>(BuildKey(name, instance));
        }

        public void Set<T>(T val, string name, object instance)
        {
            _configuration[BuildKey(name, instance)] = val.ToString();
        }

        private string BuildKey(string name, object instance = null)
        {
            return InstanceToName(instance) + devider.ToString() + name;
        }

        private string InstanceToName(object instance = null)
        {
            var ret = rootName;
            if (instance != null)
            {
                if (instance is string s)
                {
                    ret += devider.ToString() + s;
                }
                else
                {
                    ret += devider.ToString() + (instance.GetType().ToString());
                }
            }
            return ret;
        }
    }
    #endregion ConfigurationManager

}
