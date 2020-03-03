using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using HealthChecks.UI.Client;
using ControleHorasApi.Config;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

namespace ControleHorasApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore().AddApiExplorer();
            var dadosDependencias = new List<Dependency>();
            new ConfigureFromConfigurationOptions<List<Dependency>>(
                Configuration.GetSection("Dependencies"))
                    .Configure(dadosDependencias);
            dadosDependencias = dadosDependencias.OrderBy(d => d.Name).ToList();

            // Verificando a disponibilidade dos bancos de dados
            // da aplicação através de Health Checks
            // Adicionar o método para vericar o healthcheck da dependencia a ser testada
            // Exemplo: Mysql, SQLServer, MongoDB, Rabbit - Verificar qual o método correto           
            services.AddHealthChecks()
                .AddDependencies(dadosDependencias);

            services.AddHealthChecksUI();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ControleHorasAPI",
                    Version = "v1"
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //configurando o dashboard de healthcheck
            app.UseHealthChecksUI(opt =>
            {
                opt.UIPath = "/dashbord";
            });
            app.UseStaticFiles();
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ControleHorasAPI");
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // configurando o endpoint Raiz para teste da aplicação, pode ser retirado futuramente
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("API do controle de horas está funcionando");
                });
                // configurando o endpoint de healthCheck
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapControllers();
            });
        }
    }
}
