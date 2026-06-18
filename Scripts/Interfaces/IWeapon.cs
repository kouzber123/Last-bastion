namespace startup.Scripts.Interfaces
{
    // This interface defines the contract for any weapon in the game, such as melee weapons, ranged weapons, etc.
    public interface IWeapon
    {
        public string Name { get; }
        public string Type { get; }
        public string Description { get; }
        public int Damage { get; }
        public int Range { get; }

        // Behavior methods
        void Use(IDamageable target);
        bool CanUse();
    }
}