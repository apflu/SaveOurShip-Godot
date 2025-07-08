using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Character.Spell
{
    public interface ISpell
    {
        void Cast(SpellCastContext context);
    }
}
