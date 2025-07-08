using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character
{
    public abstract partial class MonsterBase : RigidBody2D
    {
        [Export]
        public float Speed = 100f;
        [Export(PropertyHint.Range, "0.0,1.0")]
        public float Push_Resistance = 0.5f; // 推力抵抗力，值越大，抵抗力越强
        [Export]
        public float Force_Magnitude = 500f; // 施加的力的大小
    }
}
