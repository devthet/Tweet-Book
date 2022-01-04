using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tweet_Book.Data;
using Tweet_Book.HealthChecks;

namespace Tweet_Book.Installers
{
    public class HealthChecksInstaller : IInstaller
    {
        public void InstallerServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>()
                .AddCheck<RedisHealthCheck>("Redis");
               
        }
    }
}
