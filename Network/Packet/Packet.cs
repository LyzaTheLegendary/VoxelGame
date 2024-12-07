namespace Network.Packet
{
    public abstract class Packet
    {
        public virtual void Serialize() => throw new NotImplementedException();
        public virtual void Deserialize(Stream stream) => throw new NotImplementedException();
        public virtual byte[] GetData() => throw new NotImplementedException();
    }
}
