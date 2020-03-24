using System;
using System.IO;

// Auto generate Expr & Stmt classes
namespace lox.tools
{
	public class GenerateAst {

		public static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.Error.WriteLine("Usage: generate_ast <output directory>");
				Environment.Exit(1);
			}
			string outputDir = args[0];

			DefineAst(outputDir, "Expr", new string[] {
				"Binary   : Expr left, Token op, Expr right",
				"Grouping : Expr expression",
				"Literal  : Object value",
				"Unary    : Token op, Expr right"
			});
		}

		private static void DefineAst(string outputDir, string baseName, string[] types)
		{
			string path = outputDir + "/" + baseName + ".cs";

			using (StreamWriter writer = new StreamWriter(path))
			{
				writer.WriteLine("using System;");
				writer.WriteLine("using System.Collections.Generic;");
				writer.WriteLine();

				writer.WriteLine("namespace lox\n{");
				writer.WriteLine("\tabstract class " + baseName + "\n\t{");

				// types
				foreach (string type in types)
				{
					string className = type.Split(":")[0].Trim();
					string fields = type.Split(":")[1].Trim();

					DefineType(writer, baseName, className, fields);
				}

				writer.WriteLine("\t}");
				writer.WriteLine("}");
			}
		}

		private static void DefineType(StreamWriter writer, string baseName, string className, string fields)
		{
			writer.WriteLine("\n\t\tpublic class " + className + " : " + baseName + "\n\t\t{");

			// Constructor
			writer.WriteLine("\t\t\t" + className + "(" + fields + ")\n\t\t\t{");

			// Assign attrs
			foreach (string field in fields.Split(", "))
			{
				string name = field.Split(" ")[1];
				writer.WriteLine("\t\t\t\tthis." + name + " = " + name + ";");
			}
			writer.WriteLine("\t\t\t}\n");

			// Fields
			foreach (string field in fields.Split(", "))
			{
				writer.WriteLine("\t\t\treadonly " + field + ";");
			}
			writer.WriteLine("\t\t}");
		}
	}
}
