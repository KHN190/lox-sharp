using System.Collections.Generic;

namespace lox
{
    public class EnvLox
    {
        /* The class stores variables by scope */

        private readonly Dictionary<string, object> values = new Dictionary<string, object>();

        internal void Define(string name, object value)
        {
            values[name] = value;
        }

        internal void Assign(Token name, object value)
        {
            if (values.ContainsKey(name.lexeme))
            {
                values[name.lexeme] = value;
                return;
            }

            throw new RuntimeError(name, "[line " + name.line + "] Undefined variable: " + name.lexeme + ".");
        }

        internal object Get(Token name)
        {
            if (values.ContainsKey(name.lexeme)) return values[name.lexeme];

            throw new RuntimeError(name, "[line " + name.line + "] Undefined variable: " + name.lexeme + ".");
        }
    }
}