using Content.Universe;
using Graphics.GpuComputing;
using Graphics.GpuMemory;
using Graphics.GpuTextures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Voxels;

namespace Graphics
{
    public class Renderer
    {
        private readonly GraphicsDevice device;
        public Matrix4 Projection { get; set; }
        public Color4 Background { get; set; }

        public Renderer(GraphicsDevice device, Matrix4 projection)
        {
            this.device = device;
            Projection = projection;
            Background = Color4.Black;
        }
        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background);
        }
        public void RenderSingleVoxel(VoxelType type, Vector3 position, Shape shape, Shader shader, TextureAtlas2D texture, Matrix4 viewMatrix)
        {
            const int cubeFaceCount = 6;
            // start using quaternions for the camera! to prevent gambol locks
            device.Bind(shader);
            device.Bind(shape.BufferStructure);
            device.Bind(shape.ElementArray);
            device.Bind(shape.VertexArray);
            device.Bind(texture);

            shader.SetUniform(Matrix4.CreateTranslation(position) * viewMatrix * Projection, "u_transformations");

            //Turn both uniforms into a long using bitwise operators
            shader.SetUniform(texture.Columns, "u_columns");

            //For the texture atlas, It needs a offset of 5 between each index, So the other slots are filled with the other faces.
            shader.SetUniform((int)type * cubeFaceCount, "u_index");
            
            GL.DrawElements(PrimitiveType.Triangles, shape.ElementArray.GetCount(), DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void RenderChunk(Chunk chunk, Shader shader, Shape shape, Matrix4 viewMatrix)
        {            
            shader.SetUniform(Matrix4.CreateTranslation(chunk.Position) * viewMatrix * Projection, "u_transformations");

            device.Bind(shader);
            device.Bind(shape.VertexArray);
            device.Bind(shape.ElementArray);
            device.Bind(chunk.Buffer);

            chunk.UpdateIfDirty();

            GL.DrawElementsInstanced(PrimitiveType.Triangles, chunk.Buffer.Count(), DrawElementsType.UnsignedInt, IntPtr.Zero, chunk.Buffer.Count());
        }
    }
}
