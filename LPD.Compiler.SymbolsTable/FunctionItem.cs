﻿namespace LPD.Compiler.SymbolsTable
{
    public sealed class FunctionItem : SymbolTableItem
    {
        public string Level { get; set; }

        public ItemType Type { get; set; }
    }
}