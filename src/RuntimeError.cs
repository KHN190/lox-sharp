using System;

namespace lox
{
    public class RuntimeError : SystemException
    {
        internal readonly Token token;

        public RuntimeError(Token token, string msg) : base(msg)
        {
            this.token = token;
        }
    }
}