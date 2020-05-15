using System;
using System.Collections.Generic;

using static lox.TokenType;

namespace lox
{
    public class Parser
    {
        /* Parser generates
         *  1. Binary / Unary expressions, also
         *  2. Statements,
         *  3. Finally a syntax tree.
         */

        // Exception
        private class ParseError : SystemException { }

        // Attrs
        private readonly List<Token> tokens;
        private int current;

        // Constructor
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        // program → declaration* EOF ;
        public List<Stmt> Parse()
        {
            List<Stmt> statements = new List<Stmt>();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }



        #region Statements

        // statement → exprStmt | ifStmt | printStmt | returnStmt | whileStmt | forStmt | block ;
        private Stmt Statement()
        {
            if (Match(IF)) return IfStatement();
            if (Match(PRINT)) return PrintStatement();
            if (Match(RETURN)) return ReturnStatement();
            if (Match(WHILE)) return WhileStatement();
            if (Match(FOR)) return ForStatement();
            if (Match(LEFT_BRACE)) return new Stmt.Block(Block());

            return ExpressionStatement();
        }

        // declaration → funDecl | varDecl | statement ;
        private Stmt Declaration()
        {
            try
            {
                if (Match(FUN)) return FunDeclaration("function");
                if (Match(VAR)) return VarDeclaration();
                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt NonVarDeclaration()
        {
            try
            {
                if (Match(FUN)) return FunDeclaration("function");
                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }

        // funDecl → "fun" function ;
        // function → IDENTIFIER? "(" parameters? ")" block ;
        //      parameters → IDENTIFIER ( "," IDENTIFIER )* ;

        private Stmt FunDeclaration(string kind)
        {
            Token name;

            // anonymous function
            if (Check(LEFT_PAREN))
            {
                int line = Peek().line;

                name = new Token(IDENTIFIER, "#lambda_" + line + "_" + current, null, line);
            }
            // normal function
            else
            {
                name = Consume(IDENTIFIER, "Expect " + kind + " name.");
            }

            Consume(LEFT_PAREN, "Expect '(' after " + kind + " name.");

            List<Token> parameters = new List<Token>();

            // parse params
            if (!Check(RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Cannot have more than 255 parameters.");
                    }
                    parameters.Add(Consume(IDENTIFIER, "Expect parameter name."));
                }
                while (Match(COMMA));
            }
            Consume(RIGHT_PAREN, "Expect ')' after parameters.");

            // parse body
            Consume(LEFT_BRACE, "Expect '{' before " + kind + " body.");

            List<Stmt> body = Block();

            return new Stmt.Function(name, parameters, body);
        }

        // varDecl → "var" IDENTIFIER ( "=" declaration )? ";" ;
        private Stmt VarDeclaration()
        {
            Token name = Consume(IDENTIFIER, "Expect variable name.");
            Stmt initializer = null;

            if (Match(EQUAL)) initializer = NonVarDeclaration();

            return new Stmt.Var(name, initializer);
        }

        // printStmt → "print" expression ";" ;
        private Stmt PrintStatement()
        {
            Expr expr = Expression();
            Consume(SEMICOLON, "Expect ';' after value.");
            return new Stmt.Print(expr);
        }

        // returnStmt → "return" expression? ";" ;
        private Stmt ReturnStatement()
        {
            Token keyword = Previous();
            Expr value = null;

            if (!Check(SEMICOLON))
            {
                value = Expression();
            }
            Consume(SEMICOLON, "Expect ';' after return value.");

            return new Stmt.Return(keyword, value);
        }

        // exprStmt → expression ";" ;
        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(SEMICOLON, "Expect ';' after expression.");
            return new Stmt.Expression(expr);
        }

        // block → "{" declaration* "}" ;
        private List<Stmt> Block()
        {
            List<Stmt> stmts = new List<Stmt>();

            while (!Check(RIGHT_BRACE) && !IsAtEnd())
            {
                stmts.Add(Declaration());
            }
            Consume(RIGHT_BRACE, "Expect '}' after block.");

            return stmts;
        }

        // whileStmt → "while" "(" expression ")" statement ;
        private Stmt WhileStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after while condition.");

            Stmt body = Statement();

            return new Stmt.While(condition, body);
        }

