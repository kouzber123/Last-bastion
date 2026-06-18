using Godot;

public struct MovementCommand
{
    public Vector3 Direction; // normalized horizontal
    public float Speed;       // desired ground speed
    public bool Jump;
}