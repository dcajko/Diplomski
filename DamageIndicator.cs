using Godot;
using System;

public partial class DamageIndicator : Node3D
{
    private Timer timer;
    private MeshInstance3D indicator;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
        indicator = GetNode<MeshInstance3D>("MeshInstance3D");
        indicator.Visible = false;
        timer.Timeout += HideIndicator;
    }

    public override void _Process(double delta)
    {
        indicator.RotationDegrees += new Vector3(0, 50, 0) * (float)delta;
    }

    public void ShowIndicator(double timeout)
    {
        indicator.Visible = true;
        timer.Start(timeout);
    }
    public void HideIndicator()
    {
        this.indicator.Visible = false;
    }
}
