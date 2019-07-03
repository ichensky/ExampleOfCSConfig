using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace ExampleOfCSConfig
{
    class Program
    {
        private static Config LoadConfig(string path)
        {

            var configCode = File.ReadAllText(path);
            var syntaxTree = CSharpSyntaxTree.ParseText(configCode);

            var compilation = CSharpCompilation.Create(
                nameof(ExampleOfCSConfig),
                new[] { syntaxTree },
                new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var dllStream = new MemoryStream();
            var pdbStream = new MemoryStream();
            var emitResult = compilation.Emit(dllStream, pdbStream);
            if (!emitResult.Success)
            {
                // emitResult.Diagnostics
            }
            dllStream.Position = 0;
            byte[] data = new byte[dllStream.Length];
            dllStream.Read(data, 0, data.Length);
            var assembly = Assembly.Load(data);
            var type = assembly.GetType($"{nameof(ExampleOfCSConfig)}.{nameof(ExampleOfCSConfig.Config)}");
            var config = Activator.CreateInstance(type);


            var s = new BinaryFormatter();
            using var ms = new MemoryStream();
            s.Serialize(ms, config);
            ms.Position = 0;
            var c = (Config)s.Deserialize(ms);
            return c;
        }


        static async Task Main(string[] args)
        {
            var config = LoadConfig("Config.cs");
            Console.WriteLine($"service url: {config.ServiceUrl}");
        }
    }
}
