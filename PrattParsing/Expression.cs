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

public abstract class Expression
{
    public int Start { get; }
    public int End { get; }

    protected Expression(int start = 0, int end = 0)
    {
        Start = start;
        End = end;
    }

    public abstract ExpressionKind Kind { get; }

    public enum ExpressionKind
    {
        Variable,
        Integer,
        BinaryOp
    }

    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
    public abstract int Evaluate(Dictionary<string, int> variables);
    public abstract override string ToString();
}