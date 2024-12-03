namespace Resources.Creators
{
    public interface ICreatorService
    {
        public string Filename { get; init; }
        public FileType FileType { get; }
        public IEnumerable<byte> GetResource();
    }
}