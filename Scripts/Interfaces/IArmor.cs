namespace startup.Scripts.Interfaces
{
    // This interface defines the contract for any armor in the game, such as player armor, enemy armor, etc.
    public interface IArmor
    {
        public string Name { get; }
        public int DefenseValue { get; }

        // Behavior method
        int ReduceDamage(int incomingDamage);
    }
}