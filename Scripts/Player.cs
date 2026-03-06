using Godot;
using System;



public partial class Player : RigidBody2D
{

	[Signal] public delegate void BoostUIEventHandler(float boostValue);
	[Signal] public delegate void UIEventHandler(Vector2 pos);


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
	[Export] public float minimumDriftAngle = 5;
	[Export] public float minimumDriftVelocity = 5;
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


	float Normalize(float angle) =>
	angle - 2* (float)Math.PI * (float)Math.Floor(angle / (2 * (float)Math.PI));

	void BoostController(double delta, Vector2 forwardDir){

		//If boost cooldown active, count down the time left
		if(boostTimerActive && (nitrousCooldownTimeLeft > 0)){
			nitrousCooldownTimeLeft -= (float)delta;
		}

		//Find out how similar our faced direction is to our moving direction
		float driftAmount = Normalize(forwardDir.Angle()) - Normalize(LinearVelocity.Angle());
		if (driftAmount > (float)Math.PI) driftAmount -= 2*(float)Math.PI;
		else if (driftAmount < -(float)Math.PI) driftAmount += 2*(float)Math.PI;

		driftAmount = Math.Abs(driftAmount);


		//Check we are drifitng hard enough (big enough difference in angle)
		if(driftAmount < 3.14 - minimumDriftAngle){ 
			//Checking we are going fast enough 
			//if(LinearVelocity.Length() > minimumDriftVelocity)
				driftGuage += (driftMultiplier * (3.14f - driftAmount) * (LinearVelocity.Length() * 0.01f));
		}
		GD.Print(driftAmount + ", " + (3.14f - minimumDriftAngle) + ", " + LinearVelocity.Length());

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
		EmitSignal(SignalName.BoostUI, driftGuage);

		EmitSignal(SignalName.UI, GlobalPosition);
	}
}
