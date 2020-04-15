using System;
using System.Collections.Generic;

namespace lox
{
	public abstract class Expr
	{
		internal interface Visitor<R>
		{
			R VisitAssignExpr<T>(Assign expr);
			R VisitBinaryExpr<T>(Binary expr);
			R VisitGroupingExpr<T>(Grouping expr);
			R VisitLiteralExpr<T>(Literal expr);
			R VisitLogicalExpr<T>(Logical expr);
			R VisitUnaryExpr<T>(Unary expr);
			R VisitVariableExpr<T>(Variable expr);
		}

		public class Assign : Expr
		{
			public Assign(Token name, Expr value)
			{
				this.name = name;
				this.value = value;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitAssignExpr<R>(this);
			}

			public readonly Token name;
			public readonly Expr value;
		}

		public class Binary : Expr
		{
			public Binary(Expr left, Token op, Expr right)
			{
				this.left = left;
				this.op = op;
				this.right = right;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitBinaryExpr<R>(this);
			}

			public readonly Expr left;
			public readonly Token op;
			public readonly Expr right;
		}

		public class Grouping : Expr
		{
			public Grouping(Expr expression)
			{
				this.expression = expression;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitGroupingExpr<R>(this);
			}

			public readonly Expr expression;
		}

		public class Literal : Expr
		{
			public Literal(Object value)
			{
				this.value = value;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitLiteralExpr<R>(this);
			}

			public readonly Object value;
		}

		public class Logical : Expr
		{
			public Logical(Expr left, Token op, Expr right)
			{
				this.left = left;
				this.op = op;
				this.right = right;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitLogicalExpr<R>(this);
			}

			public readonly Expr left;
			public readonly Token op;
			public readonly Expr right;
		}

		public class Unary : Expr
		{
			public Unary(Token op, Expr right)
			{
				this.op = op;
				this.right = right;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitUnaryExpr<R>(this);
			}

			public readonly Token op;
			public readonly Expr right;
		}

		public class Variable : Expr
		{
			public Variable(Token name)
			{
				this.name = name;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitVariableExpr<R>(this);
			}

			public readonly Token name;
		}

		internal abstract R Accept<R>(Visitor<R> visitor);
	}
}
