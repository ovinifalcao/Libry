using System.Collections.Generic;

namespace Libry
{ 
    class Tp_Assembly : Tp_Containers
    {

        public int Version { get; set; }

        public string Company { get; set; }

        public Implementation_Description Description { get; set; }

        public Tp_Assembly()
        {
        }
    }
}
