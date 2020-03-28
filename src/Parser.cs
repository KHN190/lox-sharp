using System;
using System.Collections.Generic;

using static lox.TokenType;

namespace lox
{
    public class Parser
    {
        // Exception
        private class ParseError : SystemException { }

        // Attrs
        private readonly List<Token> tokens;
        private int current = 0;

        // Constructor

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }



        #region Binary Operations

        internal Expr Expression()
        {
            return Equality();
        }

        // equality → comparison ( ( "!=" | "==" ) comparison )* ;
        private Expr Equality()
        {
            Expr expr = Comparison();

            // priority doesn't matter
            // because scanner always matched it with longest

            while (Match(BANG_EQUAL, EQUAL_EQUAL))
            {
                Token op = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        // comparison → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* ;
        private Expr Comparison()
        {
            Expr expr = Addition();

            while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
            {
                Token op = Previous();
                Expr right = Addition();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        // addition → multiplication ( ( "+" | "-" ) multiplication )* ;
        private Expr Addition()
        {
            Expr expr = Multiplication();

            while (Match(MINUS, PLUS))
            {
                Token op = Previous();
                Expr right = Multiplication();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }

        // multiplication → unary ( ( "*" | "/" ) unary )* ;
        private Expr Multiplication()
        {
            Expr expr = Unary();

            while (Match(STAR, SLASH))
            {
                Token op = Previous();
                Expr right = Multiplication();
                expr = new Expr.Binary(expr, op, right);
            }
            return expr;
        }
        #endregion



        #region Unary Operations

        // unary → ( "!" | "-" ) unary | primary ;
        private Expr Unary()
        {
            if (Match(MINUS, BANG))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Expr.Unary(op, right);
            }
            return Primary();
        }

        // primary → NUMBER | STRING | "false" | "true" | "nil" | "(" expression ")" ;
        private Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NIL)) return new Expr.Literal(null);

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous().literal);
            }

            // handle parenthesis
            if (Match(LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(RIGHT_PAREN, "Expect ')' after expression.");
                return new Expr.Grouping(expr);
            }

            // Never meant to meet this
            Error(Peek(), "Invalid syntax.");

            return null;
        }
        #endregion


        // Consume
        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }


        private bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        // Access

        private Token Advance()
        {
            if (!IsAtEnd()) current += 1;
            return Previous();
        }

        private Token Peek()
        {
            return tokens[current];
        }

        private Token Previous()
        {
            return tokens[current - 1];
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().type == type;
        }

        private bool IsAtEnd()
        {
            return Peek().type == EOF;
        }

        // Error Handler

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        // Error Recovery

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().type == SEMICOLON) return;

                switch (Peek().type)
                {
                    case CLASS:
                    case FUN:
                    case VAR:
                    case FOR:
                    case IF:
                    case WHILE:
                    case PRINT:
                    case RETURN:
                        break;
                }
            }
            Advance();
        }
    }
}