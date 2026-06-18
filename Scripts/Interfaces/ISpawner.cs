using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace startup.Scripts.Interfaces
{
    // This interface defines the contract for any spawner in the game, such as enemy spawners, item spawners, etc.
    public interface ISpawner
    {
        void SpawnEntity(RandomNumberGenerator random);
        void SpawnEntities();
        void Despawn();
    }
}