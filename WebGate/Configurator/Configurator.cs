using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace WebGate.Configurator
{
    public interface IConfigProvider
    {
        //this[string name]  { get; }
    }

    public interface IConfigDescriptor
    {
        string Name { get; }
        Dictionary<string, string> Items { get;}
    }

    public class ConfigItemDescriptor
    {
        public Type   Type        { get; set; }
        public object Value       { get; set; }
        public String Caption     { get; set; }
        public String Description { get; set; }

    }

    /// <summary>
    /// ConfigNode
    /// </summary>
    public class ConfigNode : IConfigProvider, IConfigDescriptor
    {
        public string Name { get; }
        public Dictionary<string, string> Items => throw new NotImplementedException();

        private Dictionary<string, ConfigItemDescriptor> items = new Dictionary<string, ConfigItemDescriptor>();

        public ConfigNode(string name, Dictionary<string, ConfigItemDescriptor> _items)
        {
            items = _items;
        }

        public ConfigNode(string name, Dictionary<string, object> _items)
        {
            Name = name;
            foreach ( var i in  _items) 
            {
                items.Add(i.Key, new ConfigItemDescriptor() { Value = i.Value, Type = i.GetType() });
            }
        }
    }


    /// <summary>
    /// Singletone 
    /// </summary>
    public class Configurator
    {
        private static Configurator instance;

        private Configurator()
        {
            var configurationBuilder = new ConfigurationBuilder();
            //var builder = new ConfigurationBuilder()
            //    .AddInMemoryCollection(DefaultConfigurationStrings)
            //    .AddJsonFile("Config.json",
            //    true)
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json");
        }

        public static Configurator Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configurator();
                }
                return instance;
            }
        }
    }

}
