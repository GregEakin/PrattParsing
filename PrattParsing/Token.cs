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

public class Token
{
    public enum TokenType
    {
        Identifier,
        Integer,
        Add,
        Sub,
        Mul,
        Div,
        Exp,
        LPar,
        RPar,
        Eof,
    }

    public TokenType Type { get; }
    public string? Identifier { get; }
    public int? IntegerValue { get; }
    public int Start { get; }
    public int End { get; }

    public Token(TokenType type, string? identifier = null, int? integerValue = null, int start = 0, int end = 0)
    {
        Type = type;
        Identifier = identifier;
        IntegerValue = integerValue;
        Start = start;
        End = end;
    }

    public int Lbp() => 
        Type switch
        {
            TokenType.Identifier or TokenType.LPar or TokenType.Integer or TokenType.RPar => 0,
            TokenType.Add or TokenType.Sub => 2,
            TokenType.Mul or TokenType.Div => 3,
            TokenType.Exp => 4,
            TokenType.Eof => -1,
            _ => throw new ArgumentException($"Unknown token", nameof(Type))
        };

    public override string ToString() =>
        Type switch
        {
            TokenType.Identifier => $"(IDENT \"{Identifier}\")",
            TokenType.Integer => $"INT({IntegerValue})",
            TokenType.Add => "ADD",
            TokenType.Sub => "SUB",
            TokenType.Mul => "MUL",
            TokenType.Div => "DIV",
            TokenType.Exp => "EXP",
            TokenType.LPar => "LPAR",
            TokenType.RPar => "RPAR",
            TokenType.Eof => "EOF",
            _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
        };
}