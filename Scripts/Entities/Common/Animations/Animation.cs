
using Godot;
using startup.Scripts.Interfaces;

public partial class Animation : CharacterBody3D, IAnimation
{
    public AnimationPlayer AnimationPlayer { get; set; }
    public Animation(AnimationPlayer animationPlayer)
    {
        AnimationPlayer = animationPlayer;
    }

    public AnimationPlayer GetAnimationPlayer()
    {
        return AnimationPlayer;
    }

    public void Run()
    {

        if (AnimationPlayer != null)
        {
            AnimationPlayer.Play("Running_A");
        }


    }

    public void Stop()
    {

        if (AnimationPlayer != null)
        {
            AnimationPlayer.Stop();
        }
    }

    public void Jump()
    {
        if (AnimationPlayer != null)
        {
            AnimationPlayer.Play("Jump_Full_Long");
        }
    }
}
