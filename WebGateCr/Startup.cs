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
using WebGateCr.Models;
using WebGateCr.Models.Data;
using Microsoft.EntityFrameworkCore;
using WebGateApi.Base;
using WebGateCr.Models.NvaSd;
using WebGateCr.Models.AxaptaModel.ATS;

namespace WebGateCr
{
    public class Startup
    {
        public const string AxaptaKey = @"AxaptaConnectionString";
        public const string AxExtKey = @"AxExtConnectionString";
        public const string ConStrSec = @"ConnectionString";

        public IConfiguration Configuration { get; set; }  //Root
        public ILogger Logger { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }

        public Startup(IHostingEnvironment env, IConfiguration cfg, ILoggerFactory loggerFactory)
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
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowSpecificOrigin",
            //        builder => builder.AllowAnyOrigin());
            //});

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

            services.AddCors();
            services.AddMvc();

            services.AddTransient<IFlightBoard, FlightBoard>();
            services.AddTransient<INvaSd,    NvaSd>();
            services.AddTransient<IAxCommon, AxCommon>();

            services.AddSingleton<IDalBuilderFactory>(new DalBuilderFactory());

            services.AddSingleton<IHttpErrorBuilder>(new HttpErrorBuilder());  // Error builder

            services.AddSingleton(new Resp112Helper());            // ATS 112 response helper

            services.AddSingleton(new DataContextHelper());        // DataContextHelper for controller

            services.AddSingleton(new EntytyMetadatasHelper());    // Metadata Helper for controller

            

            Logger.LogInformation("Servises was started...");

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(
                builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseMvc();
            

        }
    }
}
