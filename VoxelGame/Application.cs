using Graphics;
using Graphics.Camera;
using Graphics.GpuComputing;
using NativeCalls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Resources;
using Voxels;


namespace VoxelGame
{
    public class Application : GameWindow
    {
        public static Application Instance { get; private set; }

        public readonly MonitorStruct m_monitorInfo = new MonitorStruct();
        public Camera3D Camera { get; private set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public Renderer Renderer { get; set; }
        public Storage Storage { get; set; }
        public Application() : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 5), // Request OpenGL 4.5
            Profile = ContextProfile.Core,  // Core profile ensures modern features
        })
        {
            Camera = new Camera3D(45f, 1920f, 1080f);
            Storage = new Storage();
            Title = "VoxelGame demo";
            Instance = this;
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GraphicsDevice = new GraphicsDevice();
            Renderer = new Renderer(GraphicsDevice, Camera.GetProjectionMatrix());
            Storage.Load();

            Shader shader;
            using (Resource<Shader> resource = Storage.GetResource<Shader>("Shaders/test.shaders"))
                shader = resource.GetComponent();

            Shape shape;
            using (Resource<Shape> shapeResource = Storage.GetResource<Shape>("Shapes/cube.shape"))
                shape = shapeResource.GetComponent();
            Console.WriteLine("Loaded");
        }
        protected override void OnUnload()
        {
            Storage.Save();
            base.OnUnload();
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0,0, e.Width, e.Height);
            Camera.UpdateFov((float)e.Width, (float)e.Height);
            Renderer.Projection = Camera.GetProjectionMatrix();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {

            Camera.InputController(this.KeyboardState, this.MouseState, (float)args.Time);
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            Renderer.Clear();

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
