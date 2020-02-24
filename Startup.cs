using ControleHorasApi.Config;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ControleHorasApi
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Adicionar o método para vericar o healthcheck da dependencia a ser testada
            // Exemplo: Mysql, SQLServer, MongoDB, Rabbit - Verificar qual o método correto
            services.AddHealthChecks();
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
                opt.UIPath = "/status-dashbord";
            });
        }
    }
}
