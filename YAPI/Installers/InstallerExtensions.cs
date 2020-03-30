using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YAPI.Installers
{
    public static class InstallerExtensions
    {
        public static void installServicesInAssembly(this IServiceCollection services, IConfiguration configuration)
        {
            //get all classes that implements IInstaller and create instances of them then cast hem to IInstaller / noice
            var installers = typeof(Startup).Assembly.ExportedTypes.Where(x =>
                  typeof(IInstaller).IsAssignableFrom(x) && !x.IsAbstract && !x.IsAbstract).Select(Activator.CreateInstance).Cast<IInstaller>().ToList();

            //here we have installing all our services using installer classes installService method / this one also is noice
            installers.ForEach(installer => installer.InstallServices(services, Configuration));
        }
    }
}
