using Content;
using Graphics;
using Graphics.Camera;
using Graphics.GpuComputing;
using NativeCalls;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Resources;
using System.Runtime.InteropServices;
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

        private Shader shader; // temp value

        private Vector3 tempPos = new Vector3(0, 0, 0);
        public Application() : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 5), // Request OpenGL 4.5
            Profile = ContextProfile.Core,  // Core profile ensures modern features
#if DEBUG
            Flags = ContextFlags.Debug
#endif
        }
        
        )
        {
            Camera = new Camera3D(45f, 1920f, 1080f);
            Storage = new Storage();
            Title = "VoxelGame demo";
            Instance = this;
        }
        private static void OnDebugMessage(
            DebugSource source,     // Source of the debugging message.
            DebugType type,         // Type of the debugging message.
            int id,                 // ID associated with the message.
            DebugSeverity severity, // Severity of the message.
            int length,             // Length of the string in pMessage.
            IntPtr pMessage,        // Pointer to message string.
            IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
        {
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);

            if (type == DebugType.DebugTypeError)
                throw new Exception(message);
            
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            GraphicsDevice = new GraphicsDevice();
            Renderer = new Renderer(GraphicsDevice, Camera.GetProjectionMatrix());
#if DEBUG
            GL.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);

            // Optionally
            GL.Enable(EnableCap.DebugOutputSynchronous);
#endif
            Storage.Load();

            //Shader shader;
            using (Resource<Shader> resource = Storage.GetResource<Shader>("Shaders/test.shaders"))
                shader = resource.GetComponent();

            //Shape shape;
            //using (Resource<Shape> shapeResource = Storage.GetResource<Shape>("Shapes/cube.shape"))
            //    shape = shapeResource.GetComponent();
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

            //Shape shape;
            //using (Resource<Shape> shapeResource = Storage.GetResource<Shape>("Shapes/cube.shape"))
            //    shape = shapeResource.GetComponent();


            Renderer.RenderSingleVoxel(VoxelType.DIRT, tempPos, GameContent.GetShape("cube"), shader, Camera.GetViewMatrix());
            //Renderer.RenderSingleVoxel(VoxelType.DIRT, new Vector3(0, 0, 0), shape, shader, Camera.GetViewMatrix());

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
