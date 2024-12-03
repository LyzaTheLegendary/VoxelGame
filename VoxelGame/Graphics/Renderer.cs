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
        public void RenderSingleVoxel(VoxelType type, Vector3 position, Shape shape, Shader shader, Texture2D texture, Matrix4 viewMatrix)
        {
            device.Bind(shader);
            device.Bind(shape.BufferStructure);
            device.Bind(shape.ElementArray);
            device.Bind(shape.VertexArray);
            device.Bind(texture);

            //shader.SetUniform(Matrix4.Identity * viewMatrix * Projection, "u_viewProjection");
            //shader.SetUniform(position, "u_pos");
            //shader.SetUniform(, "u_pos");
            //shader.SetUniform((int)type, "u_type");
            shader.SetUniform(viewMatrix, "u_view");
            shader.SetUniform(Projection, "u_projection");
            //shader.SetUniform(Matrix4.Identity, "u_model");
            
            GL.DrawElements(PrimitiveType.Triangles, shape.ElementArray.GetCount(), DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void RenderBatch(VoxelBatch batch, Shader shader, Shape shape, Matrix4 viewMatrix)
        {
            shader.SetUniform(viewMatrix * Projection, "u_vp");
            shader.SetUniform(batch.Position, "u_batchPos");

            GpuShaderStorageBuffer<Voxel> buffer = batch.GetGpuBuffer();
            batch.PrepareRenderIfNeeded();

            device.Bind(shader);
            device.Bind(shape.VertexArray);
            device.Bind(shape.ElementArray);
            device.Bind(buffer);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, buffer.Count(), DrawElementsType.UnsignedInt, IntPtr.Zero, buffer.Count());
        }
    }
}
