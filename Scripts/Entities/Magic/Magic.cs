using System;
using startup.Scripts.Interfaces;

public enum MagicEffectType
{
    Damage,
    Heal
}

public class Magic : Equippable, IMagic
{
    public Magic()
    {
        Id = Guid.NewGuid();
    }

    public string Description { get; set; }
    public DamageType MagicType { get; set; } = DamageType.Magic;
    public MagicEffectType EffectType { get; set; } = MagicEffectType.Damage;
    public int ManaCost { get; set; }
    public float EffectStrength { get; set; } = 1f;

    public void Cast(IDamageable target)
    {
        var amount = Damage * Math.Max(EffectStrength, 1f);

        if (EffectType == MagicEffectType.Heal)
        {
            target.Heal(amount);
            return;
        }

        target.TakeDamage(amount);
    }
}