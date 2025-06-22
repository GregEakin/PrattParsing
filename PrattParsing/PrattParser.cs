// Copyright 2025 Gregory Eakin
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

namespace PrattParsing;

public static class PrattParser
{
    public static Expression ParseExpr(string str)
    {
        var tokens = Lex(str);
        // Console.WriteLine(string.Join(",", tokens.Select(token => token.ToString())));
        var state = new ParseState(tokens);
        var result = Expr(state, 0);
        return result;
    }

    public static List<Token> Lex(string str)
    {
        var tokens = new List<Token>();
        var i = 0;
        while (i < str.Length)
        {
            var c = str[i];

            // Skip whitespace
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            // Identifiers: [a-zA-Z][a-zA-Z0-9]*
            if (char.IsLetter(c))
            {
                var start = i;
                while (i < str.Length && (char.IsLetterOrDigit(str[i])))
                    i++;
                var id = str.Substring(start, i - start);
                tokens.Add(new Token(Token.TokenType.Identifier, identifier: id, start: start, end: i));
                continue;
            }

            // Integers: [0-9]+
            if (char.IsDigit(c))
            {
                var start = i;
                while (i < str.Length && char.IsDigit(str[i]))
                    i++;
                var num = str.Substring(start, i - start);
                tokens.Add(new Token(Token.TokenType.Integer, integerValue: int.Parse(num), start: start, end: i));
                continue;
            }

            // Single-character tokens
            switch (c)
            {
                case '+':
                    tokens.Add(new Token(Token.TokenType.Add, start: i, end: i + 1));
                    break;
                case '-':
                    tokens.Add(new Token(Token.TokenType.Sub, start: i, end: i + 1));
                    break;
                case '*':
                    tokens.Add(new Token(Token.TokenType.Mul, start: i, end: i + 1));
                    break;
                case '/':
                    tokens.Add(new Token(Token.TokenType.Div, start: i, end: i + 1));
                    break;
                case '^':
                    tokens.Add(new Token(Token.TokenType.Exp, start: i, end: i + 1));
                    break;
                case '(':
                    tokens.Add(new Token(Token.TokenType.LPar, start: i, end: i + 1));
                    break;
                case ')':
                    tokens.Add(new Token(Token.TokenType.RPar, start: i, end: i + 1));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown character: {c}");
            }

            i++;
        }

        tokens.Add(new Token(Token.TokenType.Eof));
        return tokens;
    }

    public static Expression Expr(ParseState state, int limit)
    {
        Token Current() => state.Current;
        void Advance() => state.Advance();

        Expression Nud(Token token)
        {
            switch (token)
            {
                case { Type: Token.TokenType.Identifier, Identifier: not null }:
                    return new VarExpr(token.Identifier, token.Start, token.End);

                case { Type: Token.TokenType.Integer, IntegerValue: not null }:
                    return new IntExpr(token.IntegerValue.Value, token.Start, token.End);

                case { Type: Token.TokenType.LPar }:
                {
                    var expr = Expr(state, 0);
                    if (Current().Type != Token.TokenType.RPar)
                        throw new ArgumentException("Expected closing paren", nameof(token));

                    var start = token.Start;
                    var end = Current().End;
                    Advance(); // consume ')'
                    // Wrap in a BopExpr or just return expr with updated span if you want
                    // For now, just return expr (optionally update expr's Start/End if needed)
                    return expr;
                }

                default:
                    throw new ArgumentException($"No nud for {token}", nameof(token));
            }
        }

        Expression Led(Expression left, Token token)
        {
            var nextLimit = token.Type switch
            {
                Token.TokenType.Exp => 5,
                Token.TokenType.Mul => 3,
                Token.TokenType.Div => 3,
                _ => 2
            };
            var right = Expr(state, nextLimit);
            return new BopExpr(token.Type, left, right, left.Start, right.End);
        }

        // --- Pratt parsing core ---
        var first = Current();
        Advance();
        var left = Nud(first);
        while (Current().Lbp() > limit)
        {
            var next = Current();
            Advance();
            left = Led(left, next);
        }

        return left;
    }
}