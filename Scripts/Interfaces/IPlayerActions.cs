// <summary>
// Interface for player actions in the game.
// Methods to run play on game
using System;
using Godot;
using startup.Scripts.Interfaces;
using startup.Scripts.Entities.Base;

public interface IPlayerActions
{
    void EquipItem(IWeapon weapon);
    void EquipArmor(IArmor armor);
    void UnequipItem(Guid itemId);
    void Move(Vector3 direction, float speed);
    void LevelUp();
    void CastMagic(string spellName, IDamageable target);
    void PickupItem(IWeapon weapon);
    void SelectClass(string className);
    void SelectRace(string raceName);
    void InitializeClassAndStats(JobClass jobClass);
    void InitializeRaceAndStats(Race race);
    void TakeDamage(float amount);
    public DamageResult ApplyDamage(float amount);
    void Dead();
    public MovementCommand CalculateMovement(Vector3 inputDirection, float speed, bool isJumping);

}
