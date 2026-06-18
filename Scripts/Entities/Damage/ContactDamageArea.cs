using Godot;

public partial class ContactDamageArea : Area3D
{
    [Export] public float DamageAmount = 10f;
    [Export] public float DamageInterval = 0.5f;
    [Export] public Node ExcludeTarget; // Optional: exclude a specific node from being damaged
    [Export] public bool ExcludeSelf = true;

    private IDamageable _target;
    private float _damageTimer = 0f;

    public override void _Ready()
    {
        if (ExcludeSelf && ExcludeTarget == null)
        {
            ExcludeTarget = GetParent() ?? GetOwner();
        }
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null) return;

        _damageTimer -= (float)delta;
        if (_damageTimer <= 0f)
        {
            _target.TakeDamage(DamageAmount);
            _damageTimer = DamageInterval;
        }
    }

    private void OnBodyEntered(Node body)
    {
        // Don't damage if it's the excluded target
        // ExcludeTarget != null && body == ExcludeTarget ||
        // is in same > Exclude target is self and body is in same group as self
        var isInSameGroup = ExcludeTarget != null && body.IsInGroup(ExcludeTarget.GetGroups()[0]);
        if (ExcludeTarget != null && body == ExcludeTarget || isInSameGroup)
            return;

        // Check if this body can take damage - works for any IDamageable
        var damageable = body as IDamageable;
        GD.Print($"Body is damageable: {damageable}");
        if (damageable != null)
        {
            GD.Print($"Damaging: {damageable}");
            _target = damageable;
            _damageTimer = 0f; // immediate first damage
        }
    }

    private void OnBodyExited(Node body)
    {
        var damageable = body as IDamageable;
        if (damageable == _target)
        {
            _target = null;
        }
    }
}