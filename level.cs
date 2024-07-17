using Godot;
using System;

public partial class level : Node3D
{
    private Pawn pawn;

    public override void _Ready()
    {
        pawn = GetNode<Pawn>("Pawn");
        var view = GetNode<View>("View");
        view.Pawn = pawn;
    }

    private void Level_MouseClick(Vector3 location)
    {
        pawn.SetGoToLocation(location);
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventKey key)
        {
            if(key.PhysicalKeycode == Key.M)
            {
                pawn.SetGoToLocation(new Vector3(100,100, 100));
            }
        }
    }
}
