using Godot;
using System;

public partial class Zombie : RigidBody2D
{
	[Export]
	Node2D player;
	[Export]
	public float speed = 10f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (player != null)
		{
			Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
			ApplyForce(direction * speed);
		}
	}
}
