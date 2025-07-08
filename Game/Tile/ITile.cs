using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Tile
{
    public interface ITile
    {
        Room Room { get; } // tile 所在的房间
        Vector2I Position { get; set; } // tile 在房间中的位置
        bool HasDurability { get; } // tile 是否有耐久度；如果为false，则无法被损坏或修复
        int Durability { get; set; } // 耐久度
        int MaxDurability { get; } // 最大耐久度
        bool HasSealingIntegrity { get; } // tile 是否有密封完整性
    }
}
