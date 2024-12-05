using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graphics.Camera { 
    public class Camera3D
    {
        private FovInfo fovInfo;
        private Matrix4 projection;

        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;
        // The up direction in world space (constant)
        private readonly Vector3 WorldUp;

        public Vector3 Position;
        private Vector2 lastPos = new Vector2(0f, 0f);


        // Euler Angles
        public float Yaw;
        public float Pitch;

        // camera settings
        private float SPEED = 8f;
        private float SENSITIVITY = 180f;

        // Constructor
        public Camera3D(float fovy, float height, float width, float depthNear = 0.1f, float depthFar = 100f, float yaw = 0f, float pitch = 0f)
        {
            fovInfo = new FovInfo
            {
                fov = fovy,
                depthFar = depthFar,
                depthNear = depthNear,
                screenHeight = height,
                screenWidth = width
            };

            UpdateFov(height, width);
            Position = new Vector3(0f, 0f, 0f);
            WorldUp = Vector3.UnitY;
            Yaw = yaw;
            Pitch = pitch;
            lastPos = new Vector2(0f, 0f);
            Front = new Vector3(0.0f, 0.0f, -1.0f); // probably shouldn't be -1f but idk it works lol
            UpdateCameraVectors();
        }

        public Matrix4 GetProjectionMatrix() => projection;

        public Matrix4 GetViewMatrix()
            => Matrix4.LookAt(Position, Position + Front, Up);

        public void UpdateFov(float height, float width)
        {
            fovInfo.screenHeight = height;
            fovInfo.screenWidth = width;

            float aspectRatio = 1920f / 1080f; // TODO figure out why it kills itself when it's done using the dynamic height and width.

            // Now create the perspective projection using the correct aspect ratio
            projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(fovInfo.fov),
                aspectRatio,
                fovInfo.depthNear,
                fovInfo.depthFar
            );
        }
        public void InputController(KeyboardState input, MouseState mouse, float time) // bug it should not be able to look higher than 180 degrees for obvious reasons.
        {
            if (input.IsKeyDown(Keys.W))
            {
                Position += Front * SPEED * time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                Position -= Right * SPEED * time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                Position -= Front * SPEED * time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                Position += Right * SPEED * time;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                Position.Y += SPEED * time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                Position.Y -= SPEED * time;
            }

            Vector2 currentPos = new Vector2(mouse.X, mouse.Y);

            if (lastPos == currentPos)
                return;

            float deltaX = mouse.X - lastPos.X;
            float deltaY = mouse.Y - lastPos.Y;
            lastPos = currentPos;


            Yaw += deltaX * SENSITIVITY * time;
            Pitch -= deltaY * SENSITIVITY * time;

            UpdateCameraVectors();
        }
        public void UpdateCameraVectors()
        {
            float yawRad = MathHelper.DegreesToRadians(Yaw);
            float pitchRad = MathHelper.DegreesToRadians(Pitch);

            // Calculate the new Front vector
            Front.X = MathF.Cos(yawRad) * MathF.Cos(pitchRad);
            Front.Y = MathF.Sin(pitchRad);
            Front.Z = MathF.Sin(yawRad) * MathF.Cos(pitchRad);
            Front = Vector3.Normalize(Front);

            // Recalculate the Right and Up vectors
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    }

    public record struct FovInfo
    {
        public float fov;
        public float screenHeight;
        public float screenWidth;
        public float depthNear;
        public float depthFar;
    }
}
