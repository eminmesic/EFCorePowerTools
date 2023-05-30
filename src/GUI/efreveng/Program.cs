﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using RevEng.Common;
using RevEng.Core;

[assembly: CLSCompliant(true)]

namespace EfReveng
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;

                if (args == null)
                {
                    throw new ArgumentNullException(nameof(args));
                }

                if (args.Length > 0)
                {
                    if ((args.Length == 3 || args.Length == 4)
                        && int.TryParse(args[1], out int dbTypeInt)
                        && bool.TryParse(args[0], out bool mergeDacpacs))
                    {
                        SchemaInfo[] schemas = null;
                        if (args.Length == 4)
                        {
                            schemas = args[3].Split(',').Select(s => new SchemaInfo { Name = s }).ToArray();
                        }

                        var builder = new TableListBuilder(dbTypeInt, args[2], schemas, mergeDacpacs);

                        var buildResult = builder.GetTableModels();

                        buildResult.AddRange(builder.GetProcedures());

                        buildResult.AddRange(builder.GetFunctions());

                        Console.Out.WriteLine("Result:");
                        Console.Out.WriteLine(buildResult.Write());

                        return 0;
                    }

                    if (!File.Exists(args[0]))
                    {
                        Console.Out.WriteLine("Error:");
                        Console.Out.WriteLine($"Could not open options file: {args[0]}");
                        return 1;
                    }

                    var options = ReverseEngineerOptionsExtensions.TryDeserialize(File.ReadAllText(args[0], System.Text.Encoding.UTF8));
                    options.DatabaseType = DatabaseType.SQLServer;
                    options.ProjectPath = @"C:\Repos\Hidaya\backend\src\Platform.EntityFramework";
                    //options.Dacpac = @"C:\Repos\Hidaya\database\src\bin\Debug\Hidaya.Database.dacpac";
                    options.ConnectionString = "Data Source=.;Initial Catalog=NewHidayaDb;Integrated Security=True;";

                    if (options == null)
                    {
                        Console.Out.WriteLine("Error:");
                        Console.Out.WriteLine("Could not read options");
                        return 1;
                    }

                    var result = ReverseEngineerRunner.GenerateFiles(options);

                    Console.Out.WriteLine("Result:");
                    Console.Out.WriteLine(result.Write());
                }
                else
                {
                    Console.Out.WriteLine("Error:");
                    Console.Out.WriteLine("Invalid command line");
                    return 1;
                }

                return 0;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Console.Out.WriteLine("Error:");
                Console.Out.WriteLine(ex.Demystify());
                return 1;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
