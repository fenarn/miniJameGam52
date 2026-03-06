using Godot;
using System;



public partial class Player : RigidBody2D
{

	[Export] public float accelImpulseSpeed = 1;
	[Export] public float decelImpulseSpeed = 1;
	[Export] public float rotateImpulseSpeed = 1;





	[Export] public float nitrousImpulseSpeed = 1;
	[Export] public float nitrousTimeSec = 1;
	bool nitrousBoost = false;
	float nitrousTimeLeft = 1;

	[Export] public bool boostTimerActive = false;
	[Export] public float nitrousCooldownSec = 5;
	float nitrousCooldownTimeLeft = 0;

	[Export] public bool boostDriftActive = false;
	[Export] public float minimumDriftActive = 5;
	[Export] public float driftMultiplier = 5;
	float driftGuage = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("nitrous"))
		{
			//Check to see if the timer cooldown has ended
			if(boostTimerActive && (nitrousCooldownTimeLeft > 0)){
				return;
			}
			//Check to see if enough drifting has happened
			if(boostDriftActive){
				if(driftGuage < 1){
					return;	
				}else{
					driftGuage = 0;
				}
			}

			nitrousTimeLeft = nitrousTimeSec;
			nitrousBoost = true;
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


		

		//Boost Controls
		BoostController(delta, forwardDir);
	}



	void BoostController(double delta, Vector2 forwardDir){

		//If boost cooldown active, count down the time left
		if(boostTimerActive && (nitrousCooldownTimeLeft > 0)){
			nitrousCooldownTimeLeft -= (float)delta;
		}

		//Calculate boost addition from drifting
		float driftAmount = Math.Abs(Rotation - LinearVelocity.Angle());
		if(driftAmount > minimumDriftActive){
			driftGuage += driftAmount * driftMultiplier;
		}
		


		//Decide whether to apply the boost
		if(nitrousBoost){

			if((nitrousTimeLeft < 0)){
				//If boost cooldown active, Disable the nitro, and set the cooldown timer for when to next allow nitro
				nitrousBoost = false;
				nitrousCooldownTimeLeft = nitrousCooldownSec;
			}

			nitrousTimeLeft -= (float)delta;
			ApplyImpulse(-forwardDir * nitrousImpulseSpeed, Vector2.Zero);
		}

		
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
