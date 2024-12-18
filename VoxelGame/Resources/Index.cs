using System.Collections.Concurrent;
using Utils;

namespace Resources
{
    public class Index : IComponent
    {
        private int files = 0;
        private ConcurrentDictionary<FileType, List<string>> fileIndex = new();
        public Index() { 
           
        }

        public bool HasReference(FileType type, string filename)
        {
            fileIndex.TryGetValue(type, out List<string>? list);

            bool? result = list?.Contains(filename);
            return (result == null) ? false : result!.Value;
        }
        public void AddReference(FileType type, string filename)
        {
            if (!fileIndex.TryGetValue(type, out _))
                fileIndex[type] = new List<string>();

            fileIndex[type].Add(filename);
        }

        public void RemoveReference(FileType type, string filename)
        {
            if (!fileIndex.TryGetValue(type, out _))
                return;

            fileIndex[type].Remove(filename);
        }
        public ConcurrentDictionary<FileType, List<string>> GetIndex() => fileIndex;
        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            using (MemoryStream ms = new MemoryStream(data as byte[] ?? data.ToArray()))
            {
                files = ms.Read<int>();

                for (int i = 0; i < files; i++)
                {
                    FileType type = (FileType)ms.Read<int>();
                    string filename = ms.ReadString();

                    if(!fileIndex.TryGetValue(type, out _))
                        fileIndex[type] = new List<string>();

                    fileIndex[type].Add(filename);
                }
            }
        }
    }
}
