using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Libry
{
    class TextScan
    {
        public string ProjectPath { get; set; }

        public List<string> Occurences { get; private set; } = new List<string>();

        private string[] FilesToScan;

        private Dicio LanguageDictionary;


        public TextScan(string projectPath)
        {
            ProjectPath = projectPath;
            GetLanguageDictionary();
            GetFilesToScan();
            ReadingEachFile();
        }



        private void GetFilesToScan()
        {
            var AllDirectoryFiles = Directory.GetFiles(ProjectPath, "*", SearchOption.AllDirectories);

            FilesToScan = (from Sort in AllDirectoryFiles
                          where Path.GetExtension(Sort) is ".cs"
                          select Sort).ToArray();
        }

        private void ReadingEachFile()
        {
            foreach(string Fl in FilesToScan)
            {
                ReadingEachLine(Fl);
            }
        }

        private void ReadingEachLine(string FileName)
        {
            using (var sr = new StreamReader(FileName))
            {
                string ln;
                while ((ln = sr.ReadLine()) != null)
                {

                    ln = AvoidCommentary(ln);
                    bool IsNamedStructure = (GetNamedStructures(ln));
                    bool IsKnownType = Searching(ln, LanguageDictionary.PrimitiveTypes);
                    bool IsContextualCaracter = Searching(ln, new List<string>() { "{", "}" });

                    string FirstLine = ln;
                    if (!IsNamedStructure && IsKnownType && !String.IsNullOrEmpty(ln) && !MethodReservedWords(ln) && !IsContextualCaracter)
                    {
                        if (MethodReservedWords(BlockByBracketsPairs(ln, sr),
                                                new List<string>() { "return" }))
                        {
                            AddToOccurencesList(FirstLine);
                        }

                    }
                    else if (!IsNamedStructure && !IsKnownType && !String.IsNullOrEmpty(ln) && !IsContextualCaracter)
                    {
                        if (ln.Contains(";")) { continue; }

                        var LineArray = ln.Split(' ');

                        if (LineArray.Count() > 2)
                        {
                            var NextLine = NextValidLine(sr);
                            if (!String.IsNullOrEmpty(NextLine) && (ln.Contains("{") || NextLine.Contains("{")))
                            {
                                var Blk = BlockByBracketsPairs(FirstLine, sr, 1);
                                if (Searching(Blk, new List<string>() { "return" })) { AddToOccurencesList(FirstLine); };
                            }
                        }
                    }
                    else if (MethodReservedWords(ln) && !IsContextualCaracter)
                    {
                        AddToOccurencesList(FirstLine);
                    }
                }
            }
        }

        private string NextValidLine(StreamReader sr)
        {
            string TxtLine = null;
            while(!(String.IsNullOrEmpty(TxtLine) == sr.EndOfStream))
            {
                TxtLine = sr.ReadLine();
            }
            return TxtLine;
        }

        private string BlockByBracketsPairs(string FileLine, StreamReader sr, int InitBrackets = 0)
        {
            string Block = null;
            int EndBrackets = 0;
            bool BlockComplete = false;

            do
            {
                Block += FileLine;
                if (!String.IsNullOrEmpty(FileLine) && FileLine.Contains("{")) { InitBrackets++; }
                if (!String.IsNullOrEmpty(FileLine) && FileLine.Contains("}")) { EndBrackets++; }
                if (InitBrackets != 0 && InitBrackets == EndBrackets)
                {
                    BlockComplete = true;
                }
                FileLine = sr.ReadLine();
            }
            while (!BlockComplete);

            return Block;
        }

        private string AvoidCommentary(string FileLine)
        {
            if (FileLine.Replace(" ", "").IndexOf("///") == 0) { return ""; }
            if (FileLine.Contains("//"))
            {
                return FileLine.Remove(FileLine.IndexOf("//"));
            }

            return FileLine;
        }

        private bool MethodReservedWords(string FileLine, List<string> OnlyMethods = null)
        {
            // Consider partial as a word reserved for methods because it did not have the identifier "class"
            if ((OnlyMethods == null))
            {
                OnlyMethods = new List<string>()
                {
                    "void",
                    "abstract",
                    "partial"
                };
            }

            if (Searching(FileLine, OnlyMethods))
            {
                return true;
            }

            return false;
        }

        private void GetLanguageDictionary()
        {
            var Dic = new Dicio()
            {
                 MainStructures = new List<string>
                 {
                    "class",
                    "struct",
                    "enum",
                    "namespace",
                    "interface",
                    "delegate",
                    "(){",
                    "property",
                 },

                 AcessModifiers = new List<string>()
                 {
                    "public",
                    "protected",
                    "internal",
                    "protected internal",
                    "private",
                    "private protected"
                 },


                 //YES I KNOW STRING IS NOT A PRIMITIVE TYPE
                PrimitiveTypes = new List<string>()
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
                 },

            };

            LanguageDictionary = Dic;

        }

        private bool GetNamedStructures(string FileLine)
        {
            if(Searching(FileLine, LanguageDictionary.MainStructures))
            {
                AddToOccurencesList(FileLine);
                return true;
            }

            return false;
        }

        private void AddToOccurencesList(string FileLine)
        {
            FileLine = FileLine.TrimStart(' ').TrimEnd(' ');
            Occurences.Add(FileLine);
        }

        private bool Searching(string FileLine, List<string> DicionaryStructure)
        {
            foreach (string Ns in DicionaryStructure)
            {
                if (FileLine.Contains(Ns))
                {
                    var OccurrencyIndex = FileLine.IndexOf(Ns);
                    var LineArray = FileLine.ToArray();

                    bool Begining = true, Ending = true;

                    if (OccurrencyIndex != 0)
                    {
                        Begining = false;

                        if (Encoding.ASCII.GetBytes(LineArray[OccurrencyIndex - 1].ToString())[0] < 40)
                        {
                            Begining = true;
                        }
                    }

                    if (OccurrencyIndex + (Ns).Length != LineArray.Length)
                    {
                        Ending = false;

                        if (Encoding.ASCII.GetBytes(LineArray[OccurrencyIndex + (Ns).Length].ToString())[0] < 40)
                        {
                            Ending = true;
                        }
                    }

                    if (Begining && Ending)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
