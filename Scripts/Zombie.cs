using Godot;
using System;


public enum AttackState{passive, charging, attacking, frozen};

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


	[Export] public float frozenKillTimer = 3f;
    
	[Export]
	private float leapDistance = 200f;
	[Export]
	private float leapForce = 150f;
	[Export]
	private float leapAttackWait = 5f;
	[Export]
	private Texture2D zombieMat;
	[Export]
	private Texture2D zombieChargeMat;
	[Export]
	private Texture2D zombieLeapMat;

	[Export]
	private float attackChargeTime = 1f;

	private float attackEffectiveTime = 0.5f;

	Timer timerAttack, timerAttackEffective, timerAttackChargeTime;

	Vector2 attackDir;

	public AttackState attackState;



	[Export]
	public int attackDamage = 5;




	float CalcTimeTillNextAttack()
	{
		return (float)leapAttackWait + (float)GD.RandRange(-0.5f,2f);
	}




    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		if(player == null)
		{
			player = GetNode("/root/Scene/PlayerCharacter") as RigidBody2D;			
		}


		//Time until testing for LeapAttack
		timerAttack = new Timer();
		timerAttack.Timeout += LeapAttackTest;
		timerAttack.OneShot = true; 
		AddChild(timerAttack); 
		timerAttack.Start(CalcTimeTillNextAttack());


		//Time between testing, and attacking
		timerAttackChargeTime = new Timer();
		timerAttackChargeTime.Timeout += LeapAttack;
		timerAttackChargeTime.OneShot = true; 
		AddChild(timerAttackChargeTime);


		//Amount of time the damage window is open for
		timerAttackEffective = new Timer();
		timerAttackEffective.Timeout += SetAttackIneffective;
		timerAttackEffective.WaitTime = attackEffectiveTime;
		timerAttackEffective.OneShot = true; 
		AddChild(timerAttackEffective);

		
		

		BodyEntered += OnBodyEntered;
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if ((player != null) && (attackState == AttackState.passive))
		{
			Vector2 target = player.GlobalPosition - GlobalPosition;
			Vector2 direction = target.Normalized();
			Vector2 forward = -Transform.Y;

			float angle = forward.AngleTo(direction);

			//This little commented snippet below is gonne be left here as a testament to mine (Fenarn's) humongous stupidity.
			//float angleRotated = Mathf.Wrap(angle + Mathf.Pi / 2f, -Mathf.Pi, Mathf.Pi);

			//F   -aljowen

			ApplyTorqueImpulse(angle*torqueStrength - AngularVelocity * damping);
			ApplyForce(direction * thrust);
			
			
			
		}
		else if(attackState == AttackState.frozen)
		{
			//If the zombie is frozen start killing them
			frozenKillTimer -= (float)delta;

			if(frozenKillTimer <= 0)
			{
				QueueFree();
			}
		}
	}



	public void LeapAttackTest()
	{
		//Check for correct state
		if(attackState != AttackState.passive) return;


		//Figure out if zombie is close to player
		Vector2 target = player.GlobalPosition - GlobalPosition;
		float playerDistance = GlobalPosition.DistanceTo(player.GlobalPosition);

		//If they are close, start charging the attack
		if(playerDistance <= leapDistance)
		{
			//Set the direction for the attack in stone
			attackDir = target.Normalized();
			Rotation = attackDir.Angle() + ((float)Math.PI / 2);
			LockRotation = true;
			GetNode<Sprite2D>("Sprite2D").Texture = zombieChargeMat;
			attackState = AttackState.charging;
			timerAttackChargeTime.Start(attackChargeTime);
		}
		else
		{
			SetAttackIneffective();
		}
	}


	public void LeapAttack()
	{
		//Check for correct state
		if(attackState != AttackState.charging) return;

		ApplyImpulse(attackDir * leapForce);

		attackState = AttackState.attacking;
		timerAttackEffective.Start();

		GetNode<Sprite2D>("Sprite2D").Texture = zombieLeapMat;		
	}

	private void SetAttackIneffective()
	{
		//Don't do this if frozen
		if(attackState == AttackState.frozen) return;

		LockRotation = false;
		GetNode<Sprite2D>("Sprite2D").Texture = zombieMat;
		timerAttack.Start(CalcTimeTillNextAttack());
		attackState = AttackState.passive;
	}

	private void OnBodyEntered(Node body)
	{
		if(attackState == AttackState.attacking && body.Name == "PlayerCharacter")
		{
			Player playerLocal = body as Player;
			//TODO: dealing actual damage to hp
			if(playerLocal != null && playerLocal.healthPoints > 0)
			{
				playerLocal.healthPoints -= attackDamage;
				GD.Print($"Dealt damage to {body.Name}");	
			}
			
		}
		
	}

}
