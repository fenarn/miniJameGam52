using Godot;
using System;

public partial class Zombie : RigidBody2D
{
	[Export]
	RigidBody2D player;
	[Export]
	public float thrust = 10f;
	[Export]
	public float torqueStrength = 0.5f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (player != null)
		{
			Vector2 target = (player.GlobalPosition - GlobalPosition);
			Vector2 direction = target.Normalized();
			Vector2 forward = -Transform.Y;

			float angle = forward.AngleTo(direction);

			Vector2 diff = forward - direction;

			float angle2 = direction.Angle();

			//GD.Print(target);
			//GD.Print(direction);
			GD.Print(angle2);
			//GD.Print(angle);
			

			
			/*ApplyTorqueImpulse(angle * torqueStrength);
			ApplyImpulse(forward * thrust);*/

			ApplyForce(direction * thrust);
		}
	}
}
