using System.Collections.Generic;
using System;

namespace lox
{
    class Return : Exception
    {
        internal readonly object value;

        public Return(object value)
        {
            this.value = value;
        }
    }


    #region Native Functions

    public class Clock : ILoxCallable
    {
        int ILoxCallable.Arity()
        {
            return 0;
        }

        object ILoxCallable.Call(Interpreter interpreter, List<object> arguments)
        {
            return (double) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000.0;
        }

        public override string ToString()
        {
            return "<native fn: clock>";
        }
    }
    #endregion



    #region Functions

    public class LoxFunction : ILoxCallable
    {
        private readonly Stmt.Function declaration;
        private readonly EnvLox closure;

        public LoxFunction(Stmt.Function declaration)
        {
            this.closure = null;
            this.declaration = declaration;
        }

        public LoxFunction(Stmt.Function declaration, EnvLox closure)
        {
            this.closure = closure;
            this.declaration = declaration;
        }

        int ILoxCallable.Arity()
        {
            return declaration.parameters.Count;
        }

        object ILoxCallable.Call(Interpreter interpreter, List<object> arguments)
        {
            EnvLox env = new EnvLox(closure);

            for (int i = 0; i < declaration.parameters.Count; i++)
            {
                env.Define(declaration.parameters[i], arguments[i]);
            }

            object value;
            try
            {
                value = interpreter.ExecuteBlock(declaration.body, env);
            }
            catch (Return returnValue)
            {
                value = returnValue.value;
            }
            return value;
        }

        public override string ToString()
        {
            return "<fn: " + declaration.name.lexeme + ">";
        }
    }

    #endregion
}