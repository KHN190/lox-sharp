using System;

namespace lox
{
	public abstract class Stmt
	{
		internal interface Visitor<R>
		{
			R VisitExpressionStmt<T>(Expression stmt);
			R VisitPrintStmt<T>(Print stmt);
			R VisitVarStmt<T>(Var stmt);
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

		internal abstract R Accept<R>(Visitor<R> visitor);
	}
}
