using System.Collections.Generic;

public enum DamageType { Physical, Fire, Ice, Magic, Holy, Dark } // extend as time goes on
public class Stats
{
    public int Mana { get; set; } = 1;
    public int Strength { get; set; } = 1;
    public int Agility { get; set; } = 1;
    public int Stamina { get; set; } = 1;
    public Dictionary<DamageType, float> Resistances { get; } = [];
    public float DamageMultiplier(DamageType type)
    {
        return Resistances.TryGetValue(type, out var resistance) ? resistance : 1f;
    }


}
