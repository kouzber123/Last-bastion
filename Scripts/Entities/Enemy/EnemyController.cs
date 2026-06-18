using System.Collections.Generic;
using Godot;

public partial class EnemyController : CharacterBody3D, IDamageable
{
    [Export] public Node TargetNode;

    [Export]
    private float speed = 1f;
    [Export] private PackedScene _damageScene;
    private EnemyActions enemyActions;
    private Vector3 _velocity;
    // simulate get Enemy 
    private AnimationPlayer animEnemyPlayer = null;
    // get Enemy actions
    private Node3D characterModel;
    private Enemy Enemy = null;
    [Export] public float RotationSpeed { get; set; } = 5f;
    public Vector3 MoveDirection { get; set; } = Vector3.Zero;


    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
    {
        base._Ready();
        InitEnemy();

    }
    public void UpdateEnemyPhysics(double delta, Vector3 targetPosition, float distanceToEnemy, Vector3 separationForce)
    {
        TryFindTarget();
        ApplyGravity(Enemy.Gravity, delta);
        /*  CommandRotation(characterModel, MoveDirection, RotationSpeed); */
        MoveCommand(CommandListener(targetPosition, distanceToEnemy));
        GetAnimation();
        // 2. Blend the separation force into the horizontal velocity!
        float pushWeight = 3.5f; // Increase this number if they blend/overlap too much

        // Final velocity is a combination of their normal movement towards the Enemy and the separation force pushing them away from neighbors. The manager calculates the separation force based on nearby enemies and passes it in here, so you don't have to worry about it in the Enemy script!
        Vector3 finalVelocity = Velocity;

        // Add separation force to the horizontal velocity (X and Z axes)
        finalVelocity.X += separationForce.X * pushWeight;
        finalVelocity.Z += separationForce.Z * pushWeight;
        Velocity = finalVelocity;
        MoveAndSlide();
    }

