using System;
using System.IO;
using System.Collections.Generic;

namespace lox
{
	class Lox
	{
		private static bool hadError = false;

		// static void Main(string[] args)
		// {
		// 	if (args.Length > 1)
		// 	{
		// 		Console.WriteLine("Usage: lox [script]");
		// 		Environment.Exit(64);
		// 	}
		// 	else if (args.Length == 1)
		// 		RunFile(args[0]);
		// 	else
		// 		RunPrompt();
		// }

		/**
		* Run Script / Prompt
		*/
		static void RunFile(string script) 
		{
			string path = Path.GetFullPath(script);
			Console.WriteLine("Load script from {0}", path);

			Run(File.ReadAllText(path));

			if (hadError)
				Environment.Exit(65);
		}

		static void RunPrompt() 
		{
			string line = "";

			Console.Write("> ");
			while ((line = Console.ReadLine()) != null)
			{
				Run(line);
				Console.Write("> ");

				hadError = false;
			}
		}

		static void Run(string source)
		{
			Scanner scanner = new Scanner(source); 
			List<Token> tokens = scanner.ScanTokens();

			// For now, print tokens
			foreach (Token tok in tokens)
				Console.WriteLine(tok);
		}

		/**
		* Error Handler
		*/
		public static void Error(int line, string msg)
		{
			Report(line, "", msg);
		}

		static void Report(int line, string where, string msg)
		{
			Console.Error.WriteLine("[line {0}] Error{1}: {2}", line, where, msg);
			hadError = true;
		}
	}
}
