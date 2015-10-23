﻿using System.Linq;
using LPD.Compiler.SymbolsTable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LPD.Compiler.Test.SymbolsTableTests
{
    [TestClass]
    public class SymbolsTableTests
    {
        [TestMethod]
        public void SET_VARS_TYPES_TEST()
        {
            var symbolsTable = new VectorSymbolsTable();

            for (int i = 0; i < 5; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Name = "variavel", Lexeme = "Item " + i, Type = ItemType.Boolean });
            }

            for (int i = 5; i < 8; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Name = "variavel", Lexeme = "Item " + i });
            }

            symbolsTable.SetTypeLastestVars(ItemType.Integer);

            for (int i = 5; i < 8; i++)
            {
                var item = symbolsTable.Search("Item " + i) as IdentificatorItem;

                Assert.IsNotNull(item);
                Assert.AreEqual(item.Type, ItemType.Integer);
            }
        }

        [TestMethod]
        public void SET_FUNCTION_TYPE_TEST()
        {
            var symbolsTable = new VectorSymbolsTable();

            symbolsTable.Insert(new FunctionItem() { Name = "funcao", Lexeme = "Func1" });
            symbolsTable.Insert(new FunctionItem() { Name = "funcao", Lexeme = "Func2" });
            symbolsTable.SetFunctionType("Func1", ItemType.Boolean);

            var func = symbolsTable.Search("Func1") as FunctionItem;

            Assert.IsNotNull(func);
            Assert.AreEqual(func.Lexeme, "Func1");
            Assert.AreEqual(ItemType.Boolean, func.Type);
        }

        [TestMethod]
        public void REMOVE_UNTIL_TEST()
        {
            var symbolsTable = new VectorSymbolsTable();
            var func1 = new FunctionItem() { Name = "funcao", Lexeme = "Func1", Level = "L1" };
            var func2 = new FunctionItem() { Name = "funcao", Lexeme = "Func2", Level = "L2" };

            symbolsTable.Insert(func1);

            for (int i = 0; i < 5; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Name = "variavel", Lexeme = "Item " + i, Type = ItemType.Boolean });
            }

            symbolsTable.Insert(func2);

            for (int i = 5; i < 8; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Name = "variavel", Lexeme = "Item " + i, Type = ItemType.Boolean });
            }

            symbolsTable.RemoveUntil("L2");

            Assert.IsTrue(symbolsTable.Items.Contains(func1));
            Assert.IsTrue(symbolsTable.Items.Contains(func2));

            var lexemes = symbolsTable.Items.Select(item => item.Lexeme);

            for (int i = 5; i < 8; i++)
            {
                Assert.IsFalse(lexemes.Contains("Item " + i));
            }
        }


        [TestMethod]
        public void SEARCH_BY_LEVEL_TEST()
        {
            var symbolsTable = new VectorSymbolsTable();
            var func = new FunctionItem() { Name = "funcao", Lexeme = "Func1", Level = "L1" };
            var proc = new ProcItem() { Name = "procedimento", Lexeme = "Proc1", Level = "L2" };
            var item1 = new IdentificatorItem() { Name = "variavel", Lexeme = "x", Type = ItemType.Boolean };
            var item2 = new IdentificatorItem() { Name = "variavel", Lexeme = "a", Type = ItemType.Integer };
            var item3 = new IdentificatorItem() { Name = "variavel", Lexeme = "b", Type = ItemType.Integer };

            for (int i = 0; i < 5; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Name = "variavel", Lexeme = "Item " + i, Type = ItemType.Boolean });
            }

            symbolsTable.Insert(func);
            symbolsTable.Insert(item1);
            symbolsTable.Insert(proc);
            symbolsTable.Insert(item2);
            symbolsTable.Insert(item3);

            var result = symbolsTable.SearchByLevel(item1.Lexeme, func.Level);

            Assert.AreEqual(result, item1);

            result = symbolsTable.SearchByLevel(item2.Lexeme, func.Level);
            Assert.IsNull(result);

            result = symbolsTable.SearchByLevel(item1.Lexeme, proc.Level);
            Assert.IsNull(result);

            result = symbolsTable.SearchByLevel(item2.Lexeme, proc.Level);
            Assert.AreEqual(result, item2);

            result = symbolsTable.SearchByLevel(item3.Lexeme, proc.Level);
            Assert.AreEqual(result, item3);
        }

        [TestMethod]
        public void SEARCH_DOUBLE()
        {
            var symbolsTable = new VectorSymbolsTable();
            var item1 = new IdentificatorItem() { Name = "variavel", Lexeme = "x", Type = ItemType.Boolean };
            var item2 = new IdentificatorItem() { Name = "variavel", Lexeme = "x", Type = ItemType.Integer };
            var item3 = new IdentificatorItem() { Name = "variavel", Lexeme = "y", Type = ItemType.Integer };

            symbolsTable.Insert(item1);
            symbolsTable.Insert(item3);
            symbolsTable.Insert(item2);

            Assert.IsTrue(symbolsTable.SearchDouble("x"));
        }
    }
}