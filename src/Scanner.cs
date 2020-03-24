using System;
using System.Collections.Generic;
using static lox.TokenType;

namespace lox
{
	public class Scanner
	{
		#region Attrs

		private readonly string source;
		private readonly List<Token> tokens = new List<Token>();
		private int start = 0;
		private int current = 0;
		private int line = 1;

		private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>() 
		{
			{"and",    AND},
			{"class",  CLASS},
			{"else",   ELSE},
			{"false",  FALSE},
			{"for",    FOR},
			{"fun",    FUN},
			{"if",     IF},
			{"nil",    NIL},
			{"or",     OR},
			{"print",  PRINT},
			{"return", RETURN},
			{"super",  SUPER},
			{"this",   THIS},
			{"true",   TRUE},
			{"var",    VAR},
			{"while",  WHILE}
		};

		public Scanner(string source)
		{
			this.source = source;
		}
		#endregion



		#region ScanMethods

		public List<Token> ScanTokens()
		{
			while (!IsAtEnd())
			{
				start = current;
				ScanToken();
			}

			tokens.Add(new Token(EOF, "", null, line));
			return tokens;
		}

		private void ScanToken()
		{
			char c = Advance();
			switch (c)
			{
				case '(': AddToken(LEFT_PAREN); break;
				case ')': AddToken(RIGHT_PAREN); break;
				case '{': AddToken(LEFT_BRACE); break;
				case '}': AddToken(RIGHT_BRACE); break;
				case ',': AddToken(COMMA); break;
				case '.': AddToken(DOT); break;
				case '-': AddToken(MINUS); break;
				case '+': AddToken(PLUS); break;
				case ';': AddToken(SEMICOLON); break;
				case '*': AddToken(STAR); break;

				case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
				case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
				case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
				case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;

				case ' ': break;
				case '\r': break;
				case '\t': break;
				case '\n': line++; break;

				case '"': String(); break;

				case '/':
					if (Match('/'))
						while (Peek() != '\n' && !IsAtEnd()) Advance();
					else
						AddToken(SLASH);
					break;

				default:
					if (Char.IsDigit(c))
						Number();
					else if (IsLetter(c)) // lox specific, we don't believe in C#
						Identifier();
					else
						Lox.Error(line, "unexpected character: " + c);
					break;
			}
		}
		#endregion



		#region CreateTokenType

		private void String()
		{
			while (Peek() != '"' && !IsAtEnd()) 
			{
				if (Peek() == '\n') line++;
				Advance();
			}

			// Unterminated string
			if (IsAtEnd())
			{
				Lox.Error(line, "Unterminated string.");
				return;
			}

			// Closing "
			Advance();

			// Trim the surrounding quotes
			string value = source.Substring(start + 1, current - 1 - start - 1);
			AddToken(STRING, value);
		}

		private void Number()
		{
			while (Char.IsDigit(Peek())) Advance();

			// Look for a fractional part.
			if (Peek() == '.' && Char.IsDigit(PeekNext())) 
			{
				// Consume the "."
				Advance();

				while (Char.IsDigit(Peek())) Advance();
			}

			AddToken(NUMBER, Double.Parse(source.Substring(start, current - start)));
		}

		private void Identifier()
		{
			while (IsLetterOrDigit(Peek())) Advance();

			// by default, user defined identifier
			TokenType type = IDENTIFIER;
			string text = source.Substring(start, current - start);
			
			// reserved keyword?
			if (keywords.ContainsKey(text))
				type = keywords[text];

			AddToken(type);
		}
		#endregion



		#region TokenManipulation

		private void AddToken(TokenType type, object literal = null)
		{
			string text = source.Substring(start, current - start);
			tokens.Add(new Token(type, text, literal, line));
		}

		// same as Advance but consumes 1 token
		// used for check >=, <=, ++, etc
		private bool Match(char expected)
		{
			if (IsAtEnd()) return false;
			if (source[current] != expected) return false;

			current++;
			return true;
		}

		// consumes 1 token
		private char Advance()
		{
			current++;
			return source[current - 1];
		}

		private char Peek()
		{
			if (IsAtEnd()) return '\0';
			return source[current];
		}

		private char PeekNext()
		{
			if (current + 1 >= source.Length) return '\0';
			return source[current + 1];
		}

		private bool IsAtEnd()
		{
			return current >= source.Length;
		}
		#endregion



		#region LoxTokenRules

		private static bool IsLetter(char c)
		{
			return Char.IsLetter(c) || c == '_';
		}

		private static bool IsLetterOrDigit(char c)
		{
			return IsLetter(c) || Char.IsDigit(c);
		}
		#endregion
	}
}
