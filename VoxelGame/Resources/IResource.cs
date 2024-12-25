namespace Resources
{
    public enum FileType : int
    {
        SHAPE,
        MODEL,
        TRANSLATION,
        TEXTURE,
        TEXTURE_ATLAS,
        SHADER,
        INDEX,
    }
    public interface IResource<T> : IDisposable where T:IComponent
    {
        public string Filename { get; init; }
        public FileType Type { get; init; }
        public ushort Version { get; init; }
        public int Flags { get; init; }

        public T GetComponent();
    }
}
