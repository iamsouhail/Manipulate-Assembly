
using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace Injector
{
    class Program
    {

        public static MethodInfo getMethodCall()
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(@"C:\Users\Souhail\Documents\github\Intern\Lib\bin\Debug\Lib.dll");
                var stream = assembly.GetManifestResourceStream("Lib.Logger.dll");
                long taille = stream.Length;
                Byte[] arr = new byte[taille];
                stream.Read(arr, 0, (int)taille);
                Assembly embededAssembly = Assembly.Load(arr);

                foreach (var type in embededAssembly.GetTypes())
                {
                    if (type.Name == "TheLogger")
                        foreach (var method in type.GetMethods())
                        {
                            if (method.Name == "log")
                                return method;
                        }
                }
              

                
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static void addCall(AssemblyDefinition lib)
        {
            foreach (var moduleDefinition in lib.Modules)
            {
                foreach (var moduleDefinitionType in moduleDefinition.Types)
                {
                    if(moduleDefinitionType.Name == "<Module>" || moduleDefinitionType.Name == "ModuleInitializer") continue;          
                    foreach (var methodDefinition in moduleDefinitionType.Methods)
                    {
                        var ilProcess = methodDefinition.Body.GetILProcessor();
                        methodDefinition.Body.Instructions.Insert(0, ilProcess.Create(OpCodes.Nop));
                        methodDefinition.Body.Instructions.Insert(0, ilProcess.Create(OpCodes.Call, lib.MainModule.ImportReference(getMethodCall())));
                        methodDefinition.Body.Instructions.Insert(0,ilProcess.Create(OpCodes.Ldstr, moduleDefinitionType+" --> "+methodDefinition.Name));
                        
                        

                    }
                }
            }
        }



        public static MethodReference getInititializer(AssemblyDefinition lib)
        {
            try
            {
                foreach (var moduleDefinition in lib.Modules)
                {
                    foreach (var moduleDefinitionType in moduleDefinition.Types)
                    {
                        if (moduleDefinitionType.Name == "ModuleInitializer") 
                        foreach (var methodDefinition in moduleDefinitionType.Methods)
                        {
                            if (methodDefinition.Name == "Initialize")
                                return methodDefinition;

                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }





        public static void Inject(AssemblyDefinition lib)
        {
            const MethodAttributes Attributes = MethodAttributes.Static | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var initializer = getInititializer(lib);
            if (initializer == null)
            {
                throw new ArgumentNullException(nameof(initializer));
            }
            var initializerReturnType = lib.MainModule.Import(initializer.ReturnType);
            // Create a new method .cctor (a static constructor) inside the Assembly  
            var cctor = new MethodDefinition(".cctor", Attributes, initializerReturnType);
            var il = cctor.Body.GetILProcessor();
            il.Append(il.Create(OpCodes.Call, initializer));
            il.Append(il.Create(OpCodes.Ret));

            var moduleClass = lib.MainModule.Types.FirstOrDefault(t => t.Name == "<Module>");
            moduleClass.Methods.Add(cctor);
        }
        static void Main(string[] args)
        {
            AssemblyDefinition lib = AssemblyDefinition.ReadAssembly(@"C:\Users\Souhail\Documents\github\Intern\Lib\bin\Debug\Lib.dll");
            addCall(lib);
            Inject(lib);
            lib.Write(@"C:\Users\Souhail\Documents\github\Intern\ans\lib.dll");
        }
    }
}
