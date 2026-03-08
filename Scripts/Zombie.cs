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
    
	[Export]
	private float leapDistance = 200f;
	[Export]
	private float leapForce = 150f;
	[Export]
	private float leapAttackWait = 5f;

	private float attackEffectiveTime = 0.5f;

	Timer timerAttack, timerAttackEffective;

	bool attackEffective = true;



    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if(player == null)
		{
			player = GetNode("/root/Scene/PlayerCharacter") as RigidBody2D;			
		}
		timerAttack = new Timer();
		timerAttack.WaitTime = leapAttackWait + GD.RandRange(-0.5f,2f);
		timerAttack.Timeout += LeapAttack;
		timerAttack.Autostart = true;

		timerAttackEffective = new Timer();
		timerAttackEffective.Timeout += SetAttackIneffective;
		timerAttackEffective.WaitTime = attackEffectiveTime;
		timerAttackEffective.Autostart = false;
		timerAttackEffective.OneShot = true;
		
		AddChild(timerAttack); 
		AddChild(timerAttackEffective);

		BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if ((player != null) && (frozen == false))
		{
			Vector2 target = player.GlobalPosition - GlobalPosition;
			Vector2 direction = target.Normalized();
			Vector2 forward = -Transform.Y;

			float angle = forward.AngleTo(direction);

			//This little commented snippet below is gonne be left here as a testament to mine (Fenarn's) humongous stupidity.
			//float angleRotated = Mathf.Wrap(angle + Mathf.Pi / 2f, -Mathf.Pi, Mathf.Pi);
			
			ApplyTorqueImpulse(angle*torqueStrength - AngularVelocity * damping);
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

	public void LeapAttack()
	{
		Vector2 target = player.GlobalPosition - GlobalPosition;
		Vector2 direction = target.Normalized();
		Vector2 forward = -Transform.Y;

		float angle = forward.AngleTo(direction);

		float playerDistance = GlobalPosition.DistanceTo(player.GlobalPosition);
		if(playerDistance <= leapDistance && !frozen)
		{
			ApplyImpulse(direction * leapForce);
			timerAttack.WaitTime = leapAttackWait + GD.RandRange(-0.5f,2f);
			//GD.Print("LEAP ATTACK! " + timerAttack.WaitTime);
			attackEffective = true;
			timerAttackEffective.Start();
		}
	}

	private void SetAttackIneffective()
	{
		attackEffective = false;
		//GD.Print("Won't damage anymore");
	}

	private void OnBodyEntered(Node body)
	{
		if(attackEffective && body.Name == "PlayerCharacter")
		{
			//TODO: dealing actual damage to hp
			GD.Print($"Would deal damage to {body.Name}");
		}
		
	}
}
