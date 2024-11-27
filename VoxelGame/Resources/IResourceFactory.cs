namespace Resources
{
    public interface IResourceFactory
    {
        public void CreateResourceFromData(IEnumerable<byte> data);
    }
}
