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
| - | - |
| Failure | |
| SetupNoise | |
| GenerateNoise | |
| GenerateSolarSystem | |
| WorldLayout | |
| CompleteLayout | |
| NoiseMapBuilder | |
| ClearingLevel | |
| Processing | |
| Borders | |
| ProcessRivers | |
| ConvertCellsToEdges | |
| DrawWorldBorder | |
| PlaceTemplates | |
| SettleSim | |
| DetectNaturalCavities | |
| PlacingCreatures | |
| Complete | |
| NumberOfStages | |
