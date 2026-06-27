using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using startup.Scripts.Entities.Base;
using startup.Scripts.Entities.Common;


/// <summary>
/// Todo list
/// refactor to use db ie the json > read json file for jobclass
/// vvreate real methods so we can later convert to db calls
/// 
/// </summary>
public static class JobClassFactory
{

    private static List<JobClass> _cachedJobClasses { get; set; }
    public static JobClass Create(string className)
    {
        // get the first jobclass item that equals to user given value, if not found return fallback to warrior
        var jobClass = getJobClasses().FirstOrDefault(jobClass => jobClass.Name.Equals(className, StringComparison.OrdinalIgnoreCase));
        return jobClass != null ? Clone(jobClass) : FallbackToWarrior();
    }

    private static List<JobClass> getJobClasses()
    {

        _cachedJobClasses ??= LoadSeedData();
        return _cachedJobClasses;
    }

    // Loads json data into list and return list of classes
    private static List<JobClass> LoadSeedData()
    {
        try
        {
            var absolutePath = ProjectSettings.GlobalizePath(Constants.JobClassesSeedPath);
            if (!File.Exists(absolutePath))
            {
                GD.PushWarning($"Job class seed file not found at {absolutePath}. Falling back to default Warrior class.");
                return [FallbackToWarrior()];
            }
            // Read the JSON seed file and deserialize it into a list of JobClass objects
            var json = File.ReadAllText(absolutePath);
            // add json options 
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            // add enum converter as json classes are using strings
            options.Converters.Add(new JsonStringEnumConverter());
            var jobClasses = JsonSerializer.Deserialize<List<JobClass>>(json, options);
            if (jobClasses == null || jobClasses.Count == 0)
            {
                GD.PushWarning($"Job class seed file at {absolutePath} is empty or invalid. Falling back to default Warrior class.");
                return [FallbackToWarrior()];
            }
            return jobClasses;
        }
        catch (Exception ex)
        {
            GD.PushError($"Error loading job class seed data: {ex.Message}. Falling back to default Warrior class.");
            return [FallbackToWarrior()];
        }
    }

    private static JobClass Clone(JobClass jb)
    {

        return new JobClass
        {
            Id = jb.Id,
            Name = jb.Name,
            Type = jb.Type,
            Description = jb.Description,
            Mana = jb.Mana,
            Strength = jb.Strength,
            Agility = jb.Agility,
            Stamina = jb.Stamina

        };
    }

    private static JobClass FallbackToWarrior()
    {
        GD.PushWarning($"Job class not found. Falling back to default Warrior class.");
        return new JobClass
        {
            Id = Guid.NewGuid(),
            Name = "Warrior",
            Type = JobClassTypes.Warrior,
            Description = "A strong melee fighter with high health and defense.",
            Mana = 1,
            Strength = 5,
            Agility = 2,
            Stamina = 5
        };
    }
}