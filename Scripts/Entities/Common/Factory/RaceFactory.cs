using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using startup.Scripts.Entities.Base;
using startup.Scripts.Entities.Common;


/// <summary>
/// Creates race objects from seed data and exposes safe lookup methods for gameplay systems.
/// </summary>
/// <remarks>
/// This factory centralizes race loading so all callers get consistent race definitions,
/// resilient fallback behavior, and cloned objects that cannot mutate shared cached data.
/// </remarks>
public static class RaceFactory
{
    // remove when we have db
    private static readonly object LockObject = new();
    private static List<Race> _cachedRaces;
    public static Race Create(string raceName)
    {
        var race = GetRaces().FirstOrDefault(race => string.Equals(race.Name, raceName, StringComparison.OrdinalIgnoreCase));

        return race != null ? Clone(race) : CreateFallbackHuman();
    }

    /// Later implementation will use DB services to get data instead of JSON file, so this method will be removed and replaced by DB call.
    private static List<Race> GetRaces()
    {
        lock (LockObject)
        {
            _cachedRaces ??= LoadRacesFromSeed();
            return _cachedRaces;
        }
    }

    /// Loads race templates from the JSON seed file.
    /// String enum conversion is enabled because seed data stores enum names like "Human".
    /// Later implementation will use DB services to get data instead of JSON file, so this method will be removed and replaced by DB call.
    /// Will me used in migrations
    private static List<Race> LoadRacesFromSeed()
    {
        try
        {
            var absolutePath = ProjectSettings.GlobalizePath(Constants.RacesSeedPath);
            if (!File.Exists(absolutePath))
            {
                GD.PushWarning($"Race seed file not found at {absolutePath}. Falling back to Human defaults.");
                return [CreateFallbackHuman()];
            }

            var json = File.ReadAllText(absolutePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            options.Converters.Add(new JsonStringEnumConverter());
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

    /// <summary>
    /// Creates a guaranteed-valid default race used as a safety fallback.
    /// </summary>
    /// <returns>A baseline Human race definition.</returns>
    /// <remarks>
    /// Fallback data keeps systems operational when seed loading fails,
    /// reducing startup hard-fail risks from content issues.
    /// </remarks>
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

    // to be implemented later
    public static Race Create(RacialTypes type)
    {
        var race = GetRaces().FirstOrDefault(race => race.RacialType == type);

        return race != null ? Clone(race) : CreateFallbackHuman();
    }

    /// Not used now but on charater creation screen we will need to show all races
    public static IReadOnlyList<Race> GetAll()
    {
        return GetRaces().Select(Clone).ToList();
    }
}
