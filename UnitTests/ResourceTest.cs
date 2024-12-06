using Graphics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using Resources;
using Resources.Creators;

namespace UnitTests
{
    public class ResourceTest : ITest
    {
        public void Run()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    Storage storage = new();
                    AddImagetoStorage(storage);
                    AddShaderToStorage(storage);
                    AddCubeToStorage(storage);

                    // initiate openGL context
                    new OpenGLContext(storage).Run();

                    storage.Remove("Textures/BlockAtlas.bitmap");
                    storage.Remove("Shaders/test.shaders");
                    storage.Remove("Shapes/cube.shape");
                }
            }
            catch(Exception ex)
            {
                TestConsole.Failed($"Resource saving: {ex.Message}");
                return;
            }
            TestConsole.Passed("Resource saving");
        }
        public static void AddImagetoStorage(Storage storage)
        {
            byte[] image = File.ReadAllBytes("BlockAtlas.png");
            storage.StoreResource(new BitmapCreatorService("Textures/BlockAtlas.bitmap", image, true, 16, 16));
        }
        public static void AddShaderToStorage(Storage storage)
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

            Vector2[] texCoords = new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        };

            Vertex[] vertices = new Vertex[cubeVerts.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vertex()
                {
                    Position = cubeVerts[i],
                    TexCoord = texCoords[i]
                };
            }

            storage.StoreResource(new ShapeCreatorService("Shapes/cube.shape", "cube", vertices, cubeIndices));
        }
    }
}
