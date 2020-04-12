using System;
using System.Collections.Generic;

namespace lox
{
    public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
    {
        // variables by scope
        private EnvironmentLox env = new EnvironmentLox();


        #region Evaluate

        internal void Interpret(Expr expr)
        {
            try
            {
                object value = Evaluate(expr);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        internal void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (Stmt stmt in statements)
                {
                    Execute(stmt);
                }
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        // returns evaluated values (double, bool, etc.)
        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        // returns null always
        private object Execute(Stmt stmt)
        {
            return stmt.Accept(this);
        }
        #endregion



        #region Statments

        object Stmt.Visitor<object>.VisitExpressionStmt<T>(Stmt.Expression stmt)
        {
            Evaluate(stmt.expression);
            return null;
        }

        object Stmt.Visitor<object>.VisitPrintStmt<T>(Stmt.Print stmt)
        {
            object value = Evaluate(stmt.expression);
            Console.WriteLine(Stringify(value));
            return null;
        }

        object Stmt.Visitor<object>.VisitVarStmt<T>(Stmt.Var stmt)
        {
            object value = null;

            if (stmt.initializer != null)
            {
                value = Evaluate(stmt.initializer);
            }
            env.Define(stmt.name.lexeme, value);

            return null;
        }
        #endregion



        #region Expressions

        // Binary operations
        object Expr.Visitor<object>.VisitBinaryExpr<T>(Expr.Binary expr)
        {
            object right = Evaluate(expr.right);

            object left = Evaluate(expr.left);

            switch (expr.op.type)
            {
                // a - b
                case TokenType.MINUS:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left - (double)right;

                // a + b
                case TokenType.PLUS:
                    if (left is double && right is double)
                        return (double)left + (double)right;

                    if (left is string && right is string)
                        return (string)left + (string)right;

                    throw new RuntimeError(expr.op, "Operands must both be numbers or strings.");

                // a * b
                case TokenType.STAR:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left * (double)right;

                // a / b
                case TokenType.SLASH:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left / (double)right;

                // a > b
                case TokenType.GREATER:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left > (double)right;

                // a < b
                case TokenType.LESS:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left < (double)right;

                // a >= b
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left >= (double)right;

                // a <= b
                case TokenType.LESS_EQUAL:
                    CheckNumberOperand(expr.op, left, right);
                    return (double)left <= (double)right;

                // a == b
                case TokenType.EQUAL:
                    return IsEqual(left, right);

                // a != b
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
            }
            // unreachable
            return null;
        }

        // Brackets
        object Expr.Visitor<object>.VisitGroupingExpr<T>(Expr.Grouping expr)
        {
            return Evaluate(expr.expression);
        }

        // Trivial literal value
        object Expr.Visitor<object>.VisitLiteralExpr<T>(Expr.Literal expr)
        {
            return expr.value;
        }

        // Unary operations
        object Expr.Visitor<object>.VisitUnaryExpr<T>(Expr.Unary expr)
        {
            object right = expr.right;

            switch (expr.op.type)
            {
                // -x
                case TokenType.MINUS:
                    CheckNumberOperand(expr.op, right);
                    return -(double)right;
                // +x
                case TokenType.PLUS:
                    CheckNumberOperand(expr.op, right);
                    return (double)right;
                // !x
                case TokenType.BANG:
                    return !IsTruthy(right);
            }
            // unreachable
            return null;
        }

        // Var operations
        object Expr.Visitor<object>.VisitVariableExpr<T>(Expr.Variable expr)
        {
            return env.Get(expr.name);
        }

        object Expr.Visitor<object>.VisitAssignExpr<T>(Expr.Assign expr)
        {
            object value = Evaluate(expr.value);
            env.Assign(expr.name, value);

            return value;
        }
        #endregion



        #region Utils

        bool IsTruthy(object expr)
        {
            if (expr == null) return false;
            if (expr is bool) return (bool)expr;
            return true;
        }

        bool IsEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;

            return left.Equals(right);
        }

        string Stringify(object value)
        {
            if (value == null) return "nil";
            return value.ToString();
        }
        #endregion



        #region Runtime Errors

        void CheckNumberOperand(Token op, object right)
        {
            if (right is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        void CheckNumberOperand(Token op, object left, object right)
        {
            if (left is double && right is double) return;
            throw new RuntimeError(op, "Operands must be numbers.");
        }
        #endregion
    }
}
