using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    internal class ModuleInitializer
    {
        private const string EmbeddedResourceNamespace = "Lib";

        public static void Initialize()
        {
            
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }

        private static Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
           
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);

            if (assembly != null)
                return assembly;

            var resourceName = GetResourceName(args.Name);
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                

                if (stream != null)
                {
                    
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);

                   
                    return Assembly.Load(buffer);
                }
            }
            return null;
        }

        private static string GetResourceName(string assemblyName)
        {
            var name = new AssemblyName(assemblyName).Name;
            return $"{EmbeddedResourceNamespace}.{name}.dll";
        }
    }
}
