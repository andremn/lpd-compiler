using System.Linq;
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
            var symbolsTable = new VectorSymbolTable();

            for (int i = 0; i < 5; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Lexeme = "Item " + i, Type = ItemType.Boolean });
            }

            for (int i = 5; i < 8; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Lexeme = "Item " + i });
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
            var symbolsTable = new VectorSymbolTable();

            symbolsTable.Insert(new FunctionItem() { Lexeme = "Func1" });
            symbolsTable.Insert(new FunctionItem() { Lexeme = "Func2" });
            symbolsTable.SetFunctionType("Func1", ItemType.Boolean);

            var func = symbolsTable.Search("Func1") as FunctionItem;

            Assert.IsNotNull(func);
            Assert.AreEqual(func.Lexeme, "Func1");
            Assert.AreEqual(ItemType.Boolean, func.Type);
        }

        [TestMethod]
        public void REMOVE_UNTIL_TEST()
        {
            var symbolsTable = new VectorSymbolTable();
            var func1 = new FunctionItem() { Lexeme = "Func1" };
            var func2 = new FunctionItem() { Lexeme = "Func2" };

            symbolsTable.Insert(func1);

            for (int i = 0; i < 5; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Lexeme = "Item " + i, Type = ItemType.Boolean, Level = 1 });
            }

            symbolsTable.Insert(func2);

            for (int i = 5; i < 8; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Lexeme = "Item " + i, Type = ItemType.Boolean, Level = 2 });
            }

            symbolsTable.RemoveUntil(2);

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
            var symbolsTable = new VectorSymbolTable();
            var func = new FunctionItem() { Lexeme = "Func1" };
            var proc = new ProcItem() { Lexeme = "Proc1"};
            var item1 = new IdentificatorItem() { Lexeme = "x", Type = ItemType.Boolean, Level = 1 };
            var item2 = new IdentificatorItem() { Lexeme = "a", Type = ItemType.Integer, Level = 2 };
            var item3 = new IdentificatorItem() { Lexeme = "b", Type = ItemType.Integer, Level = 2 };

            for (int i = 0; i < 5; i++)
            {
                symbolsTable.Insert(new IdentificatorItem() { Lexeme = "Item " + i, Type = ItemType.Boolean });
            }

            symbolsTable.Insert(func);
            symbolsTable.Insert(item1);
            symbolsTable.Insert(proc);
            symbolsTable.Insert(item2);
            symbolsTable.Insert(item3);

            var result = symbolsTable.SearchByLevel(item1.Lexeme, 1);

            Assert.AreEqual(result, item1);

            result = symbolsTable.SearchByLevel(item2.Lexeme, 1);
            Assert.IsNull(result);

            result = symbolsTable.SearchByLevel(item1.Lexeme, 2);
            Assert.IsNull(result);

            result = symbolsTable.SearchByLevel(item2.Lexeme, 2);
            Assert.AreEqual(result, item2);

            result = symbolsTable.SearchByLevel(item3.Lexeme, 2);
            Assert.AreEqual(result, item3);
        }

        [TestMethod]
        public void SEARCH_DOUBLE()
        {
            var symbolsTable = new VectorSymbolTable();
            var item1 = new IdentificatorItem() { Lexeme = "x", Type = ItemType.Boolean };
            var item2 = new IdentificatorItem() { Lexeme = "x", Type = ItemType.Integer };
            var item3 = new IdentificatorItem() { Lexeme = "y", Type = ItemType.Integer };

            symbolsTable.Insert(item1);
            symbolsTable.Insert(item3);
            symbolsTable.Insert(item2);

            Assert.IsTrue(symbolsTable.SearchDouble("x"));
        }

        [TestMethod]
        public void CLEAN_UP_TEST()
        {
            var symbolsTable = new VectorSymbolTable();
            var item1 = new IdentificatorItem() { Lexeme = "a", Type = ItemType.Boolean, Level = 1 };
            var item2 = new IdentificatorItem() { Lexeme = "x", Type = ItemType.Integer, Level = 2 };
            var item3 = new IdentificatorItem() { Lexeme = "y", Type = ItemType.Integer, Level = 1 };
            var func = new FunctionItem() { Lexeme = "func", Type = ItemType.Integer, };
            var proc = new ProcItem() { Lexeme = "proc" };

            symbolsTable.Insert(func);
            symbolsTable.Insert(item1);
            symbolsTable.Insert(item3);
            symbolsTable.Insert(proc);
            symbolsTable.Insert(item2);

            symbolsTable.CleanUpToLevel(2);
            Assert.IsNull(symbolsTable.Search(item2.Lexeme));
            symbolsTable.CleanUpToLevel(1);
            Assert.IsNull(symbolsTable.Search(item1.Lexeme));
            Assert.IsNull(symbolsTable.Search(item3.Lexeme));
        }
    }
}
