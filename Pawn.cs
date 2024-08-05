using Diplomski;
using Godot;
using System;

public partial class Pawn : CharacterBody3D
{
    private NavigationAgent3D agent;
    private Vector3 TargetPosition;
    private bool selected;



    [Export]
    public Node3D SelectionNode { get; set; }

    [Export]
    public float Speed { get; set; } = 25;
    public bool Selected
    {
        get => selected; set
        {
            if (selected != value) 
            { 
                Select(value); 
            }
            selected = value;

        }
    }
    [Export]
    public VisibilityLevel visibility { get; set; } = VisibilityLevel.None;

    [Export]
    public int Player { get; set; } = 0;
    [Export]
    public Color PlayerColor { get; set; } = new Color(1, 1, 1, 1);

    public override void _Ready()
    {
        agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        //agent.VelocityComputed += Agent_VelocityComputed;
        string GroupName = Player != 0 ? $"Player{Player}" : "Monster";
        this.AddToGroup(GroupName);

        var debugMesh = GetNode<MeshInstance3D>("DebugMesh");
        var material = new StandardMaterial3D();
        material.AlbedoColor = PlayerColor;
        debugMesh.SetSurfaceOverrideMaterial(0, material);


    }

    private void Select(bool selected)
    {
        if (SelectionNode != null)
        {
            SelectionNode.Visible = selected;
        }
        //GD.Print(selected ? "Selected" : "Unselected");
    }

    private void Agent_VelocityComputed(Vector3 safeVelocity)
    {
        this.Velocity = Velocity.MoveToward(safeVelocity, 0.05f);
        this.MoveAndSlide();
    }

    public void Unselect()
    {
        GD.Print("Unselected");
        Selected = false;
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
