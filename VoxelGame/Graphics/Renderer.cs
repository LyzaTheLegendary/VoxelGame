using Content.Universe;
using Graphics.Camera;
using Graphics.GpuComputing;
using Graphics.GpuMemory;
using Graphics.GpuTextures;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using VoxelGame.Resources.Components;
using Voxels;

namespace Graphics
{
    public class Renderer
    {
        public Matrix4 Projection { get; set; }
        public Color4 Background { get; set; }
        public Camera3D Camera { get; set; }
        public Renderer(Camera3D camera)
        {
            Camera = camera;
            Background = Color4.Black;
            Projection = Camera.GetProjectionMatrix();
            
        }
        public void Clear()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(Background);
        }
        public void UpdateFov(float height, float width)
        {
            Camera.UpdateFov(height, width);
            Projection = Camera.GetProjectionMatrix();
        }
        
        public void RenderSingleVoxel(VoxelType type, Vector3 position, Shape shape, Shader shader, TextureAtlas2D texture)
        {
            const int cubeFaceCount = 6;
            // start using quaternions for the camera! to prevent gambol locks
            GraphicsDevice.Bind(shader);
            GraphicsDevice.Bind(shape.BufferStructure);
            GraphicsDevice.Bind(shape.ElementArray);
            GraphicsDevice.Bind(shape.VertexArray);
            GraphicsDevice.Bind(texture);

            shader.SetUniform(Matrix4.CreateTranslation(position) * Camera.GetViewMatrix() * Projection, "u_transformations");

            //Turn both uniforms into a long using bitwise operators
            shader.SetUniform(texture.Columns, "u_columns");

            //For the texture atlas, It needs a offset of 5 between each index, So the other slots are filled with the other faces.
            shader.SetUniform((int)type * cubeFaceCount, "u_index");
            
            GL.DrawElements(PrimitiveType.Triangles, shape.ElementArray.Count(), DrawElementsType.UnsignedInt, IntPtr.Zero);
        }

        public void RenderChunk(Chunk chunk, Shader shader, TextureAtlas2D texture)
        {
            GraphicsDevice.Bind(shader);
            //shader.SetUniform(texture.Columns, "u_columns");
            //shader.SetUniform(Chunk.BATCH_SIZE, "u_batchSize");
            shader.SetUniform(Matrix4.CreateTranslation(chunk.Position) * Camera.GetViewMatrix() * Projection, "u_transformations");
            
            
            GraphicsDevice.Bind(chunk.Mesh.VertexBuffer);
            GraphicsDevice.Bind(chunk.Mesh.IndexBuffer);
            GraphicsDevice.Bind(chunk.Mesh.BufferStructure);
            
            GraphicsDevice.Bind(texture);
            chunk.UpdateIfDirty();

            GL.DrawElements(PrimitiveType.Triangles, chunk.Mesh.IndexBuffer.Count(), DrawElementsType.UnsignedInt, IntPtr.Zero);
            //GL.DrawElementsInstanced(PrimitiveType.Triangles, chunk.Mesh.IndexBuffer.Count(), DrawElementsType.UnsignedInt, IntPtr.Zero, chunk.Mesh.IndexBuffer.Count());
        }
    }
}
