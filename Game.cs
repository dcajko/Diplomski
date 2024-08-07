using Diplomski;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Game : Node3D
{
    private int movesToExecute;
    private Timer MoveExecutionTimer;

    [Signal]
    public delegate void TurnChangeEventHandler(int player);
    [Signal]
    public delegate void ExecutionStartedEventHandler();
    [Signal]
    public delegate void ExecutionEndedEventHandler();

    [Export]
    public int NumberOfPlayers { get; set; } = 2;
    [Export]
    public double TurnExectutionDuration { get; set; } = 2;

    public int PlayerTurn { get; set; } = 1;
    public int PlayerGold { get; set; } = 0;
    public List<Move> Moves { get; set; } = new List<Move>();

    private View ViewNode { get; set; }

    public void SpawnPlayerPawn()
    {
        GetTree().CallGroup("PlayerSpawner", "Spawn");
    }

    public void SpawnEnemyPawn()
    {
        GetTree().CallGroup("EnemySpawner", "Spawn");
    }

    public override void _Ready()
    {
        ViewNode = GetNode<View>("CameraRoot/Camera");
        MoveExecutionTimer = new Timer();
        MoveExecutionTimer.Timeout += TurnTimeout;
        Helper.AddNode(this, MoveExecutionTimer);
        this.SetupTurn();
    }

    public void NextTurn()
    {
        PlayerTurn++;
        if (PlayerTurn > NumberOfPlayers)
        {
            PlayerTurn = 0;
            PlayMoves();
        }
        SetupTurn();
    }

    public void SetupTurn()
    {
        EmitSignal(SignalName.TurnChange, PlayerTurn);
        ViewNode.SetupPlayerView(PlayerTurn);
    }

    public void PlayMoves()
    {
        EmitSignal(SignalName.ExecutionStarted, PlayerTurn);
        movesToExecute = Moves.Count;
        if (movesToExecute > 0)
        {
            this.MoveExecutionTimer.Start(TurnExectutionDuration);
        }
        foreach (var move in Moves)
        {
            move.FormationBox.AllPawnsAtLocation += MoveDone;
            move.Execute();
        }
    }

    public void MoveDone()
    {
        movesToExecute--;
        if (movesToExecute <= 0)
        {
            ExecutionFinished();
        }
    }

    public void TurnTimeout()
    {
        GD.Print("TurnTimeout");
        foreach (var move in Moves)
        {
            move.FormationBox.StopUnits();
        }
        ExecutionFinished();
    }

    public void ExecutionFinished()
    {
        GD.Print("AllMovesFinished");
        this.MoveExecutionTimer.Stop();
        foreach (var item in Moves)
        {
            item.Dispose();
        }
        Moves.Clear();
        GetTree().GetNodesInGroup("Selectable").Cast<Pawn>().Where(x => x.Player == -1).ToList().ForEach(x => x.QueueFree());
        EmitSignal(SignalName.ExecutionEnded, PlayerTurn);
        NextTurn();
    }
}
