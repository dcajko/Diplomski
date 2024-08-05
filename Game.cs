using Diplomski;
using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node3D
{
    [Export]
    public int NumberOfPlayers { get; set; } = 2;

    public int PlayerTurn { get; set; } = 1;

    public List<Move> Moves { get; set; } = new List<Move>();

    public void NextTurn()
    {
        PlayerTurn++;
        if (PlayerTurn > NumberOfPlayers)
        {
            PlayerTurn = 1;
            PlayMoves();
        }
        SetupTurn();
    }

    public void SetupTurn()
    {

    }

    public void PlayMoves()
    {

    }
}
