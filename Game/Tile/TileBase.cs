using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Tile
{
    public abstract partial class TileBase(Room room, Vector2I position) : Resource, ITile
    {
        [Signal]
        public delegate void TileDamagedEventHandler(TileBase tile, int amount);
        [Signal]
        public delegate void TileRepairedEventHandler(TileBase tile, int amount);

        public Room Room { get; protected set; } = room;
        public Vector2I Position { get; set; } = position;

        public bool HasDurability { get; protected set; }
        public int Durability { get; set; }
        public int MaxDurability { get; protected set; }
        public bool HasSealingIntegrity { get; protected set; } = false;

        private int _currentDurability;


        protected TileBase(Room room, Vector2I position, bool has_Durability, int durability, int max_Durability) : this(room, position)
        {
            HasDurability = has_Durability;
            Durability = durability;
            MaxDurability = max_Durability;
        }

        public virtual int CurrentDurability
        {
            get => _currentDurability;
            set
            {
                // ... (耐久度更新和边界限制逻辑) ...
                int newDurability = Mathf.Clamp(value, 0, MaxDurability);

                // 如果耐久度没有变化，直接返回
                if (_currentDurability == newDurability) return;

                // 如果耐久度提高，则触发 TileRepairedEventHandler 信号；反之，则触发 TileDamagedEventHandler 信号
                if (newDurability < _currentDurability)
                {
                    EmitSignal(nameof(TileDamagedEventHandler), this, _currentDurability - newDurability);
                }
                else if (newDurability > _currentDurability)
                {
                    EmitSignal(nameof(TileRepairedEventHandler), this, newDurability - _currentDurability);
                }

                // 设置_currentDurability
                _currentDurability = newDurability;
            }
        }

    }
}
