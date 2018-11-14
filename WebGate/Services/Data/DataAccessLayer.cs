using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebGate.Models;

namespace WebGate.Services.Data
{

    #region DalContextProvider
    /// <summary>
    ///  Сервис для управления соеденением с базой и раздачей контестов данных
    /// </summary>
    public interface IDalContextProvider
    {
        T GetNamedContext<T>() where T : DbContext; 
    }    

    public class DalContextProviderOptions
    {
        public string AxExtentionConnectionString { get; set; }
        public string AxaptaConnectionString      { get; set; }
    }

    public class DalContextProvider : IDalContextProvider 
    {
        //DbContext _context ;
        DalContextProviderOptions _options;
        Dictionary<Type, string>  _bind = new Dictionary<Type, string>() ;       

        public DalContextProvider(IOptions<DalContextProviderOptions> optionsAccessor)
        {
            _options = optionsAccessor.Value;

            // сопоставление 
            _bind.Add(typeof(AxaptaContext)      , _options.AxaptaConnectionString);
            _bind.Add(typeof(AxExtentionContext) , _options.AxExtentionConnectionString);
        }

        T IDalContextProvider.GetNamedContext<T>()
        {
            T ret  = null;
            if (_bind.ContainsKey(typeof(T))) // Хули ? рефлекшн. 
            {
                var tp = typeof(T);
                var cnstr = tp.GetConstructor(new Type[] { typeof(DbContextOptions) });
                if (cnstr != null)
                {
                    var opt = new DbContextOptionsBuilder().UseSqlServer(_bind[typeof(T)]).Options;
                    var sret = cnstr.Invoke( new object[]{  opt }  );
                }
            }
            return ret;
        }

    }
    #endregion DalContextProvider

    #region DalQuery
    public class DalQuery 
    {
        DbContext           _context  = null;
        IDalContextProvider _provider;

        //bool IsOk  => (_context != null && _context.

        public DalQuery(IDalContextProvider provider )
        {
            _provider = provider;
        }

        public void SetContext<T>() where T : DbContext
        {
            _context = _provider.GetNamedContext<T>();
        }

        public int ExecuteSqlCommand(string query)
        {

            //_context.Database.

            return _context.Database.ExecuteSqlCommand(query);
        }
    }
    #endregion DalQuery

}
