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
using Graphics.GpuTextures;
using Resources.Components;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Content.Universe;
using Content.Universe.Entities;


namespace VoxelGame
{
    public class Application : GameWindow
    {
        public readonly MonitorStruct m_monitorInfo = new MonitorStruct();
        public Renderer Renderer { get; set; }
        public Storage Storage { get; set; }

        private Shader shader; // temp value
        private Shader chunkShader;
        private Shader modelShader;
        private Model model;
        private float frame = 0;
        private Matrix3[] animation;
        private TextureAtlas2D texture; // temp val
        private Vector3 tempPos = new Vector3(0, 0, 0);// temp val
        private World world = new World(WorldType.EARTH);
        private Player player;
        public Application() : base(GameWindowSettings.Default, new NativeWindowSettings
        {
            API = ContextAPI.OpenGL,
            APIVersion = new Version(4, 5), // Request OpenGL 4.5
            Profile = ContextProfile.Core,  // Core profile ensures modern features
#if DEBUG
            Flags = ContextFlags.Debug
#endif
        })
        {
            Thread.CurrentThread.Name = "Game loop";
            Storage = new Storage();
            Title = "VoxelGame demo";
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
            GraphicsDevice.Init();
            TextureManager.Init();
            CursorState = CursorState.Grabbed;
            Renderer = new Renderer(new Camera3D(45f, 1920f, 1080f));
            player = new Player(world, tempPos, "test123");
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            //TODO: fix culling
            //GL.Enable(EnableCap.CullFace);
            //GL.CullFace(CullFaceMode.Front);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
#if DEBUG
            GL.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);

            // Optionally
            GL.Enable(EnableCap.DebugOutputSynchronous);

#endif
            Storage.Load();

            //Shader shader;
            using (Resource<Shader> resource = Storage.GetResource<Shader>("Shaders/blockShader.shaders"))
                shader = resource.GetComponent();

            using (Resource<Shader> resource = Storage.GetResource<Shader>("Shaders/chunkShader.shaders"))
                chunkShader = resource.GetComponent();

            using (Resource<Shader> resource = Storage.GetResource<Shader>("Shaders/modelShader.shaders"))
                modelShader = resource.GetComponent();

            using (Resource<Bitmap> Resource = Storage.GetResource<Bitmap>("Textures/BlockAtlas.bitmap"))
                texture = GraphicsDevice.AllocateTextureAtlas(Resource.GetComponent(), TextureUnit.Texture0);

            using(Resource<Model> Resource = Storage.GetResource<Model>("Model/test.model"))
                model = Resource.GetComponent();
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
            Renderer.UpdateFov(e.Width, e.Height);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Environment.Exit(0);

            Renderer.Camera.InputController(KeyboardState, MouseState, (float)args.Time);


            animation = model.GetAnimation("Walk").GetFrame(model.GetBones(), ref frame, (float)args.Time);
            

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {

            Renderer.Clear();
            //Renderer.RenderSingleVoxel(Voxels.VoxelType.DIRT, new Vector3(0, 7, 0), GameContent.GetShape("Object_4"), shader, texture);
            Renderer.RenderModel(model, Vector3.Zero, animation, modelShader);
            //Renderer.RenderChunk(world.GetChunk(0, 0, 0), chunkShader, texture);
            //Renderer.RenderChunk(world.GetChunk(1, 0, 0), chunkShader, texture);
            //Renderer.RenderChunk(world.GetChunk(0, 0, 1), chunkShader, texture);
            //Renderer.RenderChunk(world.GetChunk(0, 0, -1), chunkShader, texture);
            //Renderer.RenderChunk(world.GetChunk(-1, 0, 0), chunkShader, texture);


            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
