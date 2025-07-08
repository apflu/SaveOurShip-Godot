using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character.Spell
{
    public abstract partial class Spell : Resource, ISpell
    {
        public abstract void Cast(SpellCastContext context);
    }
}
