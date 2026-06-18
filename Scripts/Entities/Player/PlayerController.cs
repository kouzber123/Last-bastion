// <summary>
// PlayerController class to manage player actions and interactions in the game.
using Godot;
using System.Collections.Generic;

// Player controller is the parent node (prev: Player)
// Player is going to be entity that will be called here
public partial class PlayerController : CharacterBody3D, IDamageable
{

    [Export]
    private float speed = 2f;
    [Export] private PackedScene _damageScene;
    private PlayerActions playerActions;
    private Vector3 _velocity;
    // simulate get player 
    private AnimationPlayer animPlayerNode = null;
    // get player actions
    private Node3D characterModel;
    private Player player = null;
    [Export] public float RotationSpeed { get; set; } = 5f;

    // processors
    public override void _Ready()
    {
        base._Ready();
        // load player
        InitPlayer();


    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        ApplyGravity(player.Gravity, delta);
        GetAnimation();
        MoveCommand(CommandListener());
        MoveAndSlide();
    }
    public override void _EnterTree()
    {
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
    }

    // commands
    public MovementCommand CommandListener()
    {
        return playerActions.CalculateMovement(GetInputDirection(), speed, Input.IsActionJustPressed("Jump"));
    }
    public void MoveCommand(MovementCommand cmd)
    {


        if (cmd.Jump && IsOnFloor())
        {
            Velocity += Vector3.Up * cmd.Speed;
        }

        if (cmd.Direction == Vector3.Zero)
        {
            Velocity = new Vector3(0, Velocity.Y, 0);
            // animation.Stop();
            return;
        }
        if (cmd.Direction.Length() > 0)
        {
            CommandRotation(characterModel, cmd.Direction);
        }

        Velocity = new Vector3(cmd.Direction.X * cmd.Speed, Velocity.Y, cmd.Direction.Z * cmd.Speed);
    }



    public void CommandRotation(Node3D characterModel, Vector3 moveDirection)
    {
        CommandRotation(characterModel, moveDirection, RotationSpeed);
    }
    public void CommandRotation(Node3D characterModel, Vector3 moveDirection, float rotationSpeed)
    {
        float targetAngle = Mathf.Atan2(moveDirection.X, moveDirection.Z);
        float smoothAngle = Mathf.LerpAngle(
            characterModel.Rotation.Y,
            targetAngle,
            (float)(rotationSpeed * GetProcessDeltaTime())
        );
        characterModel.Rotation = new Vector3(characterModel.Rotation.X, smoothAngle, characterModel.Rotation.Z);
    }
    public void InitPlayer()
    {
        //register actions
        AddToGroup("player");
        var _model = GetNodeOrNull<Node3D>("Skin");
        if (_model != null) characterModel = _model;
        animPlayerNode = GetNodeOrNull<AnimationPlayer>("Skin/AnimationPlayer");
        player = new Player
        {
            Name = "Helios",
            Stamina = 40,
            Strength = 50,
            Mana = 50,
            Agility = 30,
            SpellBook = new List<Magic>
            {
                new()
                {
                    Name = "Fireball",
                    Description = "A fiery projectile that damages the target.",
                    MagicType = DamageType.Fire,
                    EffectType = MagicEffectType.Damage,
                    Damage = 10f,
                    EffectStrength = 1f,
                    ManaCost = 20,
                    Cooldown = 2f
                },
                new()
                {
                    Name = "Heal",
                    Description = "A restorative spell that heals the target.",
                    MagicType = DamageType.Holy,
                    EffectType = MagicEffectType.Heal,
                    Damage = 10f,
                    EffectStrength = 1f,
                    ManaCost = 4,
                    Cooldown = 2f
                }
            }
        };
        GD.Print("My name is " + player.Name, " i came to save the world");
        playerActions = new PlayerActions(player);
        playerActions.SelectRace("Human");
        playerActions.InitializeRaceAndStats(player.SelectedRace);
        playerActions.SelectClass("Warrior");
        playerActions.InitializeClassAndStats(player.SelectedClass);
        if (_damageScene != null)
        {
            var damageArea = _damageScene.Instantiate<ContactDamageArea>();
            var collisionNode = GetNodeOrNull<Node3D>("CollisionShape3D");
            damageArea.DamageAmount = player.AttackDamage * 999;
            damageArea.Scale = collisionNode.Scale;
            AddChild(damageArea);
        }
    }
    public void TakeDamage(float amount)
    {
        GD.Print($"Player takes {amount} damage.");
        var result = playerActions.ApplyDamage(amount);
        if (result.IsDead) Dead();

    }
    private void Dead()
    {
        GD.Print("Player died.");
        QueueFree();
        Visible = false;
    }
    // getters
    public Vector3 GetInputDirection()
    {
        // read input and return 2d of the movevemnt
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_down", "move_up");
        Vector3 direction = new(inputDirection.X, 0, -inputDirection.Y);
        return direction;
    }
    public void GetAnimation()
    {
        var animation = new Animation(animPlayerNode);
        if (playerActions.IsJumping) animation.Jump();
        if (playerActions.IsRunning) animation.Run();
        else animation.Stop();

    }

    public virtual void ApplyGravity(float gravity, double delta)
    {
        Velocity += Vector3.Down * gravity * (float)delta;
    }

    public void Heal(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        Dead();
    }

    // refactor later 

}
