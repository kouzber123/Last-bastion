using System;
using System.Linq;
using Godot;
using startup.Scripts.Interfaces;
using startup.Scripts.Entities.Base;

/// <summary>
///  Player actions pure plain c# to handle activiites of player in game
///  method > user currently doing / acting / processing
/// </summary>
public class PlayerActions : IPlayerActions
{

    private readonly Player player;
    private MovementCommand lastCommand;
    public MovementCommand LastCommand => lastCommand;
    public PlayerActions(Player player)
    {
        this.player = player;
    }


    // calculate intent with player
    // in order to get enemy jump
    // player Y is higher than enemy Y  and enemy is near player
    // if enemy is stuck 
    public MovementCommand CalculateMovement(Vector3 inputDirection, float speed, bool isJumping)
    {
        lastCommand = new MovementCommand
        {
            Direction = inputDirection,
            Speed = speed + (player.Agility * 0.2f),
            Jump = isJumping && player.Stamina > 0
        };
        return lastCommand;
    }
    public bool IsRunning => lastCommand.Direction != Vector3.Zero;
    public bool IsJumping => lastCommand.Jump;

    public void CastMagic(string spellName, IDamageable target)
    {
        var spell = player.SpellBook.FirstOrDefault(magic =>
            string.Equals(magic.Name, spellName, StringComparison.OrdinalIgnoreCase));

        if (spell == null)
        {
            GD.Print($"Unknown spell: {spellName}");
            return;
        }

        if (player.Mana < spell.ManaCost)
        {
            GD.Print($"Not enough mana to cast {spell.Name}");
            return;
        }

        player.Mana -= spell.ManaCost;
        spell.Cast(target);
    }

    public void Dead()
    {

        // handle player death 
        // setting property to dead then controller will handle the
        //  rest of the death process like playing animation,
        //  dropping items, etc
        throw new NotImplementedException();
    }
    public DamageResult ApplyDamage(float amount)
    {
        player.Health -= amount;
        return new DamageResult { RemainingHealth = player.Health, IsDead = player.Health <= 0f };
    }
    public void EquipArmor(IArmor armor)
    {
        throw new NotImplementedException();
    }

    public void EquipItem(IWeapon weapon)
    {
        throw new NotImplementedException();
    }

    public void InitializeClassAndStats(JobClass playerClass)
    {
        if (playerClass == null)
        {
            GD.Print("Player class is null. Cannot initialize stats.");
            return;
        }

        player.Mana += playerClass.Mana;
        player.Strength += playerClass.Strength;
        player.Agility += playerClass.Agility;
        player.Stamina += playerClass.Stamina;

        GD.Print($"Initialized player stats based on class {playerClass.Name}: Mana={player.Mana}, Strength={player.Strength}, Agility={player.Agility}, Stamina={player.Stamina}");
        player.AttackDamage += player.Strength * 2; // Example: Strength increases attack damage
        player.MaxHealth += player.Stamina * 10; // Example: Stamina increases max health
        player.Health = player.MaxHealth; // Set current health to max after initialization
        player.SelectedClass = playerClass; // Store the selected class in the player for future reference
        GD.Print($"After applying class bonuses: Health={player.Health}/{player.MaxHealth}, AttackDamage={player.AttackDamage}");
    }

    public void InitializeRaceAndStats(Race race)
    {
        if (race == null)
        {
            GD.Print("Player race is null. Cannot initialize race modifiers.");
            return;
        }

        player.Mana += race.ManaModifier;
        player.Strength += race.StrengthModifier;
        player.Agility += race.AgilityModifier;
        player.Stamina += race.StaminaModifier;

        player.AttackDamage += race.StrengthModifier;
        player.MaxHealth += race.StaminaModifier * 5;
        player.Health = player.MaxHealth;
        player.SelectedRace = race;

        GD.Print($"Applied race {race.Name}: Mana={player.Mana}, Strength={player.Strength}, Agility={player.Agility}, Stamina={player.Stamina}");
    }

    public void LevelUp()
    {
        throw new NotImplementedException();
    }

    public void Move(Vector3 direction, float speed)
    {
        throw new NotImplementedException();
    }

    public void PickupItem(IWeapon weapon)
    {
        throw new NotImplementedException();
    }

    public void SelectClass(string className)
    {
        var selected = JobClassFactory.Create(className);
        if (!string.Equals(selected.Name, className, StringComparison.OrdinalIgnoreCase))
        {
            GD.Print($"Unknown class: {className}. Defaulting to Warrior.");
        }

        player.SelectedClass = selected;
    }

    public void SelectRace(string raceName)
    {
        var selected = RaceFactory.Create(raceName);
        if (!string.Equals(selected.Name, raceName, StringComparison.OrdinalIgnoreCase))
        {
            GD.Print($"Unknown race: {raceName}. Defaulting to Human.");
        }

        player.SelectedRace = selected;
    }

    public void TakeDamage(float amount)
    {
        GD.Print($"Player takes {amount} damage.");
        player.Health -= amount;
        if (player.Health <= 0f)
        {
            Dead();
        }
    }

    public void UnequipItem(Guid itemId)
    {
        throw new NotImplementedException();
    }
}

