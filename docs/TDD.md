# Technical Design Document (TDD)  
**Project:** Arcane Conduit  
**Engine:** Unity (2D URP, Tilemap, ECS-friendly but classic MonoBehavior based unless scoped otherwise)  
**Language:** C#  
**Target:** PC (Windows/macOS)  
**Scope:** Solo Developer, 1 Month MVP  

---

## 1. Project Architecture

### 1.1 Layered Architecture
- **Presentation Layer (UI):** Handles all user interactions and feedback.
- **Game Logic Layer:** Manages simulation, automation, state machines, and time cycles.
- **Data Layer:** Contains ScriptableObjects for recipes, buildings, upgrades, etc.
- **Persistence Layer (Optional):** Basic save/load system using JSON serialization.

---

## 2. Core Systems

### 2.1 Resource System
**ScriptableObjects:**
- `ResourceType`: ID, name, icon, stack size
- `ResourceNode`: type, extraction rate, visual prefab

**MonoBehaviors:**
- `ResourceNodeBehavior`: handles timer-based extraction and animation
- `Siphon`: checks for node type, consumes mana, outputs `ResourceType`

---

### 2.2 Golem System
**Core Components:**
- `GolemAgent`: Manages inventory, movement, and task state.
- `CommandPost`: Spawner/Manager for Golems within a defined radius.
- `Waypoint`: Transform positions with editor support.

**Pathing:**
- Use NavMesh (2D) or manual grid/path array if more performant.
- Waypoint loop assignment via editor or runtime drag tool.

**Inventory:**
```csharp
class InventorySlot {
    ResourceType resource;
    int quantity;
}
```

---

### 2.3 Processing System

**Alchemy Processor / Ritual Circle**
```csharp
class ProcessingMachine {
    List<Recipe> recipes;
    Inventory input;
    Inventory output;
    float processTimer;
}
```

**Recipe ScriptableObject**
```csharp
class Recipe : ScriptableObject {
    List<ResourceAmount> inputs;
    List<ResourceAmount> outputs;
    float timeToProcess;
    int manaCost;
}
```

---

### 2.4 Mana Network

**Components:**
- `ManaWell`: Produces mana over time.
- `ManaConduit`: Transfers mana; implements loss per tile.
- `ManaConsumer`: Base class for all devices using mana.

**Logic:**
- Breadth-first conduit scan to calculate power routes.
- Optional use of graph representation to optimize routes.

---

### 2.5 Grand Conduit System

**Behavior:**
- Input buffer of required items.
- If all required materials present, trigger animation, consume resources, produce final artifact.
- Emits win event to trigger end state.

---

## 3. Game State & Management

### 3.1 Game Manager
Central coordinator for:
- Tick updates
- Global references (UI, resource registry, game time)

```csharp
class GameManager {
    List<IUpdatable> updatables;
    float tickRate = 0.5f;
}
```

---

### 3.2 UI Management
**Systems:**
- Build Menu (Category Tabs)
- Resource Overlay Toggles
- Golem Routing UI
- Machine Interaction Panel

Use Unity UI Toolkit or Canvas UI with ScriptableObject binding.

---

## 4. Data Structures

### 4.1 ScriptableObjects
- `ResourceType`, `Recipe`, `BuildingType`, `UpgradeData`, `GolemData`

### 4.2 Serialization
- Basic save/load: JSON with custom DTO classes.
- Auto-save on scene unload.

---

## 5. Development Tools

### 5.1 In-Editor Tools
- Waypoint Placement Tool (custom Gizmos)
- Golem Route Visualizer
- Mana Flow Debug Overlay

---

## 6. Performance & Optimization

- Object Pooling for golems and particles
- Use DOTS-style logic only if required
- Simplified physics and animation (no Rigidbody2D unless collision essential)

---

## 7. Milestones & Technical Goals

| Week | Technical Focus Areas                                                                 |
|------|----------------------------------------------------------------------------------------|
| 1    | Core framework, resource extraction, golem system (movement + inventory)              |
| 2    | Processing machines (alchemy + rituals), command post logic, simple mana network      |
| 3    | Advanced conduit logic, upgrade system, grand conduit, overlays, visual polish        |
| 4    | Bug fixing, save/load system, UI polish, deployable build                             |

---

## 8. Dependencies / Assets

- Tilemap & 2D animation packs (Kenney, custom)
- Unity NavMesh or A* Pathfinding Project (if needed)
- LeanTween / DOTween (for UI and transition effects)
- Odin Inspector (optional for in-editor tools)

---

## 9. Testing Strategy

- Unit testing of crafting logic and golem state machines
- Integration testing of mana networks
- Manual playtests for UX, congestion, pacing

---

## 10. Stretch Goals (Time-Permitting)

- Endless mode (score via artifacts/min)
- New biomes (e.g. Lava, Ice) with different node logic
- Dynamic weather affecting mana or golems
