using Godot;
using System;



public partial class Player : RigidBody2D
{

	[Signal] public delegate void BoostUIEventHandler(float boostValue);
	[Signal] public delegate void WhistleUIEventHandler(float whistleValue);
	[Signal] public delegate void UIEventHandler(Vector2 pos);
	[Signal] public delegate void LogEventHandler(string txt);
	[Signal] public delegate void HealthUIEventHandler(float healthValue);


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
	[Export] public float minimumDriftAngle = 0.3f;
	[Export] public float maximumDriftAngle = 1f;
	[Export] public float minimumDriftVelocity = 5;
	[Export] public float driftMultiplier = 5;
	float driftGuage = 0;
	[Export] public GpuParticles2D flames;



	[Export] public float whistleCoolDown = 5;
	[Export] public Area2D whistleColArea;
	[Export] public float whistleRadiusMod = 50f;
	float whistleCoolLeft = 0;
	bool monitoringDisableDelay = false;

	[Export] float zombieKillSpeed = 50f;

	[Export]
	public int healthPoints;
	[Export]
	public int maxHealthPoints = 50;

	public bool isDead = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		healthPoints = maxHealthPoints;
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
					flames.Emitting = true;
				}
			}

			nitrousTimeLeft = nitrousTimeSec;
			nitrousBoost = true;
		}

		if (@event.IsActionPressed("whistle"))
		{
			//Check to see if the timer cooldown has ended
			//if(whistleCoolLeft > 0){
			//	return;
			//}
			whistleCoolLeft = 0;

			foreach (var obj in GetNode<Area2D>("Area2D").GetOverlappingBodies())
			{
				if(obj is Zombie zombie)
				{
					zombie.FreezeZombie();
				}
			}
			
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if(isDead)
			return;
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


		//Adjust the whistle area collider
		whistleColArea.Scale = new Vector2(whistleCoolLeft * whistleRadiusMod, whistleCoolLeft * whistleRadiusMod);
		

		//Boost Controls
		BoostController(delta, forwardDir);


		if(whistleCoolLeft <= whistleCoolDown)
		{
			whistleCoolLeft += ((float)delta + (whistleCoolDown - whistleCoolLeft) * 0.04f) * 0.3f;
		}

		if(healthPoints <= 0 && !isDead)
		{
			isDead = true; // :(
			GameOver goScreen = GetNode("/root/Scene/UI/GameOver") as GameOver;
			goScreen.Visible = true;
			goScreen.IsPlayerDead = true;
			GD.Print("imma dead baus T_T");
		}
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
		if((3.14 - driftAmount > minimumDriftAngle) && (3.14 - driftAmount < maximumDriftAngle) && (driftAmount > 0)){ 
			//Checking we are going fast enough 
			if(LinearVelocity.Length() > minimumDriftVelocity)
				driftGuage += (driftMultiplier * (3.14f - driftAmount) * (LinearVelocity.Length() * 0.01f));
		}

		//EmitSignal(SignalName.Log, (3.14f - driftAmount) + ", \n" + (minimumDriftAngle) + ", \n" + LinearVelocity.Length()+ ", \n" + whistleCoolLeft);

		//Decide whether to apply the boost
		if(nitrousBoost){

			if((nitrousTimeLeft < 0)){
				//If boost cooldown active, Disable the nitro, and set the cooldown timer for when to next allow nitro
				nitrousBoost = false;
				nitrousCooldownTimeLeft = nitrousCooldownSec;
				flames.Emitting = false;
			}

			nitrousTimeLeft -= (float)delta;
			ApplyImpulse(-forwardDir * nitrousImpulseSpeed, Vector2.Zero);
		}

		
	}





	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		EmitSignal(SignalName.BoostUI, driftGuage);
		EmitSignal(SignalName.WhistleUI, whistleCoolLeft);

		EmitSignal(SignalName.UI, GlobalPosition);

		EmitSignal(SignalName.HealthUI, healthPoints);
	}





	void ZombieCollisionDamage(Node obj)
	{
		if(obj is Zombie zombie)
		{
			if(zombie.attackState == AttackState.passive || zombie.attackState == AttackState.charging)
				healthPoints -= 1;
			else if (zombie.attackState == AttackState.frozen)
			{
				if((zombie.prevVelocity - LinearVelocity).Length() > zombieKillSpeed)
				{
					zombie.scheduleForDeathWaitRebirthIDontKnowEitherWayTheZombieGetsDestroyed = true;
					zombie.blood.Emitting = true;
				}
					
			}
				
		}else if(obj.IsInGroup("Health"))
		{
			healthPoints += 10;
			if(healthPoints > maxHealthPoints) healthPoints = maxHealthPoints;
			obj.QueueFree();
		}


	}
}