    public override void _ExitTree()
    {
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.UnregisterEnemy(this);
        }
    }

    // from manager

    public MovementCommand CommandListener(Vector3 target, float distanceToEnemy)
    {
        var direction = FollowTarget(target);
        return enemyActions.CalculateMovement(direction, speed, distanceToEnemy <= 6);
    }
    public void MoveCommand(MovementCommand cmd)
    {
        GetAnimation();
        if (cmd.Jump && IsOnFloor())
        {
            TryJump(cmd);
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
        TryMoving(cmd);

    }
    public void TryMoving(MovementCommand cmd)
    {

        var moveDirection = cmd.Direction /* current direction enemy is moving */;
        var random = new RandomNumberGenerator();
        if (CanSeeGround(moveDirection))
        {
            // Ground ahead, randomly jump
            Velocity = new Vector3(cmd.Direction.X * cmd.Speed, Velocity.Y, cmd.Direction.Z * cmd.Speed);
        }
        else
        {
            Velocity = Vector3.Zero;
        }
        return;
    }
    public void TryJump(MovementCommand cmd)
    {
        if (!IsOnFloor()) return;  // Can't jump if airborne

        var moveDirection = cmd.Direction /* current direction enemy is moving */;
        var random = new RandomNumberGenerator();

        if (CanSeeGround(moveDirection) && random.Randf() > 0.7f)
        {
            // Ground ahead, randomly jump
            Velocity += Vector3.Up * cmd.Speed;
        }
    }
    private bool CanSeeGround(Vector3 direction)
    {
        return CastGroundRay(direction, 2f) &&
               CastGroundRay(direction.Rotated(Vector3.Up, 0.3f), 2f) &&
               CastGroundRay(direction.Rotated(Vector3.Up, -0.3f), 2f);
    }
    private bool CastGroundRay(Vector3 direction, float lookAhead)
    {
        if (direction == Vector3.Zero)
            return true; // no forward direction to check

        var rayStart = GlobalPosition + Vector3.Up * 0.5f + direction.Normalized() * lookAhead;
        var rayEnd = rayStart + Vector3.Down * 5f;

        return CastRay(rayStart, rayEnd);
    }
    private bool CastRay(Vector3 from, Vector3 to)
    {
        /*
 position = where the ray hit
 normal = surface direction at the hit point
 collider = the object that was hit
 distance = how far along the ray the hit happened
 shape_index = which shape on the collider was hit

 collider → identify the hit object
 ground, wall, obstacle, player, door, etc.
 normal → understand surface orientation
 normal.Y > 0.8 → floor/ground
 normal.X or normal.Z large → wall/obstacle
 distance → know how close the obstacle is
 close = avoid now
 far = keep going
 */
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        var result = GetWorld3D().DirectSpaceState.IntersectRay(query);
        return result.Count > 0;
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
    public void InitEnemy()
    {
        //register actions
        SetPhysicsProcess(false);
        SetProcess(false);
        AddToGroup("enemies");
        var _model = GetNodeOrNull<Node3D>("Skin");
        if (_model != null) characterModel = _model;
        animEnemyPlayer = GetNodeOrNull<AnimationPlayer>("Skin/AnimationPlayer");
        Enemy = new Enemy
        {
            Name = "EVIL skeletor",
            Stamina = 10,
            Strength = 10,
            Mana = 10,
            Agility = 10,
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
        GD.Print("My name is " + Enemy.Name, " i came to destroy the world");
        enemyActions = new EnemyActions(Enemy);
        enemyActions.SelectRace("Undead");
        enemyActions.InitializeRaceAndStats(Enemy.SelectedRace);
        enemyActions.SelectClass("Warrior");
        enemyActions.InitializeClassAndStats(Enemy.SelectedClass);
        if (_damageScene != null)
        {
            var damageArea = _damageScene.Instantiate<ContactDamageArea>();
            var collisionNode = GetNodeOrNull<Node3D>("CollisionShape3D");
            damageArea.DamageAmount = Enemy.AttackDamage;
            damageArea.Scale = collisionNode.Scale;
            AddChild(damageArea);
        }
        EnemyManager.Instance.RegisterEnemy(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        speed = 1f + (Enemy.Agility * 0.5f); // Manipulate speed based on stats
    }
    public void TakeDamage(float damage)
    {
        /*  GD.Print($"Enemy takes {damage} damage."); */
        var result = enemyActions.ApplyDamage(damage);
        if (result.IsDead)
        {
            Dead();
        }
    }
    public void TryFindTarget()
    {
        // Enemy manager will pass target for this Enemy
        var players = GetTree().GetNodesInGroup("player");
        if (players.Count > 0)
        {
            TargetNode = players[0] as CharacterBody3D;
        }
    }

    public Vector3 FollowTarget(Vector3 targetPosition)
    {

        if (TargetNode == null) return Vector3.Zero;
        Vector3 directionToPlayer = targetPosition - GlobalPosition;
        directionToPlayer.Y = 0;

        if (directionToPlayer.Length() > 0)
        {
            directionToPlayer = directionToPlayer.Normalized();
        }
        return new Vector3(directionToPlayer.X, 0, directionToPlayer.Z).Normalized();
        /*     Velocity = new Vector3(MoveDirection.X * speed, Velocity.Y, MoveDirection.Z * speed); */

    }

    // not used for ai
    /*     public Vector3 GetInputDirection()
        {
            // read input and return 2d of the movevemnt
            Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_down", "move_up");
            Vector3 direction = new(inputDirection.X, 0, -inputDirection.Y);
            return direction;
        } */
    public void GetAnimation()
    {
        var animation = new Animation(animEnemyPlayer);
        /*  GD.Print("are we running " + enemyActions.IsRunning); */
        if (enemyActions.IsJumping) animation.Jump();
        if (enemyActions.IsRunning) animation.Run();
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

    private void Dead()
    {
        GD.Print("Player died.");
        QueueFree();
        Visible = false;
    }

    public void Die()
    {
        Dead();
    }
}