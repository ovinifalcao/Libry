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

        public TextScan(string projectPath)
        {
            ProjectPath = projectPath;
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

                    if (LookingClasses(ln))
                    {
                        var ClassBlock = ReturnBlockCode(ln, sr);
                        //SepareCodeBlocks(ClassBlock);
                    }

                }
            }
        }

        private bool LookingClasses(string FileLine)
        {
            if (Searching(FileLine, new List<string>(){"class", "interface"}))
            {
                return true;
            }

            return false;
        }


        private List<string> ReturnBlockCode(string FileLine, StreamReader sr, int InitBrackets = 0)
        {
            List<string> Block = new List<string>();
            bool BlockComplete = false;
            string BlockDeterminer = null;
            int EndBrackets = 0;

            do
            {
                while (string.IsNullOrEmpty(BlockDeterminer))
                {
                    var BlTm = FindBlockTerminator(FileLine);
                    if (string.IsNullOrEmpty(BlTm))
                    {
                        FileLine = sr.ReadLine();
                    }
                    else
                    {
                        BlockDeterminer = BlTm;
                    }
                }

                if (BlockDeterminer == ";") { return Block; }
                if (!String.IsNullOrEmpty(FileLine) && FileLine.Contains("{")) { InitBrackets++; }
                if (!String.IsNullOrEmpty(FileLine) && FileLine.Contains("}")) { EndBrackets++; }
                if (InitBrackets != 0 && InitBrackets == EndBrackets)
                {
                    BlockComplete = true;
                }
                FileLine = sr.ReadLine();
                Block.Add(AvoidCommentary(FileLine));
            }
            while (!BlockComplete);

            return RemoveLastBracket(Block);
        }

        private ModelAnalisys ReturnBlockCode(List<string> CompleteClass, int BegginIndex, int InitBrackets = 0)
        {
            int EndBrackets = 0;

            var MdCodeBl = new ModelAnalisys()
            {
                CodeBlock = CompleteClass[BegginIndex],
                FirstLine = CompleteClass[BegginIndex]
            };

            string
                BlockTerminator = null,
                CurrentLine = null;

            bool BlockComplete = false;

            do
            {
                while (string.IsNullOrEmpty(BlockTerminator))
                {
                    string BlTm = null;
                    if (!String.IsNullOrEmpty(CurrentLine)) { BlTm = FindBlockTerminator(CurrentLine); };
                    if (string.IsNullOrEmpty(BlTm))
                    {
                        BegginIndex++;
                        CurrentLine = CompleteClass[BegginIndex];
                        MdCodeBl.CodeBlock += CompleteClass[BegginIndex];
                        MdCodeBl.TerminatorLine = BegginIndex;
                    }
                    else
                    {
                        BlockTerminator = BlTm;
                    }
                }

                if (BlockTerminator == ";")
                {
                    MdCodeBl.BlockEnd = BegginIndex;
                    return MdCodeBl;
                }

                if (!String.IsNullOrEmpty(CurrentLine) && CurrentLine.Contains("{")) { InitBrackets++; }
                if (!String.IsNullOrEmpty(CurrentLine) && CurrentLine.Contains("}")) { EndBrackets++; }
                if (InitBrackets != 0 && InitBrackets == EndBrackets)
                {
                    BlockComplete = true;
                }
                BegginIndex++;
                CurrentLine = CompleteClass[BegginIndex];
                MdCodeBl.CodeBlock += CompleteClass[BegginIndex];
            }
            while (!BlockComplete);

            MdCodeBl.BlockEnd = BegginIndex;
            return MdCodeBl;
        }



        private void Recognizing(ModelAnalisys MdBlock)
        {


        }

        private bool TryMethodByWords(string CodeBlock, List<string> OnlyMethods = null, string FirstLine = null)
        {
            if (string.IsNullOrEmpty(FirstLine)) { FirstLine = CodeBlock; }

            // Consider partial as a reserved word for methods because it did not have the identifier "class"
            if ((OnlyMethods == null))
            {
                OnlyMethods = new List<string>()
                {
                    "void",
                    "abstract",
                    "partial",
                    "virtual"
                };
            }


            if (Searching(FirstLine, OnlyMethods))
            {
                Occurences.Add(FirstLine);
                return true;
            }

            return false;
        }



        private string FindBlockTerminator(string FileLine)
        {
            if (FileLine.Contains(";") && !FileLine.Contains("{"))
            {
                return ";";
            }
            else if (!FileLine.Contains(";") && FileLine.Contains("{"))
            {
                return "{";
            }
            else if (FileLine.Contains(";") && FileLine.Contains("{"))
            {
                if (FileLine.IndexOf(";") < FileLine.IndexOf("{"))
                {
                    return ";";
                }
                else
                {
                    return "{";
                }
            }
            else
            {
                return null;
            }
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

        private string NextValidLine(List<string> Txt, int Index)
        {
            while (!(String.IsNullOrEmpty(Txt[Index]) == (Index == Txt.Count)))
            {
                Index++;
            }
            return Txt[Index];
        }

        private List<string> RemoveLastBracket(List<string> CompleteClass)
        {
            byte BracketsCounter = 0;
            int i = CompleteClass.Count -1;
            while (BracketsCounter < 1) 
            {
                if (CompleteClass[i].Contains("}"))
                {
                    CompleteClass.RemoveAt(i);
                    BracketsCounter++;
                    i--;
                }
            }

            return CompleteClass;
        }

        private void RemoveIdendifiedOccurrence(List<string> CompleteClass, int BegginIndex, int EndIndex)
        {
            while (BegginIndex <= EndIndex)
            {
                CompleteClass[BegginIndex] = "";
                BegginIndex++;
            }
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
