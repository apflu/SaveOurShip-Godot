using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Tile
{
    public partial class VacuumTile(Room room, Vector2I position)
        : TileBase(room, position, false, 0, 0), ITileFactory<VacuumTile>
    {
        public static VacuumTile CreateFromTileData(Room room, Vector2I position, TileData tileData)
        {
            return new VacuumTile(room, position);
        }

        public override int CurrentDurability
        {
            get => 0;
            set { GD.PrintErr("Attempted to set durability of a VoidTile."); }
        }
    }
}
