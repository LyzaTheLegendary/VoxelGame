using Resources;

public class Resource<T> : IResource<T> where T : IResourceFactory
{
    public string Filename { get; init; }
    public FileType Type { get; init; }
    public ushort Version { get; init; }
    public int Flags { get; init; }

    private Action<byte[], bool> returnAction;
    private IEnumerable<byte> data;

    protected bool disposedValue = false;
    public Resource(IEnumerable<byte> data, Action<byte[], bool> returnAction)
    {
        this.data = data;
        this.returnAction = returnAction;
    }
    ~Resource() => Dispose(false);
    
    public T GetComponent()
    {
        if (disposedValue)
            throw new ObjectDisposedException(nameof(Resource<T>));

        T resource = Activator.CreateInstance<T>();
        resource.CreateResourceFromData(data);

        return resource;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {

            // We return the data back to a ArrayPool here generally,
            // so that's why the boolean exists as we can choose whether we should OR shouldn't null the data
            if (disposing)
                returnAction(data as byte[] ?? data.ToArray(), false);

            disposedValue = true;
            data = null!;
        }
        
    }
}
