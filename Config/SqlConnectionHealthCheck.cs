using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace ControleHorasApi.Config
{
    public class SqlConnectionHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            // TODO Classe para configuração da verificação de healthcheck do banco de dados
            // Implementar após definir o SGBD a ser utilizado
            return HealthCheckResult.Healthy();
        }
    }
}