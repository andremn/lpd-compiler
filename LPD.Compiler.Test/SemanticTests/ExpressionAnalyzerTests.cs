using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LPD.Compiler.Semantic;
using LPD.Compiler.Lexical;

namespace LPD.Compiler.Test.SemanticTests
{
    [TestClass]
    public class ExpressionAnalyzerTests
    {
        [TestMethod]
        public void ADD_TOKENS_IDENTIFICATOR_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer();
            var output = string.Empty;
            var expectedOutput = "ab+cd*+e+";
            var token = new Token { Lexeme = "a", Symbol = Symbols.SIdentificador };

            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "+", Symbol = Symbols.SMais };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "b", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "+", Symbol = Symbols.SMais };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "c", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "*", Symbol = Symbols.SMult };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "d", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "+", Symbol = Symbols.SMais };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "e", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            expressionAnalyzer.PopAll();
            output = expressionAnalyzer.GetOutput();

            Assert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public void ADD_TOKENS_ONLY_CONST_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer();
            var output = string.Empty;
            var expectedOutput = "32-45*+";
            var token = new Token { Lexeme = "3", Symbol = Symbols.SNumero };

            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "-", Symbol = Symbols.SMenos };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "2", Symbol = Symbols.SNumero };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "+", Symbol = Symbols.SMais };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "4", Symbol = Symbols.SNumero };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "*", Symbol = Symbols.SMult };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "5", Symbol = Symbols.SNumero };
            expressionAnalyzer.Add(token);
            expressionAnalyzer.PopAll();
            output = expressionAnalyzer.GetOutput();

            Assert.AreEqual(output, expectedOutput);
        }
    }
}
