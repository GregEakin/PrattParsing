// Copyright 2025 Gregory Eakin
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace PrattParsing;

public class ParserTests
{
    private class ExprPrintVisitor : IExpressionVisitor<string>
    {
        public string VisitVar(VarExpr expr) => expr.Name;
        public string VisitInt(IntExpr expr) => expr.Value.ToString();
        public string VisitBop(BopExpr expr)
            => $"({expr.Left.Accept(this)} {Token.ShowBop(expr.Op)} {expr.Right.Accept(this)})";
    }

    [Test]
    [Arguments("a", "a")]
    [Arguments("a + b", "(a + b)")]
    [Arguments("a + b + c + d", "(((a + b) + c) + d)")]
    [Arguments("a + b * c + d", "((a + (b * c)) + d)")]
    [Arguments("a + b^2^3 + c", "((a + ((b ^ 2) ^ 3)) + c)")]
    [Arguments("a * (b + c)", "(a * (b + c))")]
    [Arguments("a * (a + b * c + a)", "(a * ((a + (b * c)) + a))")]
    public async Task ParseExpression(string expression, string expected)
    {
        // Console.WriteLine(expression);
        var expr = PrattParser.ParseExpr(expression);
        var printer = new ExprPrintVisitor();
        await Assert.That(expr.Accept(printer)).IsEqualTo(expected);
    }

    [Test]
    [Arguments("a", "(IDENT \"a\"),EOF")]
    [Arguments("a + b", "(IDENT \"a\"),ADD,(IDENT \"b\"),EOF")]
    [Arguments("a + b + c + d", "(IDENT \"a\"),ADD,(IDENT \"b\"),ADD,(IDENT \"c\"),ADD,(IDENT \"d\"),EOF")]
    public async Task LexerTest(string expression, string expected)
    {
        var tokens = PrattParser.Lex(expression);
        var result = string.Join(",", tokens.Select(token => token.ToString()));
        // Console.WriteLine(result);
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task EvaluateTest()
    {
        var expr = PrattParser.ParseExpr("a + 2 * b");
        var vars = new Dictionary<string, int> { ["a"] = 3, ["b"] = 4 };
        var result = expr.Evaluate(vars);
        await Assert.That(result).IsEqualTo(11);
    }
}