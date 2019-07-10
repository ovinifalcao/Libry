using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libry
{
    class FileAnalysis
    {

        public string ProjectPath { get; set; }
        public string[] ScanFiles { get; set; }
        public List<string> File { get; set; } = new List<string>();
        public List<ModelAnalisys> NamedStructures { get; set; } = new List<ModelAnalisys>();
        public List<ModelAnalisys> IdentifiedStructures { get; set; } = new List<ModelAnalisys>();

        public FileAnalysis(string ProjectMainPath)
        {
            this.ProjectPath = ProjectMainPath;
            GetFilesToScan();
            ReadingEachFile();
        }

        private void GetFilesToScan()
        {
            var AllDirectoryFiles = Directory.GetFiles(ProjectPath, "*", SearchOption.AllDirectories);

            ScanFiles = (from Sort in AllDirectoryFiles
                           where Path.GetExtension(Sort) is ".cs"
                           select Sort).ToArray();
        }
        private void ReadingEachFile()
        {
            foreach (string Fl in ScanFiles)
            {
                ReadingEachLine(Fl);
                StructureFilters();
                TakeOf();
                RemainingStructure();
                File = new List<string>();
                IdentifiedStructures.AddRange(NamedStructures);
                NamedStructures = new List<ModelAnalisys>();
            }
        }

        private void ReadingEachLine(string FileName)
        {
            using (var sr = new StreamReader(FileName))
            {
                string Line;
                while ((Line = sr.ReadLine()) != null)
                {
                    Line = AvoidCommentary(Line);
                    if ((!string.IsNullOrEmpty(Line)) && !string.IsNullOrWhiteSpace(Line))
                    {
                        File.Add(Line);
                    }

                }
            }
        }
        private string AvoidCommentary(string Line)
        {
            if (Line.Replace(" ", "").IndexOf("///") == 0) { return ""; }
            if (Line.Contains("//")){
                return Line.Remove(Line.IndexOf("//")); }
            return Line;
        }

        private void StructureFilters()
        {
            for (int IndexCounter = 0; IndexCounter < File.Count - 1; IndexCounter++)
            {
                if (GetByReservedWords(File[IndexCounter], Dicionary.MainStructures))
                {
                    var Md = ReturnBlockCode(IndexCounter);
                    Md.BlockType = ModelAnalisys.AnalysisTypes.NamedStructures;
                    NamedStructures.Add(Md);
                }
                else if (GetByReservedWords(File[IndexCounter], Dicionary.MethodsCommonStructures))
                {
                    var Md = ReturnBlockCode(IndexCounter);
                    Md.BlockType = ModelAnalisys.AnalysisTypes.Methods;
                    NamedStructures.Add(Md);
                    IndexCounter = Md.BlockEnd;
                }
                else if (GetByReservedWords(File[IndexCounter], Dicionary.PropertiesCommonStructures) && File[IndexCounter].Contains(";"))
                {
                    var Md = ReturnBlockCode(IndexCounter);
                    Md.BlockType = ModelAnalisys.AnalysisTypes.Properties;
                    NamedStructures.Add(Md);
                    IndexCounter = Md.BlockEnd;
                }
                else if (GetByReservedWords(File[IndexCounter], Dicionary.PrimitiveTypes))
                {
                    var SummaryModel = ReturnBlockCode(IndexCounter);
                    if (SummaryModel.BlockTerminator == "{" && !GetByReservedWords(SummaryModel.CodeBlock, Dicionary.PropertiesCommonStructures))
                    {
                        SummaryModel.BlockType = ModelAnalisys.AnalysisTypes.Methods;
                        NamedStructures.Add(SummaryModel);
                    };
                    IndexCounter = SummaryModel.BlockEnd;
                }
            }
        }

        private void RemainingStructure()
        {
            for (int IndexCounter = 0; IndexCounter < File.Count - 1; IndexCounter++)
            {
                if (GetByReservedWords(File[IndexCounter], Dicionary.MainStructures))
                {
                    var Md = ReturnBlockCode(IndexCounter);
                    Md.BlockType = ModelAnalisys.AnalysisTypes.NamedStructures;
                    NamedStructures.Add(Md);
                    IndexCounter = Md.TerminatorLine;
                }
                else 
                {
                    if (NextValidLine(File, IndexCounter).Contains("{"))
                    {
                        var MdInProgress = ReturnBlockCode(IndexCounter - 1);
                        //if(MdInProgress.CodeBlock.Contains)

                        //if (GetByReservedWords(File[IndexCounter], new List<string>() { "get" }))
                        //{
                        //    var Md = ReturnBlockCode(IndexCounter);
                        //    Md.BlockType = ModelAnalisys.AnalysisTypes.Methods;
                        //    NamedStructures.Add(Md);
                        //    IndexCounter = Md.BlockEnd;
                        //}
                        IndexCounter = MdInProgress.BlockEnd;
                    }
                }

            }
        }

        private string NextValidLine(List<string> Txt, int Index)
        {
            while (!(String.IsNullOrEmpty(Txt[Index]) == (Index == Txt.Count)))
            {
                Index++;
            }
            return Txt[Index];
        }

        private void TakeOf()
        {
            for (int i = 0; i <  NamedStructures.Count -1; i++)
            {
                if (NamedStructures[i].BlockType != ModelAnalisys.AnalysisTypes.NamedStructures)
                {
                    int RemoveCount = NamedStructures[i].BlockBegin;
                    while (RemoveCount <= NamedStructures[i].BlockEnd)
                    {
                        File[RemoveCount] = "";
                        RemoveCount++;
                    }
                }
            }
            File.RemoveAll(St => String.IsNullOrEmpty(St));
        }

        private bool GetByReservedWords(string FileLine, List<string> ReservedWords)
        {

            if (Searching(FileLine, ReservedWords))
            {
                return true;
            }

            return false;
        }

        private ModelAnalisys ReturnBlockCode(int BegginIndex, string BlockTerminator = null)
        {
            int InitBrackets= 0, EndBrackets = 0;

            var MdCodeBl = new ModelAnalisys()
            {
                CodeBlock = File[BegginIndex],
                FirstLine = File[BegginIndex],
                TerminatorLine = BegginIndex,
                BlockBegin = BegginIndex
            };

            DecideBlockTerminator(MdCodeBl);
            string CurrentLine = MdCodeBl.FirstLine;
            BegginIndex = MdCodeBl.TerminatorLine;
            bool BlockComplete = false;

            if(MdCodeBl.BlockTerminator == "{")
            {
                InitBrackets++;
            }
            else
            {
                MdCodeBl.BlockEnd = BegginIndex;
                return MdCodeBl;
            }

            do
            {
                if (!String.IsNullOrEmpty(CurrentLine) && CurrentLine.Contains("{")) { InitBrackets++; }
                if (!String.IsNullOrEmpty(CurrentLine) && CurrentLine.Contains("}")) { EndBrackets++; }
                if (InitBrackets != 0 && InitBrackets == EndBrackets)
                {
                    BlockComplete = true;
                }
                else
                {
                    BegginIndex++;
                }
                CurrentLine = File[BegginIndex];
                MdCodeBl.CodeBlock += File[BegginIndex];
            }
            while (!BlockComplete);

            MdCodeBl.BlockEnd = BegginIndex;
            return MdCodeBl;
        }

        private void DecideBlockTerminator(ModelAnalisys MdAnalysis)
        {
            string CurrentLine = MdAnalysis.FirstLine;
            while (string.IsNullOrEmpty(MdAnalysis.BlockTerminator))
            {
                if (!String.IsNullOrEmpty(CurrentLine)) { MdAnalysis.BlockTerminator = FindBlockTerminator(CurrentLine); };
                if (string.IsNullOrEmpty(MdAnalysis.BlockTerminator))
                {
                    MdAnalysis.TerminatorLine ++;
                    CurrentLine = File[MdAnalysis.TerminatorLine];
                    MdAnalysis.CodeBlock += File[MdAnalysis.TerminatorLine];
                }
            }
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
