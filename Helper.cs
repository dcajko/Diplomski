using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomski
{
    public static class Helper
    {
        public static bool AddNode(Node parent, Node child, string name)
        {
            parent.AddChild(child);
            child.Name = name;
            child.Owner = parent;
            return true;
        }
    }
}
