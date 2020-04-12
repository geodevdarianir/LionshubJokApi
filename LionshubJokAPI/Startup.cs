using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LionshubJokAPI.Models;
using LionshubJokAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LionshubJokAPI
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
            services.Configure<Settings>(options =>
            {
                options.ConnectionString
                    = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database
                    = Configuration.GetSection("MongoConnection:Database").Value;
                
            });

           // ყოველთვის ერთი ობიექტი
            services.AddScoped<ITableService,TableService>();
            services.AddScoped<IGamerService,GamerService>();
            services.AddScoped<JokerService>();

            //p => new JokerService(p.GetService<ITableService>(), p.GetService<IGamerService>())
            // სულ ახალ ობიექტს გაძლევს
            //services.AddTransient<ITableService,IGamerService, JokerService>();
            //services.AddTransient<IGamerService, GamerService>();
            //services.AddTransient<ITableService, TableService>();
            
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
