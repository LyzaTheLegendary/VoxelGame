using Content;
using Resources.Creators;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using Utils;
using VoxelGame.Utils;
using Voxels;

namespace Resources
{
    public class Storage // create a file thread
    {
        private ArrayPool<byte> arrPool = ArrayPool<byte>.Shared;
        const string RESOURCE_PATH = "./Resource";

        private Index index;
        public Storage()
        {
            index = new Index();
        }
        public void StoreResource(ICreatorService creator) // this could be done by a thread
        {
            FileHeader header = new FileHeader()
            {
                Type = creator.FileType,
                Version = 1,
                Flags = 0
            };

            IEnumerable<byte> data = MarshalHelper.ToBytes(header).Concat(creator.GetResource());

            index.AddReference(creator.FileType, creator.Filename);
            File.WriteAllBytes(Path.Combine(RESOURCE_PATH, Path.Combine(creator.Filename)), data as byte[] ?? data.ToArray());
        }
        public Resource<T> GetResource<T>(string filename) where T : IComponent
        {
            using (FileStream fs = File.Open(Path.Combine(RESOURCE_PATH, filename), FileMode.Open))
            {
                byte[] data = arrPool.Rent((int)fs.Length - Marshal.SizeOf<FileHeader>()); ;

                FileHeader header;
                try
                {
                    header = fs.Read<FileHeader>();
                    fs.ReadExactly(data, 0, (int)(fs.Length - fs.Position));
                }
                catch (Exception)
                {
                    arrPool.Return(data, false);
                    throw;
                }

                return new Resource<T>(data, arrPool.Return)
                {
                    Filename = filename,
                    Type = header.Type,
                    Version = header.Version,
                    Flags = header.Flags
                };
            }
        }
        public bool Exist(string filename)
            => File.Exists(Path.Combine(RESOURCE_PATH, filename));
        public void Remove(string filename)
            => File.Delete(Path.Combine(RESOURCE_PATH, filename));
        public void Save() => StoreResource(new IndexCreatorService(index));
        public void Load()
        {
            string path = Path.Combine(RESOURCE_PATH, "index");

            if (Exist(path))
                using (Resource<Index> resource = GetResource<Index>(path))
                    index = resource.GetComponent();

            foreach (var entry in index.GetIndex())
            {
                if (entry.Key == FileType.SHAPE)
                {
                    foreach (string filename in entry.Value)
                        using (Resource<Shape> shapeResource = GetResource<Shape>(filename))
                        {
                            Shape shape = shapeResource.GetComponent();
                            GameContent.RegisterShape(shape.Name, shape);
                        }
                }
                else if(entry.Key == FileType.TRANSLATION)
                {
                    string filename = entry.Value.First();
                    // TODO: implement translation file
                }
            }
        }
    }
}
