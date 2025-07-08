using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Tile
{
    public partial class RoomTile(Room room, Vector2I position, int maxDurability, int sealingIntegrityThreshold, float sealingIntegrityPenalty)
        : TileBase(room, position, true, maxDurability, maxDurability) // 假设 RoomTile 总是具有耐久度，且初始耐久度为最大耐久度
        , ITileFactory<RoomTile>
    {

        [Signal]
        public delegate void TileSealingIntegrityChangedEventHandler(TileBase tile, bool isBeingDamaged);
        public new bool HasSealingIntegrity { get; } = true;

        // 密封完整性阈值
        public int SealingIntegrityThreshold { get; protected set; } = sealingIntegrityThreshold;
        // 阈值惩罚
        public float SealingIntegrityPenalty { get; protected set; } = sealingIntegrityPenalty;

        public override int CurrentDurability
        {
            get => base.CurrentDurability;
            set
            {
                int oldDurability = CurrentDurability;
                bool oldIntegrityBreached = HasSealingIntegrity && (oldDurability < SealingIntegrityThreshold);

                base.CurrentDurability = value; // 调用基类的 setter

                if (HasSealingIntegrity)
                {
                    bool newIntegrityBreached = CurrentDurability < SealingIntegrityThreshold;
                    if (oldIntegrityBreached != newIntegrityBreached)
                    {
                        EmitSignal(nameof(TileSealingIntegrityChangedEventHandler), this, newIntegrityBreached);
                    }
                }
            }
        }

        public static RoomTile CreateFromTileData(Room room, Vector2I position, TileData tileData)
        {
            int maxDurability = tileData.GetCustomData("max_durability").AsInt32();
            int sealingIntegrityThreshold = tileData.GetCustomData("sealing_integrity_threshold").AsInt32();
            float sealingIntegrityPenalty = tileData.GetCustomData("sealing_integrity_penalty").AsSingle();

            return new RoomTile(room, position, maxDurability, sealingIntegrityThreshold, sealingIntegrityPenalty);
        }
    }
}
