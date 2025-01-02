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
        
        public Shader(Stream stream)
        {
            pointer = GL.CreateProgram();
            Name = stream.ReadString();
            int shaders = stream.ReadByte();
            for (int i = 0; i < shaders; i++)
            {
                ShaderType type = (ShaderType)stream.Read<int>();
                string shaderCode = stream.ReadString();

                ShaderCode code = new ShaderCode(type);

                code.Compile(shaderCode);

                AttachCode(code);
            }

            Link();
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

            if (success == 0)
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
        public void SetUniform(Matrix4[] matrices, string name)
        {
            int location = GL.GetUniformLocation(pointer, name);

            if (location == -1)
                throw new ArgumentException($"Uniform {name} does not exist");

            int requiredSize = matrices.Length * 16; // Each Matrix4 has 16 elements

            if (matrixBuffer == null || matrixBuffer.Length < requiredSize)
                matrixBuffer = new float[requiredSize];

            for (int i = 0; i < matrices.Length; i++)
            {
                matrixBuffer[i * 16 + 0] = matrices[i].M11;
                matrixBuffer[i * 16 + 1] = matrices[i].M21;
                matrixBuffer[i * 16 + 2] = matrices[i].M31;
                matrixBuffer[i * 16 + 3] = matrices[i].M41;
                matrixBuffer[i * 16 + 4] = matrices[i].M12;
                matrixBuffer[i * 16 + 5] = matrices[i].M22;
                matrixBuffer[i * 16 + 6] = matrices[i].M32;
                matrixBuffer[i * 16 + 7] = matrices[i].M42;
                matrixBuffer[i * 16 + 8] = matrices[i].M13;
                matrixBuffer[i * 16 + 9] = matrices[i].M23;
                matrixBuffer[i * 16 + 10] = matrices[i].M33;
                matrixBuffer[i * 16 + 11] = matrices[i].M43;
                matrixBuffer[i * 16 + 12] = matrices[i].M14;
                matrixBuffer[i * 16 + 13] = matrices[i].M24;
                matrixBuffer[i * 16 + 14] = matrices[i].M34;
                matrixBuffer[i * 16 + 15] = matrices[i].M44;
            }

            GL.UniformMatrix4(location, matrices.Length, false, matrixBuffer);
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
        
        public void SetUniform(long number, string name)
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
    }

    public record struct Uniform
    {
        public string Name { get; init; }
        public ActiveAttribType Type { get; init; }
        public int Location { get; init; }
    }
}
