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

public class BopExpr : Expression
{
    public Token.TokenType Op { get; }
    public Expression Left { get; }
    public Expression Right { get; }

    public BopExpr(Token.TokenType op, Expression left, Expression right, int start, int end)
        : base(start, end)
    {
        Op = op;
        Left = left;
        Right = right;
    }

    public override ExpressionKind Kind => ExpressionKind.BinaryOp;

    public override T Accept<T>(IExpressionVisitor<T> visitor) => visitor.VisitBop(this);

    public override int Evaluate(Dictionary<string, int> variables)
    {
        var left = Left.Evaluate(variables);
        var right = Right.Evaluate(variables);
        return Op switch
        {
            Token.TokenType.Add => left + right,
            Token.TokenType.Sub => left - right,
            Token.TokenType.Mul => left * right,
            Token.TokenType.Div => left / right,
            Token.TokenType.Exp => (int)Math.Pow(left, right),
            _ => throw new InvalidOperationException($"Unknown operator: {Op}")
        };
    }

    public override string ToString() => $"({Left} {Token.ShowBop(Op)} {Right})";
}