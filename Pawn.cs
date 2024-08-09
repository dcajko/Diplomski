using Diplomski;
using Godot;
using System;
using System.Runtime.CompilerServices;

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
        damageShape.BodyEntered += this.DamageShape_BodyEntered;

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
                this.EmitSignal(SignalName.InCombat);
                this.EnterCombat(pawn);
            }
        }
    }

    public void EnterCombat(Pawn pawn)
    {
        StopMovement();

        this.DamageTarget = pawn;
        this.DamageTarget.Death += ExitCombat;

        this.damageTimer.OneShot = false;
        this.damageTimer.Timeout += DealDamage;
        this.damageTimer.Start(random.NextDouble());
    }

    private void StopMovement()
    {
        this.Agent.TargetPosition = this.GlobalPosition;
    }

    public void DealDamage()
    {
        this.DamageTarget.GetDamaged(50);
    }

    public void ExitCombat()
    {
        this.damageTimer.Stop();
        this.damageTimer.Timeout -= DealDamage;
        this.DamageTarget = null;
    }

    private void GetDamaged(int damage)
    {
        GD.Print("Damage!");
        this.Hp -= damage;
        damageIndicator.ShowIndicator(0.2f);
        if (this.Hp <= 0)
        {
            this.Die();
        }
    }

    private void Die()
    {
        GD.Print("Pawn died");
        EmitSignal(SignalName.Death);
        TurnFinished();
        this.Visible = false;
        this.Player = -1;
        this.CollisionLayer = 0;
        this.GlobalPosition = new Vector3(0, 1000, 0);
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
        this.Selected = false;
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
        StopMovement();
    }
}
