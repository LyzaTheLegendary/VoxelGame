using OpenTK.Mathematics;
using Utils;
using Voxels;

namespace Content.Universe.Entities
{
    public enum EntityType : short
    {
        Player,
    }
    public enum State : byte
    {
        Idle,
        Walking,
        Running,
        Dead,
        Attacking,
    }
    public struct Stats
    {
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
        public int Speed { get; set; }
    }
    public struct Rotation
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public Quaternion ToQuaternion() => Quaternion.FromEulerAngles(MathHelper.DegreesToRadians(Yaw), MathHelper.DegreesToRadians(Pitch), MathHelper.DegreesToRadians(Roll));
    }
    public abstract class Entity
    {
        public Handle Handle { get; init; }
        public World World { get; protected set; }
        public EntityType EntityType { get; init; }

        public Vector3 Position { get => position; }
        public Vector3 Velocity {  get => velocity; }


        protected State state = State.Idle;
        protected Stats status = new Stats();
        protected Vector3 velocity  = new Vector3();
        protected Vector3 position  = new Vector3();
        protected Rotation rotation = new Rotation();
        protected Vector3 scale = new Vector3(1, 1, 1);

        protected Entity(Handle handle, World world, Vector3 position, EntityType type)
        {
            Handle = handle;
            World = world;
            this.position = position;
            EntityType = type;
        }
        public virtual void Update(double time)
        {
            if(state == State.Dead)
                return;
            
            const float GRAVITY = 1.0f;
            const float AIR_RESISTANCE = 1.0f;

            if (velocity.X > 0)
            {
                velocity.X = ApplyResistance(velocity.X, AIR_RESISTANCE, time);
                position.X += velocity.X * (float)time;
            }
            if (velocity.Y > 0)
            {
                velocity.Y = ApplyResistance(velocity.Y, GRAVITY, time);
                position.Y += velocity.Y * (float)time;
            }
            if (velocity.Z > 0)
            {
                velocity.Z = ApplyResistance(velocity.Z, AIR_RESISTANCE, time);
                position.Z += velocity.Z * (float)time;
            }

            // Collision detection?
            VoxelType type = World.GetBlockAt((int)position.X, (int)position.Y - 1, (int)position.Z);

            // detect whether the entity is on the ground
            if (type == VoxelType.AIR)
                velocity.Y -= GRAVITY;
            
        }
        protected void HandleCollisions() // TODO: implement lol it's shit
        {
            VoxelType type = World.GetBlockAt((int)position.X + 1, (int)position.Y, (int)position.Z);

            if (type.IsSolid())
            {
                velocity.X = 0;
            }
        }
        protected float ApplyResistance(float velocity, float resistance, double time) 
            => velocity - resistance * (float)time;
        public virtual void KnockBack(Entity source, int knockback)
        {
            Vector3 direction = position - source.position;
            direction.Normalize();

            velocity.X += direction.X * knockback;
            velocity.Y += direction.Y * knockback;
            velocity.Z += direction.Z * knockback;
        }
        public virtual void DamageFrom(Entity source, int damage)
        {
            status.CurrentHealth -= damage;

            if(status.CurrentHealth <= 0)
                OnDeath(source);
            
        }
        public virtual void OnDeath(Entity killer)
        {
            state = State.Dead;
        }
        public Matrix4 GetTransformations() 
            => Matrix4.CreateFromQuaternion(rotation.ToQuaternion()) * Matrix4.CreateTranslation(position) * Matrix4.CreateScale(scale);
        
    }
}
