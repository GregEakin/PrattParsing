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
        var result = ParseState.Expr(state, 0);
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

}