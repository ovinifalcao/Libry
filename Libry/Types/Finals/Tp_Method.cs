using System.Collections.Generic;

namespace Libry
{
    class Tp_Method : Implementation
    {

        public bool HaveReturn { get;  set; }
        public bool IsStatic { get;  set; }
        public bool IsOverridable { get;  set; }
        public string TypeOfReturn { get;  set; }
        public List<Signature_Description> Param { get; private set; }

    }
}
