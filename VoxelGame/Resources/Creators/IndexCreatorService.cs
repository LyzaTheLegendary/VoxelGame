
using Utils;

namespace Resources.Creators
{
    public class IndexCreatorService : ICreatorService
    {
        public string Filename { get; init; }
        public FileType FileType { get { return FileType.INDEX; } }
        private IEnumerable<byte> data;
        public IndexCreatorService(Index index)
        {
            Filename = "index";
            int totalFiles = index.GetIndex().Sum(entry => entry.Value.Count);

            List<byte> data = new();
            data.Write(totalFiles);

            foreach (var entry in index.GetIndex())
            {
                data.Write((int)entry.Key);
                data.Write(Filename);
            }

            this.data = data;
        }

        public IEnumerable<byte> GetResource() => this.data.ToArray();
    }
}
