using Godot;
using startup.Scripts.Interfaces;

public abstract partial class EntitySpawnerBase : Node, ISpawner
{
    public void Despawn()
    {
        throw new System.NotImplementedException();
    }

    public virtual void SpawnEntities()
    {
        throw new System.NotImplementedException();
    }

    public virtual void SpawnEntity(RandomNumberGenerator random)
    {
        throw new System.NotImplementedException();
    }

    // ie looting items, killing enemies will reset via this
    public virtual void OnEntityKilled()
    {
        // Optional: Override in derived classes to handle when an entity is killed
    }
}
