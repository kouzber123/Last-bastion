namespace startup.Scripts.Interfaces
{
    // This interface defines the contract for any magic-related functionality in the game, such as spells, abilities, etc.
    /*
    Summary:
    Magic is core system of all magic spells, all common functionality of magic spells should be defined here, such as:
    handle
        - spell casting
        - spell effects > maybe not here but in main class as abstract class
        - spell type, fire, ice, lightning, etc.
        - mana management
        - cooldowns
        - interactions with other systems (e.g., combat, character stats)    
    
    */
    public interface IMagic
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DamageType MagicType { get; set; }
        public MagicEffectType EffectType { get; set; }
        public int ManaCost { get; set; }
        public float Cooldown { get; set; }
        public float Damage { get; set; } // or
        public float EffectStrength { get; set; } // for non-damage spells

        // cast a spell against a target that can take damage
        public void Cast(IDamageable target);
    }
}