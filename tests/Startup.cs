using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlusUltra.Testing;

namespace PlusUltra.DistributedCache.Tests
{
    public class Startup : TestStartup
    {
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
		{
			services.AddRedisCache(configuration, addHealthCheck: false);
		} 
    }
}