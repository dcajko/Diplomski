using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Diplomski
{
    public class Move
    {
        public int Player {  get; set; }
        public Vector3 StartLocation {  get; set; }
        public FormationBox FormationBox { get; set; }
    }
}
