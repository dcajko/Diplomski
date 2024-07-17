using Godot;
using System;

public partial class Pawn : CharacterBody3D
{
    private NavigationAgent3D agent;
    private Vector3 ClickPosition;
    private Vector3 TarketPosition;

    [Export]
    public float Speed { get; set; } = 25;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        agent.VelocityComputed += Agent_VelocityComputed;
    }

    private void Agent_VelocityComputed(Vector3 safeVelocity)
    {
        this.Velocity = Velocity.MoveToward(safeVelocity, 0.05f);
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

        this.Velocity = newVelocity;
        this.MoveAndSlide();
    }

    public void SetGoToLocation(Vector3 location)
    {
        agent.TargetPosition = location;
    }
}
