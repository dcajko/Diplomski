using Godot;
using System;

public partial class View : Camera3D
{
    public Pawn Pawn{get; set;}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
            {
                Pawn.SetGoToLocation(ShootRay());
            }
        }
    }

    public Vector3 ShootRay()
    {
        var mousePosition = this.GetViewport().GetMousePosition();
        int rayLength = 1000;
        var from = ProjectRayOrigin(mousePosition);
        var to = from + ProjectRayNormal(mousePosition)*rayLength;
        var space = GetWorld3D().DirectSpaceState;
        var rayQuery = new PhysicsRayQueryParameters3D();
        rayQuery.From = from;
        rayQuery.To = to;
        var raycastResult = space.IntersectRay(rayQuery);
        return (Vector3)raycastResult["position"];
    }
}
