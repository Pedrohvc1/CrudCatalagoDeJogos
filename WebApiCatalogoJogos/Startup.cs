using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiCatalogoJogos.Controllers.V1;
using WebApiCatalogoJogos.Middleware;
using WebApiCatalogoJogos.Repositories;
using WebApiCatalogoJogos.Services;

namespace WebApiCatalogoJogos
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IJogoService, JogoService>();
            services.AddScoped<IJogoRepository, JogoSqlServerRepository>();

            #region CicloDeVida
            services.AddSingleton<IExemploSingleton, ExemploCicloDeVida>();
            services.AddScoped<IExemploScoped, ExemploCicloDeVida>();
            services.AddTransient<IExemploTransient, ExemploCicloDeVida>();
            #endregion


            services.AddControllers(); // AddControllers() - quando � Api, se fosse mvc - AddControllersWithViews()           
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiCatalogoJogos", Version = "v1" });

                var basePath = AppDomain.CurrentDomain.BaseDirectory;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                c.IncludeXmlComments(Path.Combine(basePath, fileName));

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiCatalogoJogos v1"));
            }

            app.UseMiddleware<ExceptionMiddleware>(); //exceptions

            app.UseHttpsRedirection();
                
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); //mapeia os endpoints
            });
        }
    }
}