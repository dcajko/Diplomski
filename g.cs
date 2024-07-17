using Godot;
using System;

public partial class g : CharacterBody3D
{
	private NavigationAgent3D agent;
	public float Speed { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        agent.VelocityComputed += Agent_VelocityComputed;

        agent.TargetPosition = new Vector3(100, 100, 100);
    }

    private void Agent_VelocityComputed(Vector3 safeVelocity)
    {
        this.Velocity = Velocity.MoveToward(safeVelocity, 0.25f);
        this.MoveAndSlide();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
		var currentLocation = this.GlobalTransform.Origin;
		var nextLocation = agent.GetNextPathPosition();
		var newVelocity = (nextLocation - currentLocation).Normalized() * Speed;

		agent.Velocity = newVelocity;
    }

    public void SetGoToLocation(Vector3 location) 
    {
        agent.TargetPosition = location;
    }
}
