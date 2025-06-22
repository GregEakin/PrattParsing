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

public class ParseState
{
    public IReadOnlyList<Token> Tokens { get; }
    public int Index { get; private set; }
    public Token Current => Tokens[Index];

    public ParseState(List<Token> tokens, int index = 0)
    {
        if (index < 0 || index >= tokens.Count)
            throw new ArgumentOutOfRangeException(nameof(index),
                "Index must be within the bounds of the token list.");

        Tokens = tokens.AsReadOnly();
        Index = index;
    }

    public void Advance()
    {
        if (Index < Tokens.Count - 1)
            Index++;
        else
            throw new InvalidOperationException("Cannot advance past the end of the token list.");
    }

    public static Expression Expr(ParseState state, int limit)
    {
        Token Current() => state.Current;
        void Advance() => state.Advance();

        int Lbp(Token token) => token.Type switch
        {
            Token.TokenType.Identifier or Token.TokenType.LPar or Token.TokenType.Integer or Token.TokenType.RPar => 0,
            Token.TokenType.Add or Token.TokenType.Sub => 2,
            Token.TokenType.Mul or Token.TokenType.Div => 3,
            Token.TokenType.Exp => 4,
            Token.TokenType.Eof => -1,
            _ => throw new ArgumentException($"Unknown token for lbp: {token}", nameof(token))
        };

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
                    if (Current().Type == Token.TokenType.RPar)
                    {
                        var start = token.Start;
                        var end = Current().End;
                        Advance(); // consume ')'
                        // Wrap in a BopExpr or just return expr with updated span if you want
                        // For now, just return expr (optionally update expr's Start/End if needed)
                        return expr;
                    }

                    throw new ArgumentException("Expected closing parenthesis", nameof(token));
                }

                default:
                    throw new ArgumentException($"No nud for {token}", nameof(token));
            }
        }

        Expression Led(Expression left, Token token)
        {
            var right = Expr(state, token.Type switch
            {
                Token.TokenType.Exp => 5,
                Token.TokenType.Mul => 3,
                Token.TokenType.Div => 3,
                _ => 2
            });

            return new BopExpr(token.Type, left, right, left.Start, right.End);
        }

        // --- Pratt parsing core ---
        var first = Current();
        Advance();
        var left = Nud(first);
        while (Lbp(Current()) > limit)
        {
            var next = Current();
            Advance();
            left = Led(left, next);
        }

        return left;
    }
}