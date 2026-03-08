using Godot;
using System;
using System.Threading.Tasks;
using Godot.Collections;

public partial class SpawnPoint : Node2D
{
    private PackedScene zombiePrefab;
    private Player player;
	public float spawnCooldownTimeSeconds;// = 5;
	Timer timer;

	Zombie	instantiatedZombie;
	[Export]
    private float minSpawnDistance = 100f;
	public bool canSpawn = true;
	private SpawnSystem spawnSystem;

    public PackedScene ZombiePrefab { get => zombiePrefab; set => zombiePrefab = value; }

    // Called when the node enters the scene tree for the first time.

    public override void _Ready()
	{
		//ZombiePrefab = GD.Load<PackedScene>("res://Blueprints/zombie.tscn");
		player = GetNode("/root/Scene/PlayerCharacter") as Player;	
		spawnSystem = GetParent() as SpawnSystem;

		timer = new Timer();
		//timer.WaitTime = spawnSystem.CurrentSpawnCooldownSeconds;
		timer.Timeout += SpawnZombie;
		timer.Autostart = true;

		AddChild(timer); 
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer.WaitTime = spawnSystem.CurrentSpawnCooldownSeconds;
	}

	public void SpawnZombie()
	{
		float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);
		canSpawn = distanceToPlayer >= minSpawnDistance;
		if(canSpawn)
		{
			instantiatedZombie = ZombiePrefab.Instantiate() as Zombie;
			instantiatedZombie.GlobalPosition = GlobalPosition;
			GetTree().CurrentScene.AddChild(instantiatedZombie);
		}
	}
}
