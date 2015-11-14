using static LPD.Compiler.SymbolsTable.Properties.Resource;

namespace LPD.Compiler.SymbolsTable
{
    public static class ItemTypeExtensions
    {
        public static string GetFriendlyName(this ItemType itemType)
        {
            string name = string.Empty;

            switch (itemType)
            {
                case ItemType.Boolean:
                    name = BooleanName.ToLower();
                    break;
                case ItemType.Integer:
                    name = IntegerName.ToLower();
                    break;
            }

            return name;
        }
    }
}
