using System.Collections.Generic;

namespace Libry
{
    class Tp_Class : Tp_Containers
    {

        public enum ClassType
        {
            ClTp_Class,
            ClTp_Abstract,
            ClTp_Partial
        }

        public ClassType TpClass { get; set; }

        public Tp_Class()
        {
        }
    }
}
