using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Tile
{
    public interface ITileFactory<T> where T : TileBase
    {
        // 静态抽象方法，要求所有实现此接口的类提供一个CreateFromTileData方法
        // 该方法接收 Room, Position, TileData 并返回 T 类型的 TileBase
        static abstract T CreateFromTileData(Room room, Vector2I position, TileData tileData);
    }
}
