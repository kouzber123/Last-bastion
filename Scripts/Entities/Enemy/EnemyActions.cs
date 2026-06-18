using System;
using System.Linq;
using Godot;
using startup.Scripts.Interfaces;
using startup.Scripts.Entities.Base;

public class EnemyActions : IPlayerActions
{
    private readonly Enemy enemy;
    private MovementCommand lastCommand;
    public MovementCommand LastCommand => lastCommand;
    public EnemyActions(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public DamageResult ApplyDamage(float amount)
    {
        enemy.Health -= amount;
        return new DamageResult { RemainingHealth = enemy.Health, IsDead = enemy.Health <= 0f };
    }


    // calculate intent with enemy
    // calculate intent with player
    // in order to get enemy jump
    // player Y is higher than enemy Y  and enemy is near player
    // if enemy is stuck 
    public MovementCommand CalculateMovement(Vector3 inputDirection, float speed, bool isJumping)
    {
        lastCommand = new MovementCommand
        {
            Direction = inputDirection,
            Speed = speed + (enemy.Agility * 0.2f),
            Jump = isJumping && enemy.Stamina > 0
        };
        return lastCommand;
    }
    public bool IsRunning => lastCommand.Direction != Vector3.Zero;
    public bool IsJumping => lastCommand.Jump;
    public void CastMagic(string spellName, IDamageable target)
    {
        var spell = enemy.SpellBook.FirstOrDefault(magic =>
            string.Equals(magic.Name, spellName, StringComparison.OrdinalIgnoreCase));

        if (spell == null)
        {
            GD.Print($"Unknown spell: {spellName}");
            return;
        }

        if (enemy.Mana < spell.ManaCost)
        {
            GD.Print($"Not enough mana to cast {spell.Name}");
            return;
        }

        enemy.Mana -= spell.ManaCost;
        spell.Cast(target);
    }

    public void Dead()
    {

        // handle enemy death 
        // setting property to dead then controller will handle the
        //  rest of the death process like playing animation,
        //  dropping items, etc
        throw new NotImplementedException();
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
            GD.Print("Enemy class is null. Cannot initialize stats.");
            return;
        }

        enemy.Mana += playerClass.Mana;
        enemy.Strength += playerClass.Strength;
        enemy.Agility += playerClass.Agility;
        enemy.Stamina += playerClass.Stamina;

        GD.Print($"Initialized enemy stats based on class {playerClass.Name}: Mana={enemy.Mana}, Strength={enemy.Strength}, Agility={enemy.Agility}, Stamina={enemy.Stamina}");
        enemy.AttackDamage += enemy.Strength * 2; // Example: Strength increases attack damage
        enemy.MaxHealth += enemy.Stamina * 10; // Example: Stamina increases max health
        enemy.Health = enemy.MaxHealth; // Set current health to max after initialization
        enemy.SelectedClass = playerClass; // Store the selected class in the enemy for future reference
        GD.Print($"After applying class bonuses: Health={enemy.Health}/{enemy.MaxHealth}, AttackDamage={enemy.AttackDamage}");
    }

    public void InitializeRaceAndStats(Race race)
    {
        if (race == null)
        {
            GD.Print("Enemy race is null. Cannot initialize race modifiers.");
            return;
        }

        enemy.Mana += race.ManaModifier;
        enemy.Strength += race.StrengthModifier;
        enemy.Agility += race.AgilityModifier;
        enemy.Stamina += race.StaminaModifier;

        enemy.AttackDamage += race.StrengthModifier;
        enemy.MaxHealth += race.StaminaModifier * 5;
        enemy.Health = enemy.MaxHealth;
        enemy.SelectedRace = race;

        GD.Print($"Applied race {race.Name}: Mana={enemy.Mana}, Strength={enemy.Strength}, Agility={enemy.Agility}, Stamina={enemy.Stamina}");
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

        enemy.SelectedClass = selected;
    }

    public void SelectRace(string raceName)
    {
        var selected = RaceFactory.Create(raceName);
        if (!string.Equals(selected.Name, raceName, StringComparison.OrdinalIgnoreCase))
        {
            GD.Print($"Unknown race: {raceName}. Defaulting to Human.");
        }

        enemy.SelectedRace = selected;
    }

    public void TakeDamage(float amount)
    {
        GD.Print($"Enemy takes {amount} damage.");
        enemy.Health -= amount;
        if (enemy.Health <= 0f)
        {
            Dead();
        }
    }

    public void UnequipItem(Guid itemId)
    {
        throw new NotImplementedException();
    }
}
