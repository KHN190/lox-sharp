namespace lox
{
	public abstract class Expr
	{
		public interface Visitor<R>
		{
			R VisitBinaryExpr<T>(Binary expr);
			R VisitGroupingExpr<T>(Grouping expr);
			R VisitLiteralExpr<T>(Literal expr);
			R VisitUnaryExpr<T>(Unary expr);
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
			public Literal(object value)
			{
				this.value = value;
			}

			internal override R Accept<R>(Visitor<R> visitor)
			{
				return visitor.VisitLiteralExpr<R>(this);
			}

			public readonly object value;
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

		internal abstract R Accept<R>(Visitor<R> visitor);
	}
}
