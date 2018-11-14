using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGate.Models
{
    #region AirCraftType
    public class AirCraftType
    {
        public AirCraftType()
        {

        }

    }
    #endregion AirCraftType

    #region ContextOptionHelper
    public interface IContextOptionService
    {
        DbContextOptions Options { get; }
        DbContextOptions getOptions<T>();
    }

    public class ContextOptionService : IContextOptionService
    {
        IConfiguration _configuration;

        public ContextOptionService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbContextOptions Options => getOptions();

        public DbContextOptions getOptions<T>()
        {
            throw new NotImplementedException();
        }

        private DbContextOptions getOptions()
        {
            //var t = typeof(T) ;
            var b = new DbContextOptionsBuilder()
            .UseSqlServer("")
            .Options;
            return b;
        }
    }
    #endregion AxaptaContextOptionBuilder
}
