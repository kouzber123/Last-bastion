
using System.Collections.Generic;
using Godot;

/**
AllyManager handles friendly NPCs in the game, such as allies that assist the player. 
It keeps track of all active allies, manages their behavior, 
and ensures they interact properly with the player and enemies. This manager can handle tasks like:
- Registering and unregistering allies as they spawn and despawn.
- Movement and behavior logic for allies, such as following the player, 
assisting in combat, or performing specific actions.
- Managing interactions between allies and enemies, such as targeting or healing.
- Target enemy npc by locking itself to the enemy until either one dies, then it can target another enemy.

- locking logic : on enter enemy range, 
lock to that enemy and follow it until either the enemy or the ally dies, 
then unlock and look for another target. 
This can be done with a simple state machine in the ally script, 
where it has states like 
"Idle", 
"FollowingPlayer", 
"EngagingEnemy", etc. 
The manager can help by providing a list of nearby enemies for the ally to choose from when it enters combat.
Manager will provide enemy list to ally and register either randomly or nearest enemy as target using own global position and enemy global position.
**/
public partial class AllyManager : Node
{
    public static AllyManager AllyManagerInstance { get; private set; }
    public List<Player> ActiveAlliesList { get; set; }

    public override void _EnterTree()
    {
        // Instantiate the singleton instance of the AllyManager
        AllyManagerInstance = this;
        base._EnterTree();
    }
    // register allied Npcs in the list
}
