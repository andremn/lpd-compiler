﻿namespace LPD.Compiler.SymbolsTable
{
    public sealed class IdentificatorItem : LeveledItem
    {
        public ItemType Type { get; set; }

        public uint Memory { get; set; }
    }
}
