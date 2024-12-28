using Graphics.GpuMemory;
using Graphics.GpuTextures;
using OpenTK.Graphics.OpenGL4;
using Buffer = OpenTK.Graphics.OpenGL4.Buffer;

namespace Graphics;

public static class TextureManager
{
    private static List<Texture2D> textures = new();
    private static long[] liveIndex = new long[100];
    private static GpuShaderStorageBuffer<long> textureHandleBuffer;
    private static Queue<int> unusedIndexes = new Queue<int>();
    private static int highestIndex = 0;

    public static int TextureHeapSize { get; private set; }

    public static void Init()
    {
        textureHandleBuffer = GraphicsDevice.AllocateShaderBuffer<long>(100, BufferUsageHint.StaticRead, BindingPoints.TEXTURE_HANDLES);
    }

    // Returns the handle of the texture in the texture manager.
    public static void AddTexture(Texture2D texture)
    {
        if (texture.Handle == 0)
            texture.MakeResident();

        textures.Add(texture);

        if (highestIndex >= liveIndex.Length)
            ResizeBuffers(liveIndex.Length * 2);

        int index = unusedIndexes.Count > 0 ? unusedIndexes.Dequeue() : highestIndex++;

        liveIndex[index] = texture.Handle;
        
        textureHandleBuffer.Memset(index, new long[] { texture.Handle }, 1);

        TextureHeapSize += texture.Width * texture.Height;
    }

    public static Texture2D GetTexture(int index) => textures[index];
    public static void UnloadTexture(int index)
    {
        Texture2D texture = textures[index];
        
        textures.Remove(texture);
        
        liveIndex[index] = 0;
        unusedIndexes.Enqueue(index);
        
        texture.MakeNonResident();
        texture.Dispose();
        
        TextureHeapSize -= texture.Width * texture.Height;
    }
    private static void ResizeBuffers(int length)
    {
        if(liveIndex.Length > length)
            throw new Exception("Cannot resize texture Heap");
        
        long[] newBuffer = new long[length];
        
        System.Buffer.BlockCopy(liveIndex, 0, newBuffer, 0, liveIndex.Length * sizeof(long));
        
        textureHandleBuffer.Upload(newBuffer, BufferUsageHint.StaticRead);
        liveIndex = newBuffer;
    }
}