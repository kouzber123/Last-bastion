using System.Collections.Generic;
using Godot;


// Manager is a node that has been instantiated in your main scene and is responsible for updating all enemies each frame.
// Enemies will register themselves with the manager when they spawn,
// and the manager will call a custom update method on each enemy every frame,
// passing in the player's position and a separation force to help them avoid crowding each other.
// Manager pattern enables to create centralised events for enemies
// This can be used for other NPCS etc. and can be expanded to include more complex logic such as spawn waves, enemy types, etc.
public partial class EnemyManager : Node
{
    // A globally accessible static instance (Singleton pattern) so enemies can find it easily
    public static EnemyManager Instance { get; private set; }

    // High-performance list to keep track of all live enemies
    private readonly List<EnemyController> ActiveEnemiesList = [];

    // Reference to your player node to pass its position down to enemies
    private CharacterBody3D _player;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        // Replace with how you grab your player node in your scene
        _player = GetTree().GetFirstNodeInGroup("player") as CharacterBody3D;
        // optionally, register multiple players, NPCs, buildings, and ee
    }

    public void RegisterEnemy(EnemyController enemy)
    {

        if (!ActiveEnemiesList
        .Contains(enemy))
        {
            ActiveEnemiesList
            .Add(enemy);
        }
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        ActiveEnemiesList
        .Remove(enemy);
    }

    // This is the SINGLE bridge between Godot's C++ and C# for all enemies

    // Simple pseudo-logic for the Manager Loop
    Vector3 separation = Vector3.Zero;
    float personalSpace = 1.5f; // Distance they want to keep from each othe
    public override void _PhysicsProcess(double delta)
    {
        if (_player == null || ActiveEnemiesList
        .Count == 0) return;
        // 1. Get player position once and pass it to all enemies to save performance
        Vector3 playerPos = _player.GlobalPosition;
        float personalSpace = 1.5f; // Distance enemies want to keep from each other

        for (int i = 0; i < ActiveEnemiesList
        .Count; i++)
        {
            // 2. Calculate separation force for this enemy based on nearby neighbors
            EnemyController currentEnemy = ActiveEnemiesList[i];

            // Reset separation force for this enemy
            Vector3 separationForce = Vector3.Zero;
            float distanceToEnemy = 0f;
            // Loop through everyone else to check for crowding
            for (int j = 0; j < ActiveEnemiesList
            .Count; j++)
            {
                if (i == j) continue; // Skip checking self proceed for next neighbor

                // Get neighbor position and distance
                Vector3 neighborPos = ActiveEnemiesList[j].GlobalPosition;
                float distance = currentEnemy.GlobalPosition.DistanceTo(neighborPos);
                distanceToEnemy = currentEnemy.GlobalPosition.DistanceTo(playerPos);
                // if Distance is less than personal space, calculate a push away from the neighbor
                if (distance < personalSpace && distance > 0.001f) // Avoid divide-by-zero
                {
                    // Calculate direction away from the neighbor
                    Vector3 pushDirection = currentEnemy.GlobalPosition - neighborPos;
                    pushDirection.Y = 0; // Keep push forces on the flat ground grid

                    // The closer they are, the harder they push away
                    float strength = (personalSpace - distance) / personalSpace;
                    separationForce += pushDirection.Normalized() * strength;
                }
            }

            // Pass the calculated separation force into the enemy's update loop
            currentEnemy.UpdateEnemyPhysics(delta, playerPos, distanceToEnemy, separationForce);
        }
    }

}