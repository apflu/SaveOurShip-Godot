using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Tile
{
    /// <summary>
    /// room 是一系列相连的 tile 的集合，由封闭的墙壁与地面组成。
    /// </summary>
    public partial class Room : Node
    {
        public int RoomId { get; set; }
        public List<Vector2I> TilesInRoom { get; set; } = new List<Vector2I>();
        public List<Vector2I> WallTiles { get; set; } = new List<Vector2I>();
        public bool IsPressurized { get; private set; } = true;


        // 房间是否暴露在太空中失压
        public bool IsExposed() {
            return false; 
        }


    }
}
