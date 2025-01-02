using OpenTK.Mathematics;
using Utils;

namespace Resources.Components
{
    public class Animation : IComponent
    {
        public string Name { get; private set; }
        public int TotalFrames { get; private set; }
        public int BoneCount { get; private set; }
        public int TickSpeed { get; private set; } // Blender default is 24
        public List<Matrix3[]> Frames { get; private set; }

        public Animation(Stream stream)
        {
            Name = stream.ReadString();
            TotalFrames = stream.Read<int>();
            BoneCount = stream.Read<int>();
            TickSpeed = stream.Read<int>();
            
            Frames = new List<Matrix3[]>(TotalFrames);

            for (int i = 0; i < TotalFrames; i++)
            {
                Matrix3[] frame = new Matrix3[BoneCount];
                for (int frameIndex = 0; frameIndex < BoneCount; frameIndex++)
                    frame[frameIndex] = stream.Read<Matrix3>();

                Frames.Add(frame);
            }
        }

        public Matrix3[] GetFrame(Bone[] bones, ref float currentFrameIndex, float deltaTime, float animationSpeed = 1f)
        {
            if (TotalFrames < 2)
                throw new InvalidOperationException("Animation must have at least two frames for interpolation.");

            Matrix3[] interpolatedFrame = new Matrix3[BoneCount];

            int frameA = (int)Math.Floor(currentFrameIndex) % TotalFrames;
            int frameB = (frameA + 1) % TotalFrames;

            float t = currentFrameIndex - frameA;

            for (int i = 0; i < BoneCount; i++)
            {
                Bone bone = bones[i];
                
                interpolatedFrame[i] = MatrixHelper.Lerp(Frames[frameA][bone.Index], Frames[frameB][bone.Index], t);
            }

            currentFrameIndex = (currentFrameIndex + deltaTime * (TickSpeed * animationSpeed)) % TotalFrames;

            return interpolatedFrame;
        }

    }

}
