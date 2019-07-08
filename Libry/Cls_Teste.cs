using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libry
{
    class Cls_Teste
    {

        public static Implementation MeRetorna()
        {
            var EuMesmo = new Tp_Assembly()
            {
                Company = "Ark",
                Version = 100,
                ImplementationAc = Implementation.ImplementationAcessibility.Ac_Public,
                ImplementationTp = Implementation.ImplementationType.Tp_Assembly,
                Name = "Libry",
                Parent = null,
                Description = new Implementation_Description()
                {
                    Remarks = "Isso mesmo rapariga"
                },
                Members = new System.Collections.Generic.List<Implementation>()
                {
                    new Tp_Class()
                    {
                        ImplementationAc = Implementation.ImplementationAcessibility.Ac_Public,
                        ImplementationTp = Implementation.ImplementationType.Tp_Class,
                        Name = "Tp_Class",
                        Parent = "Libry",
                        TpClass = Tp_Class.ClassType.ClTp_Class,
                        Members = new System.Collections.Generic.List<Implementation>()
                        {
                            new Tp_Enum()
                            {
                                ImplementationAc = Implementation.ImplementationAcessibility.Ac_Public,
                                ImplementationTp = Implementation.ImplementationType.Tp_Enum,
                                Name = "ClassType",
                                Parent = "Tp_Class",
                                Members = new System.Collections.Generic.List<Enum_Description>()
                                {

                                    new Enum_Description()
                                    {
                                        EnumerationIndex =1,
                                        EnumerationName = "ClTp_Class"
                                    },

                                    new Enum_Description()
                                    {
                                        EnumerationIndex =2,
                                        EnumerationName = "ClTp_Abstract"
                                    },

                                    new Enum_Description()
                                    {
                                        EnumerationIndex =3,
                                        EnumerationName = "ClTp_Partial"
                                    }

                                }
                            }
                        }
                    }
                }

            };

            return EuMesmo;
        }
    }
}
