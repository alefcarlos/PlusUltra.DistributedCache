using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace PlusUltra.DistributedCache
{
    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration, bool addHealthCheck = true)
        {
            var uri = configuration.GetConnectionString("Redis");
			var prefix = configuration.GetSection("RedisConfig:Prefix").Value;

			var options = ConfigurationOptions.Parse(uri);

			if (!options.DefaultDatabase.HasValue)
				throw new ArgumentNullException("DefaultDatabase", "É obrigatório informar o database padrão do Redis.");

			services.AddStackExchangeRedisCache(config =>
			{
				config.ConfigurationOptions = options;
				config.InstanceName = string.IsNullOrWhiteSpace(prefix) ? null : $"{prefix}:";
			});

			//Adicionando healthcheck
			if (addHealthCheck)
			{
				services.AddHealthChecks()
					.AddRedis(uri, "redis", tags: new string[] { "db", "redis", "cache" });
			}

            return services;
        }
    }
}