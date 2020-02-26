using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ControleHorasApi.Config
{
    public static class DependenciesExtensions
    {
        public static IHealthChecksBuilder AddDependencies(
            this IHealthChecksBuilder builder,
            List<Dependency> dependencies)
        {
            foreach (var dependencia in dependencies)
            {
                string nomeDependencia = dependencia.Name.ToLower();
                if (nomeDependencia.StartsWith("mysql-"))
                {
                    builder = builder.AddMySql(dependencia.ConnectionString, name: dependencia.Name);
                }
            }

            return builder;
        }
    }
}
