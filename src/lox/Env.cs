using System.Collections.Generic;

namespace lox
{
    public class EnvLox
    {
        /* The class stores variables by scope */

        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        private readonly EnvLox enclosing;

        public EnvLox()
        {
            enclosing = null;
        }

        public EnvLox(EnvLox enclosing)
        {
            this.enclosing = enclosing;
        }



        #region Methods

        internal void Define(string name, object value)
        {
            // used for native function definitions
            // probably need to allow override of native funcs
            //  e.g.
            //   fun clock() { return "hello!"; }
            //
            // but you can't do it now.

            if (!values.ContainsKey(name))
            {
                values[name] = value;
                return;
            }
            Token token = new Token(TokenType.IDENTIFIER, name, null, 0);

            throw new RuntimeError(token, "is already defined.");
        }

        internal void Define(Token name, object value)
        {
            // define functions and variables

            if (!values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            throw new RuntimeError(name, "[line " + name.line + "] is already defined.");
        }

        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            // if the variable isn’t in this environment, it checks the outer one
            if (enclosing != null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, "[line " + name.line + "] Undefined variable: " + name.lexeme + ".");
        }

        internal object Get(Token name)
        {
            if (values.ContainsKey(name.lexeme)) return values[name.lexeme];

            // If the variable isn’t found in this scope, we simply try the enclosing one
            if (enclosing != null) return enclosing.Get(name);

            throw new RuntimeError(name, "[line " + name.line + "] Undefined variable: " + name.lexeme + ".");
        }
        #endregion
    }
}