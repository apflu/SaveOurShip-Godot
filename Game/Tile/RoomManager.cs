using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SaveOurShip.Game.Tile
{
    public delegate TileBase TileFactoryDelegate(Room room, Vector2I position, TileData tileData);
    public partial class RoomManager : Node
    {
        [Export]
        public TileMapLayer Terrain_TileMapLayer { get; set; } = null!;

        private Dictionary<Vector2I, TileBase> _tileLogicInstances = []; // 值是 TileBase (非泛型基类)

        // 映射瓦片类型标识符到工厂委托
        private Dictionary<string, TileFactoryDelegate> _tileFactoryMap = [];

        private void OnRoomTileSealingIntegrityChanged(TileBase tile, bool isBeingDamaged)
        {
            throw new NotImplementedException();
        }

        private void OnTileRepaired(TileBase tile, int amount)
        {
            throw new NotImplementedException();
        }

        private void OnTileDamaged(TileBase tile, int amount)
        {
            throw new NotImplementedException();
        }

        public Variant GetTileData(Vector2I position, string dataLayerName = "")
        {
            // 获取指定位置的瓦片数据
            TileData data = Terrain_TileMapLayer.GetCellTileData(position);

            if (data == null)
            {
                GD.PrintErr($"No tile data found at position: {position}");
                return default; // 返回一个默认的空 Variant
            }

            // 如果 dataLayerName 为空，它会尝试获取默认的自定义数据。
            // 否则，它会获取指定名称的自定义数据。
            Variant value = data.GetCustomData(dataLayerName);
            return value;
        }

        public int GetTileMaxDurability(Vector2I position)
        {
            Variant value = GetTileData(position);
            if (value.VariantType == Variant.Type.Int)
            {
                return value.AsInt32();
            } else
            {
                GD.PrintErr($"Tile at position {position} does not have a valid max durability value.");
                return 0;
            }
        }

        public override void _Ready()
        {
            if (Terrain_TileMapLayer == null)
            {
                GD.PrintErr("RoomManager: Terrain_TileMapLayer is not assigned!");
                return;
            }

            RegisterTileFactories();
            InitializeAllTileLogics();
        }

        private void RegisterTileFactories()
        {
            // 注册每个瓦片类型的静态工厂方法
            _tileFactoryMap.Add("room", RoomTile.CreateFromTileData);
            _tileFactoryMap.Add("void", VacuumTile.CreateFromTileData);
        }

        private void InitializeAllTileLogics()
        {
            Room currentRoom = new Room(); // TODO

            foreach (var coords in Terrain_TileMapLayer.GetUsedCells())
            {
                TileData tileData = Terrain_TileMapLayer.GetCellTileData(coords);
                if (tileData == null)
                {
                    GD.PrintErr($"RoomManager: No TileData found for cell {coords}. Skipping.");
                    continue;
                }

                Variant tileTypeVariant = tileData.GetCustomData("type");
                if (tileTypeVariant.VariantType == Variant.Type.Nil || tileTypeVariant.VariantType != Variant.Type.String)
                {
                    GD.PrintErr($"RoomManager: Tile at {coords} does not have a valid 'tile_logic_type' custom data (expected string). Skipping.");
                    continue;
                }

                string tileTypeIdentifier = tileTypeVariant.AsString();

                if (!_tileFactoryMap.TryGetValue(tileTypeIdentifier, out TileFactoryDelegate factoryMethod))
                {
                    // 临时suppress错误
                    //GD.PrintErr($"RoomManager: No factory registered for tile type '{tileTypeIdentifier}' at {coords}. Skipping.");
                    continue;
                }

                // 直接调用工厂委托来创建瓦片逻辑实例
                TileBase pendingTile = factoryMethod.Invoke(currentRoom, coords, tileData);

                if (pendingTile == null)
                {
                    GD.PrintErr($"RoomManager: Factory method for '{tileTypeIdentifier}' returned null at {coords}.");
                    continue;
                }

                // 订阅 TileBase 的通用信号
                pendingTile.TileDamaged += OnTileDamaged;
                pendingTile.TileRepaired += OnTileRepaired;

                // 订阅特定派生类信号
                if (pendingTile is RoomTile roomTile)
                {
                    roomTile.TileSealingIntegrityChanged += OnRoomTileSealingIntegrityChanged;
                }

                _tileLogicInstances[coords] = pendingTile;
            }

            GD.Print($"RoomManager: Initialized {_tileLogicInstances.Count} tile logic instances.");
        }
    }
}
