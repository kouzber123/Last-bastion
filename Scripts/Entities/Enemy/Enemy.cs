using System;
using System.Collections.Generic;
using startup.Scripts.Entities.Base;


public class Enemy
{
	private Guid Id { get; set; } = new Guid();
	public string Name { get; set; }
	public int Level { get; set; } = 1;
	public float MaxHealth { get; set; } = 100f;
	public float Health { get; set; }
	public float AttackDamage { get; set; } = 10f;
	public float JumpForce { get; set; } = 10f;
	public string PlayerName { get; set; }
	public float Gravity { get; set; } = 9.8f;
	public int Mana { get; set; } = 2;
	public int Strength { get; set; } = 2;
	public int Agility { get; set; } = 2;
	public int Stamina { get; set; } = 2;
	public Race SelectedRace { get; set; }
	public JobClass SelectedClass { get; set; }
	public ICollection<Magic> SpellBook { get; set; } = [];
	public ICollection<Item> Inventory { get; set; } = [];

}
