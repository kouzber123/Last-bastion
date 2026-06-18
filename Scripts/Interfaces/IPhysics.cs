namespace startup.Scripts.Interfaces
{
    public interface IPhysics
    {
        public float Gravity { get; set; }
        void ApplyGravity(float gravity, double delta);
        void ApplyFriction(float friction);
    }
}