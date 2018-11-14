using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WebGate.Models;
using WebGate.Bus;
using WebGate.Services;
using Microsoft.EntityFrameworkCore;
using WebGate.Models.Logic;
using WebGate.Models.DAL;

namespace WebGate
{
    public class Startup
    {
        public const string AxaptaKey = @"AxaptaConnectionString";
        public const string AxExtKey  = @"AxExtConnectionString";
        public const string ConStrSec = @"ConnectionString";

        public IConfiguration   Configuration { get; set; }  //Root
        public ILogger          Logger        { get; set; }
        public ILoggerFactory   LoggerFactory { get; set; }

        public Startup(IHostingEnvironment env, IConfiguration cfg , ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
            Configuration = cfg;
            loggerFactory.AddEventSourceLogger();

            Logger = loggerFactory.CreateLogger<Program>();
            Logger.LogInformation("LogInformation...");
            Logger.LogWarning("LogWarning...");
            Logger.LogError("LogError");

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger>(svc => Logger);

            services.AddSingleton(typeof(DbContextOptions<AxaptaContext>),
                new DbContextOptionsBuilder<AxaptaContext>()
                            .UseSqlServer(Configuration.GetSection(ConStrSec).GetValue<string>(AxaptaKey, "Data Source=SQLROOT;Initial Catalog=Ax2009_WORK_RU7;User ID=ssrsaccess;Password=passssrsaccess;")
                            ).Options
                 );

            services.AddSingleton(typeof(DbContextOptions<AxExtentionContext>),
                new DbContextOptionsBuilder<AxExtentionContext>()
                            .UseSqlServer(Configuration.GetSection(ConStrSec).GetValue<string>(AxaptaKey, "Data Source=SQLROOT;Initial Catalog=NVA_AX_Extension;User ID=sa;Password=bug;")
                            ).Options
                 );

            services.AddDbContext<AxaptaContext>();
            services.AddDbContext<AxExtentionContext>();

            services.AddMvc();
            services.AddTransient<IAxExtService,    AxExtService>();
            services.AddTransient<IAxFlightService, AxFlightService>();

            services.AddTransient<IFlightBoard, FlightBoard>();


            //services.AddSingleton<IDalAccessBuilderFactory>(new DalAccessBuilderFactory());
            //services.AddSingleton<IMFactory<IDalProvAccessBuilder> >( new  DalProvAccessBuilderFactory < TEntity, TSource >
            services.AddSingleton<IDalProvAccessBuilderFactory>(new DalProvAccessBuilderFactory());

            services.AddSingleton<IModelEntityHelperBuilderFactory>(new ModelEntityHelperBuilderFactory()); 


            Logger.LogInformation("Servises was started...");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
