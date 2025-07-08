using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character.Spell
{
    public partial class SpellCastContext
    {
        public GameCharacter Caster { get; private set; }
        public Node Target { get; private set; }
        public List<Node> Targets { get; private set; } = new List<Node>();
        public Vector2 CastPosition { get; private set; }
        public Vector2 CastDirection { get; private set; }

        public SpellCastContext(GameCharacter caster)
        {
            Caster = caster;
        }
    }
}
