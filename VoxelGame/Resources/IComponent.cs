namespace Resources
{
    public interface IComponent
    {
        public void CreateResourceFromData(IEnumerable<byte> data);
    }
}
