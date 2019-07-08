
using System.Runtime.Serialization;

namespace Libry
{
    class Implementation
    {
        public enum ImplementationType
        {
            Tp_Class,
            Tp_Struct,
            Tp_Enum,
            Tp_Assembly,
            Tp_Interface,
            Tp_Delegate,
            Tp_Method,
            Tp_Property
        }

        public enum ImplementationAcessibility
        {
            Ac_Public,
            Ac_Private,
            Ac_PrivateProtected,
            Ac_Protected,
            Ac_Internal,
            Ac_ProtectedInternal
        }

        public ImplementationType ImplementationTp { get;  set; }

        public string Name { get;  set; }

        public string Parent { get;  set; }

        public ImplementationAcessibility ImplementationAc { get;  set; }

        public Implementation()
        {
        }
    }
}
