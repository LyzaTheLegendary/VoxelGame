using OpenTK.Graphics.OpenGL4;

namespace Graphics.GpuComputing
{
    public ref struct ShaderCode
    {
        public readonly ShaderType Type { get; init; }
        private int pointer;
        private bool compiled = false;
        public ShaderCode(ShaderType shaderType)
        {
            pointer = GL.CreateShader(shaderType);
            Type = shaderType;
        }

        public bool Compiled() => compiled;
        public int GetPointer() => pointer;
        public void Compile(string shaderCode)
        {
            GL.ShaderSource(pointer, shaderCode);
            GL.CompileShader(pointer);

            GL.GetShader(pointer, ShaderParameter.CompileStatus, out int success);

            if(success == 0) { 
                string infoLog = GL.GetShaderInfoLog(pointer);
                throw new Exception(infoLog);
            }

            compiled = true;
        }

        public void Delete() => GL.DeleteShader(pointer);
    }
}
