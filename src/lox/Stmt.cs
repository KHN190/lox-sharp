using System;
using System.Collections.Generic;

namespace lox
{
	public abstract class Stmt
	{
		internal interface Visitor<R>
		{
			R VisitBlockStmt<T>(Block stmt);
			R VisitExpressionStmt<T>(Expression stmt);
			R VisitFunctionStmt<T>(Function stmt);
			R VisitIfStmt<T>(If stmt);
			R VisitPrintStmt<T>(Print stmt);
			R VisitReturnStmt<T>(Return stmt);
			R VisitVarStmt<T>(Var stmt);
			R VisitWhileStmt<T>(While stmt);
		}

		public class Block : Stmt
		{
			public Block(List<Stmt> statements)
			{
				this.statements = statements;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitBlockStmt<R>(this);
			}

			public readonly List<Stmt> statements;
		}

		public class Expression : Stmt
		{
			public Expression(Expr expression)
			{
				this.expression = expression;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitExpressionStmt<R>(this);
			}

			public readonly Expr expression;
		}

		public class Function : Stmt
		{
			public Function(Token name, List<Token> parameters, List<Stmt> body)
			{
				this.name = name;
				this.parameters = parameters;
				this.body = body;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitFunctionStmt<R>(this);
			}

			public readonly Token name;
			public readonly List<Token> parameters;
			public readonly List<Stmt> body;
		}

		public class If : Stmt
		{
			public If(Expr condition, Stmt thenBranch, Stmt elseBranch)
			{
				this.condition = condition;
				this.thenBranch = thenBranch;
				this.elseBranch = elseBranch;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitIfStmt<R>(this);
			}

			public readonly Expr condition;
			public readonly Stmt thenBranch;
			public readonly Stmt elseBranch;
		}

		public class Print : Stmt
		{
			public Print(Expr expression)
			{
				this.expression = expression;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitPrintStmt<R>(this);
			}

			public readonly Expr expression;
		}

		public class Return : Stmt
		{
			public Return(Token keyword, Expr value)
			{
				this.keyword = keyword;
				this.value = value;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitReturnStmt<R>(this);
			}

			public readonly Token keyword;
			public readonly Expr value;
		}

		public class Var : Stmt
		{
			public Var(Token name, Expr initializer)
			{
				this.name = name;
				this.initializer = initializer;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitVarStmt<R>(this);
			}

			public readonly Token name;
			public readonly Expr initializer;
		}

		public class While : Stmt
		{
			public While(Expr condition, Stmt body)
			{
				this.condition = condition;
				this.body = body;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitWhileStmt<R>(this);
			}

			public readonly Expr condition;
			public readonly Stmt body;
		}

		internal abstract R Accept<R>(Visitor<R> visitor);
	}
}
