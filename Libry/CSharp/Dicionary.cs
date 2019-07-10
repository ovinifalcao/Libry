using System.Collections.Generic;

namespace Libry
{
    class Dicionary
    {

        public static readonly List<string> MainStructures = new List<string>
        {
            "class",
            "struct",
            "enum",
            "namespace",
            "interface",
            "delegate"
        };

        public static readonly List<string> MethodsCommonStructures = new List<string>
        {
            "void",
            "partial",
            "virtual"
        };

        public static readonly List<string> PropertiesCommonStructures = new List<string>
        {
            "get;",
            "set;",
            "get",
            "set"
        };
        public static readonly List<string> AcessModifiers = new List<string>()
        {
            "public",
            "protected",
            "internal",
            "protected internal",
            "private",
            "private protected"
        };

        //YES I KNOW STRING IS NOT A PRIMITIVE TYPE
        public static readonly List<string> PrimitiveTypes = new List<string>()
        {
            "bool",
            "byte",
            "sbyte",
            "char",
            "decimal",
            "double",
            "float",
            "int",
            "uint",
            "long",
            "ulong",
            "object",
            "short",
            "ushort",
            "string",
            "void"
        };
    }
}
