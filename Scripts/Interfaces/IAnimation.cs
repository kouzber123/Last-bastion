using Godot;

namespace startup.Scripts.Interfaces
{
    public interface IAnimation
    {
        void Run();
        void Stop();
        void Jump();

        AnimationPlayer GetAnimationPlayer();

    }
}