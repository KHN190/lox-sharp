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
				writer.WriteLine();

				writer.WriteLine("namespace lox\n{");
				writer.WriteLine("\tpublic abstract class " + baseName + "\n\t{");

				// visitor
				DefineVisitor(writer, baseName, types);

				// types
				foreach (string type in types)
				{
					string className = type.Split(":")[0].Trim();
					string fields = type.Split(":")[1].Trim();

					DefineType(writer, baseName, className, fields);
				}

				// accept
				writer.WriteLine("\n\t\tinternal abstract R Accept<R>(Visitor<R> visitor);");

				writer.WriteLine("\t}");
				writer.WriteLine("}");
			}
		}

		private static void DefineType(StreamWriter writer, string baseName, string className, string fields)
		{
			writer.WriteLine("\n\t\tpublic class " + className + " : " + baseName + "\n\t\t{");

			// Constructor
			writer.WriteLine("\t\t\tpublic " + className + "(" + fields + ")\n\t\t\t{");

			// Assign attrs
			foreach (string field in fields.Split(", "))
			{
				string name = field.Split(" ")[1];
				writer.WriteLine("\t\t\t\tthis." + name + " = " + name + ";");
			}
			writer.WriteLine("\t\t\t}\n");

			// Visitor
			writer.WriteLine("\t\t\tinternal override R Accept<R>(Visitor<R> visitor)\n\t\t\t{");
			writer.WriteLine("\t\t\t\treturn visitor.Visit" + className + baseName + "<R>(this);");
			writer.WriteLine("\t\t\t}\n");

			// Fields
			foreach (string field in fields.Split(", "))
			{
				writer.WriteLine("\t\t\tpublic readonly " + field + ";");
			}
			writer.WriteLine("\t\t}");
		}

		private static void DefineVisitor(StreamWriter writer, string baseName, string[] types)
		{
			writer.WriteLine("\t\tpublic interface Visitor<R>\n\t\t{");

			foreach (string type in types)
			{
				string typeName = type.Split(":")[0].Trim();
				writer.WriteLine("\t\t\tR Visit" + typeName + baseName + "<T>(" + typeName + " " + baseName.ToLower() + ");");
			}
			writer.WriteLine("\t\t}");
		}
	}
}
