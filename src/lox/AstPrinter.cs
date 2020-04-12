using System;

namespace lox
{
    public class AstPrinter : Expr.Visitor<string>
    {
        internal string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        string Parenthesize(string name, params Expr[] exprs)
        {
            string text = "(" + name;
            foreach (Expr expr in exprs)
            {
                text += " " + expr.Accept(this);
            }
            text += ")";
            return text;
        }

        public string VisitBinaryExpr<T>(Expr.Binary expr)
        {
            return Parenthesize(expr.op.lexeme, expr.left, expr.right);
        }

        public string VisitGroupingExpr<T>(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.expression);
        }

        public string VisitLiteralExpr<T>(Expr.Literal expr)
        {
            if (expr.value == null) return "nil";
            return expr.value.ToString();
        }

        public string VisitUnaryExpr<T>(Expr.Unary expr)
        {
            return Parenthesize(expr.op.lexeme, expr.right);
        }

        public string VisitVariableExpr<T>(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        public string VisitAssignExpr<T>(Expr.Assign expr)
        {
            throw new NotImplementedException();
        }
    }
}