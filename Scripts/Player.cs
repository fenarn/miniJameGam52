using Godot;
using System;



public partial class Player : RigidBody2D
{

	[Export] public float accelImpulseSpeed = 1;
	[Export] public float decelImpulseSpeed = 1;
	[Export] public float rotateImpulseSpeed = 1;


	[Export] public float nitrousImpulseSpeed = 1;
	[Export] public float nitrousTimeSec = 1;
	[Export] public float nitrousCooldownSec = 5;
	bool nitrousBoost = false;
	float nitrousTimeLeft = 1;
	float nitrousCooldownTimeLeft = 0;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("nitrous"))
		{
			//Check to see if the cooldown has ended
			if(nitrousCooldownTimeLeft <= 0){
				nitrousTimeLeft = nitrousTimeSec;
				nitrousBoost = true;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		// calculate forward direction based on current rotation (local x-axis)
		Vector2 forwardDir = new Vector2(0, 1).Rotated(Rotation);
		if (Input.IsActionPressed("forward"))
		{
			ApplyImpulse(-forwardDir * accelImpulseSpeed, Vector2.Zero);
		}
		if (Input.IsActionPressed("backward"))
		{
			ApplyImpulse(forwardDir * decelImpulseSpeed, Vector2.Zero);
		}
		if (Input.IsActionPressed("right"))
		{
			ApplyTorqueImpulse(rotateImpulseSpeed);
		}
		if (Input.IsActionPressed("left"))
		{
			ApplyTorqueImpulse(-rotateImpulseSpeed);
		}



		//Run the timer for the nitrous, and apply the boost
		if(nitrousBoost){
			nitrousTimeLeft -= (float)delta;

			if(nitrousTimeLeft < 0){
				//Disable the nitro, and set the cooldown timer for when to next allow nitro
				nitrousBoost = false;
				nitrousCooldownTimeLeft = nitrousCooldownSec;
			}

			ApplyImpulse(-forwardDir * nitrousImpulseSpeed, Vector2.Zero);
		}
		if(nitrousCooldownTimeLeft > 0){
			nitrousCooldownTimeLeft -= (float)delta;
		}
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
