using Godot;


// most likely not needed as we have entity spwaner base, but can be used for more specific enemy spawner logic if needed
public partial class EnemySpawner : EntitySpawnerBase
{
    [Export] private PackedScene _enemyScene;
    [Export] private PackedScene _eliteEnemyScene; // Optional: separate scene for elite
    [Export] private int _enemyCount = 50;
    [Export] private Vector3 _spawnAreaMin = new(-10, 1, -10);
    [Export] private Vector3 _spawnAreaMax = new(10, 1, 10);

    private int _spawnedCount = 0;
    private int _killedCount = 0;

    public override void _Ready()
    {
        SpawnEntities();
    }

    public override void SpawnEntities()
    {
        if (_enemyScene == null)
        {
            GD.PrintErr("Enemy scene not assigned!");
            return;
        }

        var random = new RandomNumberGenerator();
        random.Randomize();

        for (int i = 0; i < _enemyCount; i++)
        {
            SpawnEntity(random);
        }
    }

    public override void SpawnEntity(RandomNumberGenerator random)
    {
        PackedScene sceneToUse = _enemyScene;

        // Example: After 50 kills, spawn elites
        if (_killedCount >= 50 && _eliteEnemyScene != null)
        {
            sceneToUse = _eliteEnemyScene;
        }

        var enemy = sceneToUse.Instantiate<CharacterBody3D>();
        float x = random.RandfRange(_spawnAreaMin.X, _spawnAreaMax.X);
        float y = random.RandfRange(_spawnAreaMin.Y, _spawnAreaMax.Y);
        float z = random.RandfRange(_spawnAreaMin.Z, _spawnAreaMax.Z);
        enemy.Position = new Vector3(x, y, z);


        AddChild(enemy);
        _spawnedCount++;
    }

    // Call this when an enemy is killed (from enemy script or collision)
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