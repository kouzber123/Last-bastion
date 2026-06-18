using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using startup.Scripts.Entities.Base;
using startup.Scripts.Entities.Common;


// <summary>
// Factory class for creating Race instances based on predefined templates or seed data.
// Provides methods to create races by name or type, and to retrieve all available races.
public static class RaceFactory
{
    private const string RacesSeedPath = "res://Scripts/Entities/Common/Seeds/Races.json";
    private static readonly object LockObject = new();
    private static List<Race> _cachedRaces;

    public static Race Create(string raceName)
    {
        var races = GetRaces();
        var race = races.FirstOrDefault(r =>
            string.Equals(r.Name, raceName, StringComparison.OrdinalIgnoreCase));

        return race != null ? Clone(race) : CreateFallbackHuman();
    }

    public static Race Create(RacialTypes type)
    {
        var races = GetRaces();
        var race = races.FirstOrDefault(r => r.RacialType == type);

        return race != null ? Clone(race) : CreateFallbackHuman();
    }

    public static IReadOnlyList<Race> GetAll()
    {
        return GetRaces().Select(Clone).ToList();
    }

    private static List<Race> GetRaces()
    {
        lock (LockObject)
        {
            _cachedRaces ??= LoadRacesFromSeed();
            return _cachedRaces;
        }
    }

    private static List<Race> LoadRacesFromSeed()
    {
        try
        {
            var absolutePath = ProjectSettings.GlobalizePath(RacesSeedPath);
            if (!File.Exists(absolutePath))
            {
                GD.PushWarning($"Race seed file not found at {absolutePath}. Falling back to Human defaults.");
                return [CreateFallbackHuman()];
            }

            var json = File.ReadAllText(absolutePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var races = JsonSerializer.Deserialize<List<Race>>(json, options);

            if (races == null || races.Count == 0)
            {
                GD.PushWarning("Races seed file is empty. Falling back to Human defaults.");
                return [CreateFallbackHuman()];
            }

            return races;
        }
        catch (Exception ex)
        {
            GD.PushWarning($"Failed to load races seed: {ex.Message}. Falling back to Human defaults.");
            return [CreateFallbackHuman()];
        }
    }

    private static Race Clone(Race race)
    {
        return new Race
        {
            Id = race.Id,
            RacialType = race.RacialType,
            Name = race.Name,
            Description = race.Description,
            ManaModifier = race.ManaModifier,
            StrengthModifier = race.StrengthModifier,
            AgilityModifier = race.AgilityModifier,
            StaminaModifier = race.StaminaModifier,
            Strengths = [.. race.Strengths],
            Weaknesses = [.. race.Weaknesses]
        };
    }

    private static Race CreateFallbackHuman()
    {
        return new Race
        {
            Id = Guid.NewGuid(),
            RacialType = RacialTypes.Human,
            Name = "Human",
            Description = "Balanced and adaptable race with no extreme weaknesses.",
            ManaModifier = 1,
            StrengthModifier = 1,
            AgilityModifier = 1,
            StaminaModifier = 1,
            Strengths = ["Balanced"],
            Weaknesses = ["None"]
        };
    }
}
