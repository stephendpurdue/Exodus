# Exodus

### Project Overview:

Exodus is a procedurally generated dungeon crawler created in Unity. Each run generates a unique dungeon layout using Binary Space Partitioning (BSP) for room placmement, and random-walk algorithms for corridor generation. The game is available to play on itch.io.

This game was created as the capstone project for my BSc in Game Development.

https://stephendpurdue.itch.io/exodus

---

### Features:

- Procedurally generated dungeons using BSP and Random Walk algorithms.
- Unity based dungeon crawler built with C#, using Unity Version Control.
- High replayability with cross-platform control support.
- Built as a standalone Unity project with clear structure to ensure easy updating or extension by other developers.

### Quick Start:

To run Exodus, either clone the repo or download the .zip file and import to Unity, the game requires Unity version 6000.3.5f2.

### Project Structure:

```text
Assets/
├─ Editor/
├─ Miscellaneous/
│  ├─ Health/
│  └─ TextMesh Pro/
├─ Scripts/
│  ├─ Data/
│  │  └─ SimpleRandomWalkSO.cs
│  ├─ DungeonGeneration/
│  │  ├─ AbstractDungeonGenerator.cs
│  │  ├─ CorridorFirstDungeonGenerator.cs
│  │  ├─ DungeonDecorator.cs
│  │  ├─ ProceduralGenerationAlgorithms.cs
│  │  ├─ RoomFirstDungeonGenerator.cs
│  │  ├─ SimpleRandomWalkDungeonGenerator.cs
│  │  ├─ TilemapVisualizer.cs
│  │  ├─ WallGenerator.cs
│  │  └─ WallTypesHelper.cs
│  ├─ Enemy/
│  │  ├─ EnemyAI.cs
│  │  ├─ EnemyHealth.cs
│  │  └─ EnemySpawner.cs
│  ├─ Environment/
│  │  ├─ CoinCollector.cs
│  │  ├─ DestructibleDecoration.cs
│  │  └─ KeyCollector.cs
│  ├─ Menus/
│  │  ├─ GameOverMenu.cs
│  │  ├─ MainMenuPostProcessing.cs
│  │  ├─ MainMenuUI.cs
│  │  ├─ ParallaxBackground.cs
│  │  ├─ PauseMenu.cs
│  │  └─ SettingsMenu.cs
│  ├─ Player/
│  │  ├─ CameraFollow.cs
│  │  ├─ FootstepController.cs
│  │  ├─ GameHUD.cs
│  │  ├─ HealthSystem.cs
│  │  ├─ PlayerController.cs
│  │  └─ PlayerSpawner.cs
│  ├─ UI/
│  │  └─ EnemyTracker.cs
│  └─ DungeonManager.cs
```
