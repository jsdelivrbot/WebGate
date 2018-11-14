

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using WebGate.Services.Data;
using WebGate.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

namespace WebGate.Services
{
    /// <summary>
    /// Запуск DI сервисов приложения
    /// </summary>
    public class ServiceBootStrapper : IServiceBootStrapper
    {
        public const string AxaptaKey = @"AxaptaConnectionString";
        public const string AxExtKey  = @"AxExtConnectionString";
        public const string ConStrSec = @"ConnectionString";

        public IConfigurationRoot Configuration { get; set; }

        public ServiceBootStrapper(IConfigurationRoot configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddOptions();

            services.AddDbContextPool<AxaptaContext>(
                    options => options.UseSqlServer(
                            Configuration.GetSection(ConStrSec).GetValue<string>( AxaptaKey, "Data Source=SQLROOT;Initial Catalog=Ax2009_MainDevelopment_16.05.2017;Integrated Security=True")
                            )
                        );

            services.AddDbContextPool<AxExtentionContext>(
                    options => options.UseSqlServer(
                            Configuration.GetSection(ConStrSec).GetValue<string>(AxExtKey, "Data Source=SQLROOT;Initial Catalog=NVA_AX_Extension;Integrated Security=True")
                            )
                        );

            //services.AddDbContext<AxaptaContext>(options => options.UseSqlServer(Configuration.GetConnectionString(AxaptaKey)));

            //services.Configure<DbContextOptions<AxExtentionContext>>(Configuration);
            //Configuration.GetConnectionString();
            //services.AddDbContext<AxExtentionContext>(options => options.UseSqlServer("eeee"));
            //services.Configure<DbContextOptions<AxExtentionContext>>
            //services.Configure<DbContextOptions<AxExtentionContext>>(opt =>
            //    {
            //        var e = new SqlServerOptionsExtension();
            //        opt.Extensions.Append<SqlServerOptionsExtension>(e);
            //    }
            //);
            //       new DbContextOptionsBuilder().UseSqlServer("eeeeeeeeeeeeee").Options
            //       );

            //myOptions =>
            //            //myOptions.FindExtension<SqlServerOptionsExtension>().ConnectionString = "eeee" ;

            //            );
            //services.AddDbContext<BloggingContext>(options => options.UseSqlite("Data Source=blog.db"));
            //services.AddOptions();
            //services.Configure<DalContextProviderOptions>(Configuration);
            //services.Configure<DalContextProviderOptions>(myOptions =>
            //{
            //    myOptions.AxaptaConnectionString        = "value1_from_action";
            //    myOptions.AxExtentionConnectionString   = "value1_from_action";
            //});
            //services.AddSingleton<IDalContextProvider, DalContextProvider>();
        }

        //public object ConfigValue()
        //{

        //}            


    }
}
