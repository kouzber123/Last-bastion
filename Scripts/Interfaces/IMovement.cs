using Godot;

namespace startup.Scripts.Interfaces
{
    // This interface defines the contract for any movement-related functionality in the game.
    public interface IMovement
    {
        public float Speed { get; set; } // Base movement speed, can be modified by SpeedMultiplier for effects like buffs/debuffs

        // Behavior methods
        void Chase(Godot.Vector3 targetPosition); // Move towards a target position, typically used for enemies chasing the player use process
        void HandleRotation(Node3D characterModel, Vector3 moveDirection, float rotationSpeed); // Handle character rotation based on movement direction or other factors

        bool IsMoving { get; }
        void ApplyGravity(float gravity, double delta);
        Animation GetAnimation();
    }
}