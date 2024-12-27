using Utils;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Resources;

namespace Graphics.GpuComputing
{
    public class Shader : IDisposable, IComponent
    {
        public string Name { get; private set; }

        private bool ready = false;
        private int pointer;
        private bool disposedValue;
        private float[] matrixBuffer = null;
        public Shader(string name)
        {
            Name = name;
            pointer = GL.CreateProgram();
        }
        public Shader()
        {
            // For IResourceFactory
        }
        public void AttachCode(scoped ShaderCode code)
        {
            if (ready)
                throw new Exception("Cannot attach new code to a already ready program.");
           
            GL.AttachShader(pointer, code.GetPointer());
            code.Delete();
        }

        public void Link()
        {
            GL.LinkProgram(pointer);

            GL.GetProgram(pointer, GetProgramParameterName.LinkStatus, out int success);

            if(success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(pointer);
                throw new Exception($"Error linking program: {infoLog}");
            }

            ready = true;
        }
        public void SetUniform(Matrix3[] matrices, string name)
        {
            int location = GL.GetUniformLocation(pointer, name);

            if (location == -1)
                throw new ArgumentException($"Uniform {name} does not exist");

            int requiredSize = matrices.Length * 9; // Each Matrix3 has 9 elements

            if (matrixBuffer == null || matrixBuffer.Length < requiredSize)
                matrixBuffer = new float[requiredSize];

            for (int i = 0; i < matrices.Length; i++)
            {
                matrixBuffer[i * 9 + 0] = matrices[i].M11;
                matrixBuffer[i * 9 + 1] = matrices[i].M21;
                matrixBuffer[i * 9 + 2] = matrices[i].M31;
                matrixBuffer[i * 9 + 3] = matrices[i].M12;
                matrixBuffer[i * 9 + 4] = matrices[i].M22;
                matrixBuffer[i * 9 + 5] = matrices[i].M32;
                matrixBuffer[i * 9 + 6] = matrices[i].M13;
                matrixBuffer[i * 9 + 7] = matrices[i].M23;
                matrixBuffer[i * 9 + 8] = matrices[i].M33;
            }

            GL.UniformMatrix3(location, matrices.Length, false, matrixBuffer);
        }
        public void SetUniform(Matrix4 matrix, string name)
        {
            int location = GL.GetUniformLocation(pointer, name);

            if (location == -1)
                throw new ArgumentException($"uniform {name} does not exist");

            GL.UniformMatrix4(location, true, ref matrix);
        }
        public void SetUniform(Vector3i vector, string name)
        {
            int location = GL.GetUniformLocation(pointer, name);

            if (location == -1)
                throw new ArgumentException($"uniform {name} does not exist");

            GL.Uniform3(location, ref vector);
        }
        public void SetUniform(Vector3 vector, string name)
        {
            int location = GL.GetUniformLocation(pointer, name);

            if (location == -1)
                throw new ArgumentException($"uniform {name} does not exist");

            GL.Uniform3(location, ref vector);
        }
        public void SetUniform(int number, string name)
        {
            int location = GL.GetUniformLocation(pointer, name);

            if (location == -1)
                throw new ArgumentException($"uniform {name} does not exist");

            GL.Uniform1(location, number);
        }
        public int GetPointer() => pointer;
        public void Bind() => GL.UseProgram(pointer);
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                    GL.DeleteProgram(pointer);
                
                disposedValue = true;
            }
        }

        ~Shader() => Dispose(disposing: false);
        

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            pointer = GL.CreateProgram();
            using (MemoryStream stream = new MemoryStream(data as byte[] ?? data.ToArray()))
            {
                Name = stream.ReadString();
                int shaders = stream.ReadByte();
                for(int i = 0; i < shaders; i++)
                {
                    ShaderType type = (ShaderType)stream.Read<int>();
                    string shaderCode = stream.ReadString();

                    ShaderCode code = new ShaderCode(type);

                    code.Compile(shaderCode);

                    AttachCode(code);
                }
            }

            Link();
        }
    }

    public record struct Uniform
    {
        public string Name { get; init; }
        public ActiveAttribType Type { get; init; }
        public int Location { get; init; }
    }
}
