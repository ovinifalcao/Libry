using System;
using Newtonsoft.Json;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace Libry
{
    class Program
    {
        static void Main(string[] args)
        {

            //var json = JsonConvert.SerializeObject(Cls_Teste.MeRetorna());
            //var me = Encoding.ASCII.GetBytes(" ");
            //Console.WriteLine(me[0]);
            var sw = new Stopwatch();
            sw.Start();
            var test = new FileAnalysis(@"C:\temp\LibryTeste");
            sw.Stop();

            foreach (ModelAnalisys St in test.IdentifiedStructures)
            {
                Console.WriteLine(St.FirstLine);
            }
            Console.WriteLine("Time it took: {0}", sw.ElapsedMilliseconds);
            Console.WriteLine("Amount of lines: {0}", test.NamedStructures.Count());

            Console.Read();

        }
    }
}
