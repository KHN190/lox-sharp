using System.Collections.Generic;

namespace lox
{

    public interface ILoxCallable
    {
        internal int Arity();

        internal object Call(Interpreter interpreter, List<object> arguments);
    }
}
