using Graphics;
using Graphics.GpuComputing;
using OpenTK.Mathematics;
using Resources;
using Resources.Creators;
using System.Runtime.InteropServices;
using VoxelGame;

internal class Program
{
    static public void Main() // requirement check if openGL is version 4.5 atleast
    {
        /*
         TODOS:
            1. Tokenize the GSLS language.
            2. A shader should know certain things like what SSBO it's bound to.
            3. The variables it has.
            4. Make sure that the VAO is the same structure as the variables
            5. Look into rastorization shaders
         */
        

        Application application = new Application();
        Storage storage = application.Storage;
        AddImagetoStorage(storage);
        AddCubeToStorage(storage);
        AddShaderToStorage(storage);

        application.Run();
    }

    public static void AddImagetoStorage(Storage storage)
    {
        byte[] image = File.ReadAllBytes("BlockAtlas.png");
        storage.StoreResource(new BitmapCreatorService("Textures/BlockAtlas.bitmap", image, true, 16, 16));
    }
    public static  void AddShaderToStorage(Storage storage)
    {
        string vertexShader = File.ReadAllText("test.vert");
        string fragmentShader = File.ReadAllText("test.frag");

        ShaderCreatorData[] data = new ShaderCreatorData[2];

        data[0] = new ShaderCreatorData()
        {
            ShaderCode = vertexShader,
            Type = OpenTK.Graphics.OpenGL4.ShaderType.VertexShader
        };

        data[1] = new ShaderCreatorData()
        {
            ShaderCode = fragmentShader,
            Type = OpenTK.Graphics.OpenGL4.ShaderType.FragmentShader
        };


        storage.StoreResource(new ShaderCreatorService("Shaders/test.shaders", data));
    }
    public static void AddCubeToStorage(Storage storage)
    {
        List<Vertex> vertices = new List<Vertex>
        {
            // Front face (mapped to texture coordinates (0,0) to (1/128, 1/128))
            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(0f, 1f)),
            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1f, 1f)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1f, 0f)),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(0f, 0f)),

            // Right face (mapped to texture coordinates (1/128, 0) to (2/128, 1/128))
            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1f, 1f)),
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(2f, 1f)),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(2f, 0f)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1f, 0f)),

            // Back face (mapped to texture coordinates (2/128, 0) to (3/128, 1/128))
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(2f, 1f)),
            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(3f, 1f)),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(3f, 0f)),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(2f, 0f)),

            // Left face (mapped to texture coordinates (3/128, 0) to (4/128, 1/128))
            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(3f, 1f)),
            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(4f, 1f)),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(4f, 0f)),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(3f, 0f)),

            // Top face (mapped to texture coordinates (4/128, 0) to (5/128, 1/128))
            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(4f, 1f)),
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(5f, 1f)),
            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(5f, 0f)),
            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(4f, 0f)),

            // Bottom face (mapped to texture coordinates (5/128, 0) to (6/128, 1/128))
            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(5f, 1f)),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(6f, 1f)),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(6f, 0f)),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(5f, 0f))
        };

        uint[] cubeIndices =
        {
            // first face
            // top triangle
            0, 1, 2,
            // bottom triangle
            2, 3, 0,

            4, 5, 6,
            6, 7, 4,

            8, 9, 10,
            10, 11, 8,

            12, 13, 14,
            14, 15, 12,

            16, 17, 18,
            18, 19, 16,

            20, 21, 22,
            22, 23, 20
        };


        storage.StoreResource(new ShapeCreatorService("Shapes/cube.shape", "cube", vertices.ToArray(), cubeIndices));
    }

    public static Vector2[] GenerateUVs(int row, int col)
    {
        float cellSize = 1.0f / 16.0f; // Adjust for your grid size
        float xMin = col * cellSize;
        float xMax = xMin + cellSize;
        float yMin = row * cellSize;
        float yMax = yMin + cellSize;

        return new Vector2[]
        {
        new Vector2(xMin, yMax), // Top-left
        new Vector2(xMax, yMax), // Top-right
        new Vector2(xMax, yMin), // Bottom-right
        new Vector2(xMin, yMin), // Bottom-left
        };
    }
}