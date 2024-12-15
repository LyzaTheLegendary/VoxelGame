using Graphics;
using Graphics.Camera;
using Graphics.GpuComputing;
using Graphics.GpuTextures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using Resources;
using Resources.Components;
using VoxelGame;
using VoxelGame.Resources.Components;

namespace UnitTests
{
    public class OpenGLContext : GameWindow
    {
        Storage storage;
        public OpenGLContext(Storage storage) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            IsVisible = false;
            this.storage = storage;
        }

        protected override void OnLoad()
        {
            Close();
            Shader shader;
            using (Resource<Shader> resource = storage.GetResource<Shader>("Shaders/test.shaders"))
                shader = resource.GetComponent();

            Shape shape;
            using (Resource<Shape> resource = storage.GetResource<Shape>("Shapes/cube.shape"))
                shape = resource.GetComponent();

            Bitmap bitmap;
            using (Resource<Bitmap> Resource = storage.GetResource<Bitmap>("Textures/BlockAtlas.bitmap"))
                bitmap = Resource.GetComponent();

            TextureAtlas2D texture = GraphicsDevice.AllocateTextureAtlas(bitmap, TextureUnit.Texture0);

            shader.Dispose();
            shape.Dispose();
            texture.Dispose();

            ErrorCode error = GL.GetError();

            if(error != ErrorCode.NoError)
                throw new System.Exception($"OpenGL error: {error}");
        }
    }
}
