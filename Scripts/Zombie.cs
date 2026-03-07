using Godot;
using System;

public partial class Zombie : RigidBody2D
{
	[Export]
	RigidBody2D player;
	[Export]
	public float thrust = 10f;
	[Export]
	public float torqueStrength = 1f;
	[Export]
	public float damping = 3f;



	[Export] public bool frozen = false;
	[Export] public float frozenKillTimer = 3f;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(player == null)
		{
			player = GetNode("/root/Scene/PlayerCharacter") as RigidBody2D;			
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if ((player != null) && (frozen == false))
		{
			Vector2 target = (player.GlobalPosition - GlobalPosition);
			Vector2 direction = target.Normalized();
			Vector2 forward = -Transform.Y;

			float angle = forward.AngleTo(direction);

			//This little commented snippet below is gonne be left here as a testament to mine (Fenarn's) humongous stupidity.
			//float angleRotated = Mathf.Wrap(angle + Mathf.Pi / 2f, -Mathf.Pi, Mathf.Pi);
			
			ApplyTorqueImpulse(angle*torqueStrength- AngularVelocity * damping);
			ApplyForce(direction * thrust);
		}
		else if(frozen)
		{
			//If the zombie is frozen start killing them
			frozenKillTimer -= (float)delta;

			if(frozenKillTimer <= 0)
			{
				QueueFree();
			}
		}
	}
}
