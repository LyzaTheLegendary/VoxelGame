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
        AddCubeToStorage(storage);
        AddShaderToStorage(storage);

        application.Run();
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
        Vector3[] cubeVerts = new Vector3[]
{
            new Vector3(-0.5f, 0.5f, 0.5f), // topleft vert
            new Vector3(0.5f, 0.5f, 0.5f), // topright vert
            new Vector3(0.5f, -0.5f, 0.5f), // bottomright vert
            new Vector3(-0.5f, -0.5f, 0.5f), // bottomleft vert
            // right face
            new Vector3(0.5f, 0.5f, 0.5f), // topleft vert
            new Vector3(0.5f, 0.5f, -0.5f), // topright vert
            new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
            new Vector3(0.5f, -0.5f, 0.5f), // bottomleft vert
            // back face
            new Vector3(0.5f, 0.5f, -0.5f), // topleft vert
            new Vector3(-0.5f, 0.5f, -0.5f), // topright vert
            new Vector3(-0.5f, -0.5f, -0.5f), // bottomright vert
            new Vector3(0.5f, -0.5f, -0.5f), // bottomleft vert
            // left face
            new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
            new Vector3(-0.5f, 0.5f, 0.5f), // topright vert
            new Vector3(-0.5f, -0.5f, 0.5f), // bottomright vert
            new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
            // top face
            new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
            new Vector3(0.5f, 0.5f, -0.5f), // topright vert
            new Vector3(0.5f, 0.5f, 0.5f), // bottomright vert
            new Vector3(-0.5f, 0.5f, 0.5f), // bottomleft vert
            // bottom face
            new Vector3(-0.5f, -0.5f, 0.5f), // topleft vert
            new Vector3(0.5f, -0.5f, 0.5f), // topright vert
            new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
            new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
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

        storage.StoreResource(new ShapeCreatorService("Shapes/cube.shape", "cube", cubeIndices, cubeVerts));
    }
}