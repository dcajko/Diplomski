using Diplomski;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyAi : Node
{
    private Game Game { get; set; }
    private GameEnvironment Environment { get; set; }
    private double DangerDistance { get; set; }

    [Export]
    public int Player { get; set; } = 2;

    [Export]
    public int SpawnScale { get; set; } = 2;

    public override void _Ready()
    {
        Game = (Game)Owner;
        Game.TurnChange += ProcessTurnChangeEvent;
    }

    public void ProcessTurnChangeEvent(int player)
    {
        if (player == Player)
        {
            this.PlayTurn();
        }
    }

    public void PlayTurn()
    {
        QueryEnvironment();
        SpawnUnits();
        AttackPlayer();
        DefendBase();
        Game.NextTurn();
    }

    private void DefendBase()
    {
    }

    private void AttackPlayer()
    {
        this.Environment.OwnUnits.ForEach(x => x.Selected = true);
        if (Environment.OwnUnits.Count > 0 && Environment.EnemyUnits.Count > 0)
        {
            Vector3 from = Environment.OwnUnits.Aggregate(Vector3.Zero, (acc, obj) => acc + obj.GlobalPosition) / Environment.OwnUnits.Count;
            Vector3 to = Environment.EnemyUnits.Aggregate(Vector3.Zero, (acc, obj) => acc + obj.GlobalPosition) / Environment.EnemyUnits.Count;
            NewOrder(to, from);
        }
        else if (Environment.OwnUnits.Count > 0 && Environment.EnemyUnits.Count == 0)
        {
            Vector3 from = Environment.OwnUnits.Aggregate(Vector3.Zero, (acc, obj) => acc + obj.GlobalPosition) / Environment.OwnUnits.Count;
            Random random = new Random();
            int randomIndex = random.Next(0, Environment.EnemyBases.Count);
            Vector3 to = Environment.EnemyBases.ElementAt(randomIndex).GlobalPosition;
            NewOrder(to, from);
        }
    }

    public void NewOrder(Vector3 moveToLocation, Vector3 LookAtLocation)
    {
        var directionTo = LookAtLocation.DirectionTo(moveToLocation);
        var newOrientation = (float)Math.Atan2(directionTo.X, directionTo.Z);
        CreateNewOrder(moveToLocation, newOrientation, Environment.OwnUnits);
    }

    private void CreateNewOrder(Vector3 locationA, float Orientation, List<Pawn> pawns)
    {
        var formationBox = new FormationBox();
        formationBox.MinSize = 2.1f;
        formationBox.Position = locationA;
        formationBox.Rotation = new Vector3(0, Orientation, 0);
        formationBox.Prepare(pawns);
        Helper.AddNode(GetTree().Root, formationBox);

        var points = formationBox.PreparePoints();
        if (points.Count > 0)
        {
            Game.Moves.Add(new Move(Game.PlayerTurn, locationA, formationBox));
        }
    }

    private void SpawnUnits()
    {
        if (Environment.OwnUnits.Count < SpawnScale * Environment.TurnCounter + 1)
        {
            int timesToSpawn = (Environment.TurnCounter + 1) * SpawnScale / Environment.HomeBases.Count;
            for (int i = 0; i < timesToSpawn; i++)
            {
                foreach (var hBase in Environment.HomeBases)
                {
                    hBase.Spawn();
                }
            }
        }
    }

    private void QueryEnvironment()
    {
        this.Environment = Game.GetGameEnvironment(Player);
        Vector3 AverageEnemyLocation = Environment.EnemyUnits.Aggregate(Vector3.Zero, (acc, obj) => acc + obj.GlobalPosition) / Environment.EnemyUnits.Count;
        Vector3 AverageHomeLocation = Environment.HomeBases.Aggregate(Vector3.Zero, (acc, obj) => acc + obj.GlobalPosition) / Environment.HomeBases.Count;
        DangerDistance = AverageEnemyLocation.DistanceTo(AverageHomeLocation);
    }
}
