using Graphics.GpuComputing;
using Graphics.GpuMemory;
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
            GL.ClearColor(Background);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }
        public void RenderSingleVoxel(VoxelType type, Vector3 position, Shape shape, Shader shader, Matrix4 viewMatrix)
        {
            device.Bind(shader);
            device.Bind(shape.BufferStructure);
            device.Bind(shape.ElementArray);
            device.Bind(shape.VertexArray);

            shader.SetUniform(viewMatrix * Projection, "u_viewProjection");
            shader.SetUniform(position, "u_pos");
            //shader.SetUniform((int)type, "u_type");

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
