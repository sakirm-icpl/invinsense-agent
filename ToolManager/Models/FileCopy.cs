namespace ToolManager.Models
{

    public class FileCopy
    {
        public string Source { get; set; }

        public string Destination { get; set; }

        public FileExistsAction FileExistsAction { get; set; }

        public bool ExtractIfPossible { get; set; }

        public bool RemoveSource { get; set; }

    }
}