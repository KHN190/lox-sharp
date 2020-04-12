using System;
using System.IO;
using System.Collections.Generic;

namespace lox
{
    class Lox
    {
        private static bool hadError;
        private static bool hadRuntimeError;

        private static readonly Interpreter interpreter = new Interpreter();

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: lox [script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
                RunFile(args[0]);
            else
                RunPrompt();
        }

        /**
		* Run Script / Prompt
		*/
        static void RunFile(string script)
        {
            string path = Path.GetFullPath(script);

            Run(File.ReadAllText(path));

            if (hadError)
                Environment.Exit(65);
            if (hadRuntimeError)
                Environment.Exit(70);
        }

        static void RunPrompt()
        {
            string line;

            Console.Write("> ");

            while ((line = Console.ReadLine()) != null)
            {
                try
                {
                    Run(line);
                }
                catch (SystemException)
                {
                    // stop execution, but no interruption in REPL
                }
                Console.Write("> ");

                hadError = false;
            }
            Console.WriteLine();
        }

        static void Run(string source)
        {
            // scan
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            // parse
            Parser parser = new Parser(tokens);
            List<Stmt> stmts = parser.Parse();

            // print
            //Expr expression = parser.Expression();
            //Console.WriteLine(new AstPrinter().Print(expression));

            // interpret
            interpreter.Interpret(stmts);
        }

        /**
		* Error Handler
		*/
        public static void Error(int line, string msg)
        {
            Report(line, "", msg);
        }

        public static void Error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                Report(token.line, " at end.", message);
            }
            else
            {
                Report(token.line, " at '" + token.lexeme + "'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.WriteLine(error.Message + "\n[line " + error.token.line + "]");
            hadRuntimeError = true;
        }

        static void Report(int line, string where, string msg)
        {
            Console.Error.WriteLine("[line {0}] Error{1}: {2}", line, where, msg);
            hadError = true;
        }
    }
}
