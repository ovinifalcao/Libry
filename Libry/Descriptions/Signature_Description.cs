namespace Libry
{
    class Signature_Description
    {

        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsOptional { get; private set; }

        public Signature_Description(string name, string description, bool isOptional = false)
        {
            Name = name;
            Description = description;
            IsOptional = isOptional;
        }

        public Signature_Description()
        { }
    }
}
