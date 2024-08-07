using Godot;
using System;

public partial class Interface : Control
{
    private CanvasItem gameControls;
    private CanvasItem SpawnPlayerPawn;
    private CanvasItem SpawnEnemyPawn;

    [Export]
    public Game Game { get; set; }

    public override void _Ready()
    {
        
    }

    public void HideGameControls()
    {

    }
}
