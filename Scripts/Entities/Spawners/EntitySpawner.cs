using Godot;

/**
 * EntitySpawner.cs
 * 
 * This script is responsible for spawning entities (enemies, items, etc.) in the game world.
 * It can be attached to a Node in the scene and configured to spawn specific entities at certain intervals or conditions.
 * 
 * Example usage:
 * - Attach this script to a Node in the scene where you want to spawn entities.
 * - Configure the PackedScene property to specify which entity to spawn.
 * - Set the spawn interval and any other parameters as needed.
 */
/*
As each spawnable object will inherit this class 
important to understand spawn areas ie flower patches, enemy spawn points, item spawn points etc.
This will allow us to easily manage and expand our spawning system in the future, such as adding different 
types of spawners (e.g., item spawners, environmental spawners), and implementing more complex spawning logic (e.g., spawn waves, random spawns, etc.)
*/
public partial class EntitySpawner : EntitySpawnerBase
{
	[Export] private PackedScene _entityScene; // The scene to spawn (e.g., enemy, item)
	[Export] private PackedScene _eliteEntityScene; // Optional: separate scene for elite
	[Export] private int _entityCount = 50;
	[Export] private Vector3 _spawnAreaMin = new(-10, 1, -10);
	[Export] private Vector3 _spawnAreaMax = new(10, 1, 10);
	[Export] private ulong seed = 0; // Optional: seed for deterministic spawning
	private int _spawnedCount = 0;
	private int _killedCount = 0;

	public override void _Ready()
	{
		SpawnEntities();
	}

	public override void SpawnEntities()
	{
		if (_entityScene == null)
		{
			GD.PrintErr("Entity scene not assigned!");
			return;
		}

		var random = new RandomNumberGenerator();
		if (seed != 0)
		{

			random.Seed = seed; // Set the seed for deterministic spawning
		}
		else random.Randomize(); // Randomize if no seed is provided

		for (int i = 0; i < _entityCount; i++)
		{
			SpawnEntity(random);
		}
	}

	public override void SpawnEntity(RandomNumberGenerator random)
	{
		if (seed != 0)
		{
			GD.Print($"Spawning with seed: {seed}");
		}
		PackedScene sceneToUse = _entityScene; // The scene to spawn (e.g., enemy, item)

		// Example: After 50 kills, spawn elites
		if (_killedCount >= 50 && _eliteEntityScene != null)
		{
			sceneToUse = _eliteEntityScene;

		}

		var entity = sceneToUse.Instantiate<Node3D>();
		float x = random.RandfRange(_spawnAreaMin.X, _spawnAreaMax.X);
		float y = random.RandfRange(_spawnAreaMin.Y, _spawnAreaMax.Y);
		float z = random.RandfRange(_spawnAreaMin.Z, _spawnAreaMax.Z);
		entity.Position = new Vector3(x, y, z);
		// Modify properties for elite

		AddChild(entity);
		_spawnedCount++;
	}


	// Call this when an entity is killed (from entity script or collision)
	public override void OnEntityKilled()
	{
		_killedCount++;
		GD.Print($"Enemies killed: {_killedCount}");

		// Optionally spawn a new enemy or elite
		if (_killedCount % 10 == 0) // Every 10 kills, spawn one more
		{
			var random = new RandomNumberGenerator();
			random.Randomize();
			SpawnEntity(random);
		}
	}
}
