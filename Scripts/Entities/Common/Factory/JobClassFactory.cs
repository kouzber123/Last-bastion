using System;

public static class JobClassFactory
{
    public static JobClass Create(string className)
    {
        return className?.Trim().ToLowerInvariant() switch
        {
            "warrior" => new JobClass
            {
                Id = Guid.NewGuid(),
                Name = "Warrior",
                Type = "Warrior",
                Description = "Warriors are heavily armored fighters who excel in close combat. They have high health and can withstand significant damage while dealing powerful attacks.",
                Mana = 1,
                Strength = 7,
                Agility = 1,
                Stamina = 5
            },
            "rogue" => new JobClass
            {
                Id = Guid.NewGuid(),
                Name = "Rogue",
                Type = "Rogue",
                Description = "Rogues are agile and stealthy characters, excelling in quick attacks and evasion. They rely on dexterity and cunning to outmaneuver their opponents.",
                Mana = 1,
                Strength = 3,
                Agility = 7,
                Stamina = 3
            },
            "mage" => new JobClass
            {
                Id = Guid.NewGuid(),
                Name = "Mage",
                Type = "Mage",
                Description = "Mages are spellcasters who harness arcane energy. They are typically frail but possess powerful magical abilities.",
                Mana = 7,
                Strength = 1,
                Agility = 3,
                Stamina = 2
            },
            _ => new JobClass
            {
                Id = Guid.NewGuid(),
                Name = "Warrior",
                Type = "Warrior",
                Description = "Warriors are heavily armored fighters who excel in close combat. They have high health and can withstand significant damage while dealing powerful attacks.",
                Mana = 1,
                Strength = 7,
                Agility = 1,
                Stamina = 5
            }
        };
    }
}