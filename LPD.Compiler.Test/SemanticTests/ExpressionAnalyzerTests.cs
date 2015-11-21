using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LPD.Compiler.Semantic;
using LPD.Compiler.Lexical;
using LPD.Compiler.SymbolsTable;

namespace LPD.Compiler.Test.SemanticTests
{
    [TestClass]
    public class ExpressionAnalyzerTests
    {
        private VectorSymbolTable _symbolTable;

        [TestInitialize]
        public void InitTests()
        {
            _symbolTable = new VectorSymbolTable();
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "a", Type = ItemType.Integer });
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "b", Type = ItemType.Integer });
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "c", Type = ItemType.Integer });
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "d", Type = ItemType.Integer });
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "e", Type = ItemType.Integer });
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "f", Type = ItemType.Boolean });
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "g", Type = ItemType.Boolean });
            _symbolTable.Insert(new IdentificatorItem() { Lexeme = "h", Type = ItemType.Boolean });
        }

        [TestMethod]
        public void INTEGER_EXPRESSION_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            var output = string.Empty;
            var expectedOutput = "ab+cd*+e+";
            var token = new Token { Lexeme = "a", Symbol = Symbols.SIdentificador };
            var type = ItemType.None;

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

            expressionAnalyzer.Analyze(out type);
            output = expressionAnalyzer.ToString();

            Assert.AreEqual(type, ItemType.Integer);
            Assert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public void INTEGER_UNARY_EXPRESSION_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            var output = string.Empty;
            var expectedOutput = "a-cd-e*+";
            var token = new Token { Lexeme = "-", Symbol = Symbols.SMaisUnario };
            var type = ItemType.None;

            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "a", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "+", Symbol = Symbols.SMais };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "c", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "-", Symbol = Symbols.SMenos };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "d", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "*", Symbol = Symbols.SMult };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "e", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);

            expressionAnalyzer.Analyze(out type);
            output = expressionAnalyzer.ToString();

            Assert.AreEqual(type, ItemType.Integer);
            Assert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public void INTEGER_CONSTS_ONLY_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            var output = string.Empty;
            var expectedOutput = "32-45*+";
            var token = new Token { Lexeme = "3", Symbol = Symbols.SNumero };
            var type = ItemType.None;

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

            expressionAnalyzer.Analyze(out type);
            output = expressionAnalyzer.ToString();

            Assert.AreEqual(type, ItemType.Integer);
            Assert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public void INTEGER_BOOLEAN_EXPRESSION_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            var output = string.Empty;
            var expectedOutput = "ab+cd-<";
            var token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            var type = ItemType.None;

            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "a", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "+", Symbol = Symbols.SMais };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "b", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "<", Symbol = Symbols.SMenorIg };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "c", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "-", Symbol = Symbols.SMenos };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "d", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);

            expressionAnalyzer.Analyze(out type);
            output = expressionAnalyzer.ToString();

            Assert.AreEqual(type, ItemType.Boolean);
            Assert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public void INTEGER_BOOLEAN_EXPRESSION_COMPLEX_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            var output = string.Empty;
            var expectedOutput = "a-badivab-**<nao";
            var token = new Token { Lexeme = "nao", Symbol = Symbols.SNao };
            var type = ItemType.None;

            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "-", Symbol = Symbols.SMenosUnario };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "a", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "<", Symbol = Symbols.SMenor };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "b", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "div", Symbol = Symbols.SDiv };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "a", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "*", Symbol = Symbols.SMult };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "a", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "*", Symbol = Symbols.SMult };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "(", Symbol = Symbols.SAbreParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "-", Symbol = Symbols.SMenosUnario };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "b", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = ")", Symbol = Symbols.SFechaParenteses };
            expressionAnalyzer.Add(token);

            expressionAnalyzer.Analyze(out type);
            output = expressionAnalyzer.ToString();

            Assert.AreEqual(type, ItemType.Boolean);
            Assert.AreEqual(output, expectedOutput);
        }

        [TestMethod]
        public void BOOLEAN_EXPRESSION_TEST()
        {
            var expressionAnalyzer = new ExpressionAnalyzer(_symbolTable);
            var output = string.Empty;
            var expectedOutput = "fnaogou";
            var token = new Token { Lexeme = "nao", Symbol = Symbols.SNao };
            var type = ItemType.None;

            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "f", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "ou", Symbol = Symbols.SOu };
            expressionAnalyzer.Add(token);
            token = new Token { Lexeme = "g", Symbol = Symbols.SIdentificador };
            expressionAnalyzer.Add(token);

            expressionAnalyzer.Analyze(out type);
            output = expressionAnalyzer.ToString();

            Assert.AreEqual(type, ItemType.Boolean);
            Assert.AreEqual(output, expectedOutput);
        }
    }
}
