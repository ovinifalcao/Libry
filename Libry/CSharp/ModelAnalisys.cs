
namespace Libry
{
    class ModelAnalisys
    {
        public enum AnalysisTypes
        {
            NamedStructures,
            Methods,
            Properties
        }

        public string CodeBlock { get; set; }
        public string FirstLine { get; set; }
        public int TerminatorLine { get; set; }
        public int BlockBegin { get; set; }
        public int BlockEnd { get; set; }
        public string BlockTerminator { get; set; }
        public AnalysisTypes BlockType { get; set; }

    }
}
