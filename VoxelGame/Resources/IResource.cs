namespace Resources
{
    public enum FileType : int
    {
        SHAPE,
        TRANSLATION,
        IMAGE,
        SHADER,
        INDEX,
    }
    public interface IResource<T> : IDisposable where T:IResourceFactory
    {
        public string Filename { get; init; }
        public FileType Type { get; init; }
        public ushort Version { get; init; }
        public int Flags { get; init; }

        public T GetComponent();
    }
}
