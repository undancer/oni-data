# oni-data 缺氧游戏的Data文件夹

##

```
Data
├── StreamingAssets
│   ├── codex                                                                     百科全书
│   │   ├── Buildings
│   │   ├── Creatures
│   │   ├── Emails
│   │   ├── Geysers
│   │   ├── Investigations
│   │   ├── Journals
│   │   ├── MyLog
│   │   ├── Notices
│   │   ├── Plants
│   │   ├── ResearchNotes
│   │   ├── Systems
│   │   └── HomePage.yaml
│   ├── elements                                                                  # 元素
│   │   ├── gas.yaml
│   │   ├── liquid.yaml
│   │   ├── solid.yaml
│   │   └── special.yaml
│   ├── strings                                                                   # 语言包
│   ├── templates                                                                 # 模版
│   │   ├── bases                                                                   # 基地
│   │   ├── features                                                                # 特性
│   │   ├── interiors                                                               # 内饰，这里指火箭内部
│   │   ├── poi                                                                     # 遗迹
│   │   ├── rough\ unused\ poi                                                      # 未使用的遗迹
│   │   ├── chunk.yaml
│   │   ├── test_disease_temperature.yaml
│   │   ├── test_disease_temperature_gas.yaml
│   │   ├── test_food_cooking.yaml
│   │   ├── test_metalrefinery.yaml
│   │   ├── test_nuclear.yaml
│   │   ├── test_pwater_sublimation.yaml
│   │   ├── test_rocketbase.yaml
│   │   ├── test_suit_checkpoint.yaml
│   │   └── warpPortal.yaml
│   ├── worldgen                                                                  # 地图生成器
│   │   ├── biomes                                                                  # 群落
│   │   ├── clusters                                                                # 星群
│   │   ├── features                                                                # 特性
│   │   ├── noise                                                                   # 噪点
│   │   ├── subworlds                                                               # 子世界
│   │   ├── traits                                                                  # 特质
│   │   ├── worlds                                                                  # 世界
│   │   ├── borders.yaml                                                            ## SettingsCache
│   │   ├── defaults.yaml                                                           ## SettingsCache
│   │   ├── layers.yaml                                                             ## SettingsCache
│   │   ├── mobs.yaml                                                               ## SettingsCache
│   │   ├── rivers.yaml                                                             ## SettingsCache
│   │   ├── rooms.yaml                                                              ## SettingsCache
│   │   ├── seed_names.txt                                                          # 暂时没啥用
│   │   └── temperatures.yaml                                                       ## SettingsCache
│   └── Tuning.json                                                               # TuningSystem
├── README.md
├── app.info
└── boot.config
```

## WorldGen

新的策略是
clusters -> worlds -> subworlds -> biomes

WorldGenProgressStages.Stages
| k | v |
| --- | :---: |
| Failure | - |
| SetupNoise | WorldGen.GenerateNoiseData |
| GenerateNoise | WorldGen.GenerateUnChunkedNoise |
| GenerateSolarSystem | ??? |
| WorldLayout | WorldGen.GenerateLayout |
| CompleteLayout | WorldGen.CompleteLayout |
| NoiseMapBuilder | WorldGen.WriteOverWorldNoise |
| ClearingLevel | WorldGen.RenderToMap |
| Processing | WorldGen.ProcessByTerrainCell |
| Borders | WorldGen.ProcessByTerrainCell |
| ProcessRivers | ??? |
| ConvertCellsToEdges | ??? |
| DrawWorldBorder | WorldGen.DrawWorldBorder |
| PlaceTemplates | TemplateSpawning.DetermineTemplatesForWorld |
| SettleSim | WorldGenSimUtil.DoSettleSim |
| DetectNaturalCavities | MobSpawning.DetectNaturalCavities |
| PlacingCreatures | WorldGen.SpawnMobsAndTemplates |
| Complete | WorldGen.RenderOffline |
| NumberOfStages | ??? |


```cs
  public enum SceneLayer
  {
    WorldSelection = -3, // 0xFFFFFFFD
    NoLayer = -2, // 0xFFFFFFFE
    Background = -1, // 0xFFFFFFFF
    Backwall = 1,
    Gas = 2,
    GasConduits = 3,
    GasConduitBridges = 4,
    LiquidConduits = 5,
    LiquidConduitBridges = 6,
    SolidConduits = 7,
    SolidConduitContents = 8,
    SolidConduitBridges = 9,
    Wires = 10, // 0x0000000A
    WireBridges = 11, // 0x0000000B
    WireBridgesFront = 12, // 0x0000000C
    LogicWires = 13, // 0x0000000D
    LogicGates = 14, // 0x0000000E
    LogicGatesFront = 15, // 0x0000000F
    InteriorWall = 16, // 0x00000010
    GasFront = 17, // 0x00000011
    BuildingBack = 18, // 0x00000012
    Building = 19, // 0x00000013
    BuildingUse = 20, // 0x00000014
    BuildingFront = 21, // 0x00000015
    TransferArm = 22, // 0x00000016
    Ore = 23, // 0x00000017
    Creatures = 24, // 0x00000018
    Move = 25, // 0x00000019
    Front = 26, // 0x0000001A
    GlassTile = 27, // 0x0000001B
    Liquid = 28, // 0x0000001C
    Ground = 29, // 0x0000001D
    TileMain = 30, // 0x0000001E
    TileFront = 31, // 0x0000001F
    FXFront = 32, // 0x00000020
    FXFront2 = 33, // 0x00000021
    SceneMAX = 34, // 0x00000022
  }
```
