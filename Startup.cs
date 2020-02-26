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

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
            });
            app.UseHealthChecksUI(opt =>
            {
                opt.UIPath = "/dashbord";
            });
        }
    }
}
