using Diplomski;
using Godot;
using System;

public partial class Pawn : CharacterBody3D
{
    private Vector3 TargetPosition;
    private bool selected;
    private Area3D damageShape;
    private Random random = new Random();
    private Pawn DamageTarget;
    private DamageIndicator damageIndicator;

    [Signal]
    public delegate void InCombatEventHandler();
    [Signal]
    public delegate void DeathEventHandler();

    public NavigationAgent3D Agent { get; set; }
    public Timer damageTimer { get; set; }

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
    public int Hp { get; private set; } = 100;

    public override void _Ready()
    {
        damageIndicator = GetNode<DamageIndicator>("DamageIndicator");

        Agent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        damageShape = GetNode<Area3D>("DamageBoxArea");
        damageShape.BodyEntered += DamageShape_BodyEntered;

        damageTimer = GetNode<Timer>("DamageTimer");
        //agent.VelocityComputed += Agent_VelocityComputed;
        string GroupName = Player != 0 ? $"Player{Player}" : "Monster";
        this.AddToGroup(GroupName);

        var debugMesh = GetNode<MeshInstance3D>("DebugMesh");
        var material = new StandardMaterial3D();
        material.AlbedoColor = PlayerColor;
        debugMesh.SetSurfaceOverrideMaterial(0, material);
    }

    private void DamageShape_BodyEntered(Node3D body)
    {
        if (body == null) return;
        if (body is Pawn pawn)
        {
            if (pawn.Player != Player)
            {
                EmitSignal(SignalName.InCombat);
                this.DamageTarget = pawn;
                this.damageTimer.Timeout += DealDamage;
                this.damageTimer.Start(random.NextDouble());
                this.Agent.TargetPosition = this.GlobalPosition;
            }
        }
    }

    private void DealDamage()
    {
        GD.Print("Damage!");
        damageIndicator.ShowIndicator(0.2f);
        DamageTarget.Death += TurnFinished;
        DamageTarget.Hp -= 7 + random.Next(3);
        if(DamageTarget.Hp <= 0)
        {
            DamageTarget.Die(this);
        }
    }

    private void Die(Pawn pawn)
    {
        GD.Print("Pawn died");
        EmitSignal(SignalName.Death);
        TurnFinished();
        this.Visible = false;
        this.Player = -1;
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

    public override void _PhysicsProcess(double delta)
    {

        var currentLocation = this.GlobalTransform.Origin;
        var nextLocation = Agent.GetNextPathPosition();
        var newVelocity = (nextLocation - currentLocation).Normalized() * Speed;

        this.Velocity = newVelocity;
        this.MoveAndSlide();
    }

    public void SetGoToLocation(Vector3 location)
    {
        Agent.TargetPosition = location;
    }

    public void TurnFinished()
    {
        this.Agent.TargetPosition = this.GlobalPosition;
        this.damageTimer.Stop();
        if (DamageTarget != null)
        {
            DamageTarget.Death -= TurnFinished;
        }
        this.DamageTarget = null;
    }
}
