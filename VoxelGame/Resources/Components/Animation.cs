using OpenTK.Mathematics;
using Utils;

namespace Resources.Components
{
    public class Animation : IComponent
    {
        public string Name { get; private set; }
        public int FrameCount { get; private set; }
        public int BoneCount { get; private set; }
        private float CurrentFrameIndex { get; set; } = 0f; //should also make this a ref variable so the animation can be used in multiple places
        public List<Matrix3[]> Frames { get; private set; }

        public Animation(Stream stream)
        {
            Name = stream.ReadString();
            FrameCount = stream.Read<int>();
            BoneCount = stream.Read<int>();

            Frames = new List<Matrix3[]>(FrameCount);

            for (int i = 0; i < FrameCount; i++)
            {
                Matrix3[] frame = new Matrix3[BoneCount];
                for (int frameIndex = 0; frameIndex < BoneCount; frameIndex++)
                    frame[frameIndex] = stream.Read<Matrix3>();

                Frames.Add(frame);
            }
        }
        public Matrix3[] GetFrame(List<Bone> bones, float deltaTime, float animationSpeed = 1f)
        {
            if (FrameCount < 2)
                throw new InvalidOperationException("Animation must have at least two frames for interpolation.");

            Matrix3[] interpolatedFrame = new Matrix3[BoneCount];

            int frameA = (int)Math.Floor(CurrentFrameIndex) % FrameCount;
            int frameB = (frameA + 1) % FrameCount;

            float t = CurrentFrameIndex - frameA;

            for (int i = 0; i < BoneCount; i++)
            {
                Bone bone = bones[i];
                interpolatedFrame[i] = Lerp(Frames[frameA][bone.Index], Frames[frameB][bone.Index], t);
            }

            CurrentFrameIndex = (CurrentFrameIndex + deltaTime * animationSpeed) % FrameCount;

            return interpolatedFrame;
        }

        private Matrix3 Lerp(Matrix3 from, Matrix3 to, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);

            return new Matrix3(
                Lerp(from.M11, to.M11, t), Lerp(from.M12, to.M12, t), Lerp(from.M13, to.M13, t),
                Lerp(from.M21, to.M21, t), Lerp(from.M22, to.M22, t), Lerp(from.M23, to.M23, t),
                Lerp(from.M31, to.M31, t), Lerp(from.M32, to.M32, t), Lerp(from.M33, to.M33, t)
            );
        }

        private static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        public void CreateResourceFromData(IEnumerable<byte> data)
        {
            using MemoryStream stream = new(data as byte[] ?? data.ToArray());

            Name = stream.ReadString();
            FrameCount = stream.Read<int>();
            BoneCount = stream.Read<int>();

            Frames = new List<Matrix3[]>(FrameCount);

            for (int i = 0; i < FrameCount; i++)
            {
                Matrix3[] frame = new Matrix3[BoneCount];
                for (int frameIndex = 0; frameIndex < BoneCount; frameIndex++)
                    frame[frameIndex] = stream.Read<Matrix3>();

                Frames.Add(frame);
            }
        }
    }

}
