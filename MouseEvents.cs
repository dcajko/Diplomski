using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomski
{
    [Flags]
    public enum MouseEvents
    {
        None = 0,
        LeftDown = 1 << 0,
        LeftUp = 1 << 1,
        LeftDragStart = 1 << 2,
        LeftDragEnd = 1 << 3,
        RightDown = 1 << 4,
        RightUp = 1 << 5,
        RightDragStart = 1 << 6,
        RightDragEnd = 1 << 7,
    }
}
