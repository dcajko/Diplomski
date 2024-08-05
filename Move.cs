using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomski
{
    public class Move
    {
        public int Player {  get; set; }
        public Vector3 StartLocation {  get; set; }
        public FormationBox FormationBox { get; set; }

        public Move(int player, Vector3 startLocation, FormationBox formationBox)
        {
            Player = player;
            StartLocation = startLocation;
            FormationBox = formationBox;
        }

        public void Execute()
        {
            this.FormationBox.MoveUnits();
        }

        public void Dispose()
        {
            this.FormationBox.QueueFree();
        }
    }
}