        // ifStmt → "if" "(" expression ")" statement ( "else" statement )? ;
        private Stmt IfStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt elseBranch = null;
            if (Match(ELSE))
            {
                elseBranch = Statement();
            }

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        // forStmt → "for" "(" ( varDecl | exprStmt | ";" )
        //                      expression? ";"
        //                      expression? ")" statement ;

        private Stmt ForStatement()
        {
            Consume(LEFT_PAREN, "Expect '(' after 'for'.");

            Stmt initializer;
            Expr condition = null;
            Expr increment = null;

            // first part
            if (Match(SEMICOLON))
            {
                initializer = null;
            }
            else if (Match(VAR))
            {
                initializer = VarDeclaration();
            }
            else
            {
                initializer = ExpressionStatement();
            }

            // second part
            if (!Check(SEMICOLON))
            {
                condition = Expression();
            }
            Consume(SEMICOLON, "Expect ';' after loop condition.");

            // third part
            if (!Check(RIGHT_PAREN))
            {
                increment = Expression();
            }
            Consume(RIGHT_PAREN, "Expect ')' after for clauses.");

            // body for clauses
            Stmt body = Statement();

            if (increment != null)
            {
                body = new Stmt.Block(
                    new List<Stmt> { body, new Stmt.Expression(increment) });
            }

            if (condition == null) condition = new Expr.Literal(true);
            body = new Stmt.While(condition, body);

            if (initializer != null)
            {
                body = new Stmt.Block(
                    new List<Stmt> { initializer, body });
            }

            return body;
        }
        #endregion



        #region Binary Expressions

        // expression → assignment ;
        internal Expr Expression()
        {
            return Assignment();
        }

        // assignment → IDENTIFIER "=" assignment | equality | logic_or ;
        private Expr Assignment()
        {
            // variable assignment
            Expr expr = Or();

            if (Match(EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is Expr.Variable)
                {
                    Token name = ((Expr.Variable)expr).name;
                    return new Expr.Assign(name, value);
                }
                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        // logic_or → logic_and ( "or" logic_and )* ;
        private Expr Or()
        {
            Expr expr = And();

            while (Match(OR))
            {
                Token op = Previous();
                Expr right = And();
                expr = new Expr.Logical(expr, op, right);
            }
            return expr;
        }

        // logic_and → equality ( "and" equality )* ;
        private Expr And()
        {
            Expr expr = Equality();

            while (Match(AND))
            {
                Token op = Previous();
                Expr right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }
            return expr;
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



        #region Unary Expressions

        // unary → ( "!" | "-" | "+" ) unary | call ;
        private Expr Unary()
        {
            if (Match(MINUS, BANG))
            {
                Token op = Previous();
                Expr right = Unary();
                return new Expr.Unary(op, right);
            }
            return Call();
        }

        // call → primary ( "(" arguments? ")" )* ;
        private Expr Call()
        {
            Expr expr = Primary();

            while (true)
            {
                if (Match(LEFT_PAREN))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }
            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            List<Expr> arguments = new List<Expr>();

            // 0 or more arguments
            if (!Check(RIGHT_PAREN))
            {
                // arguments → expression ( "," expression )* ;
                do
                {
                    if (arguments.Count >= 255)
                    {
                        Error(Peek(), "Cannot have more than 255 arguments.");
                    }
                    arguments.Add(Expression());
                }
                while (Match(COMMA));
            }
            Token paren = Consume(RIGHT_PAREN, "Expect ')' after arguments.");

            return new Expr.Call(callee, paren, arguments);
        }

        // primary → NUMBER | STRING | "false" | "true" | "nil" | "(" expression ")" | identifier ;
        private Expr Primary()
        {
            if (Match(FALSE)) return new Expr.Literal(false);
            if (Match(TRUE)) return new Expr.Literal(true);
            if (Match(NIL)) return new Expr.Literal(null);

            if (Match(NUMBER, STRING))
            {
                return new Expr.Literal(Previous().literal);
            }

            if (Match(IDENTIFIER))
            {
                return new Expr.Variable(Previous());
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



        #region Parser Utils

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
        #endregion



        #region Parser Errors

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
        #endregion
    }
}
