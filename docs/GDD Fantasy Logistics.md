**Game Design Document (1-Month Solo Project)**  
*Arcane Conduit: Automated Fantasy Logistics*  

---

## 1. High-Level Concept
- **Title (Working):** Arcane Conduit  
- **Genre:** 2D Top-down Logistics / Automation / Resource Management  
- **Scope:** Solo developer, 4-week MVP emphasizing golem-based automation  
- **Platform:** PC (Windows/Linux)  
- **Art Style:** Simplified pixel art with magical visual effects  
- **Core Hook:** Deploy and optimize golem couriers, mana networks, and arcane machinery to convert raw materials into enchanted artifacts.

---

## 2. Core Gameplay Loop
1. **Extract Resources:** Place elemental siphons on node types (Wood, Crystal, Herb).  
2. **Golem Transport:** Spawn golems at command posts and assign them along waypointed routes.  
3. **Process Materials:** Route golems to Alchemy Processors and Ritual Circles for transmutation.  
4. **Power Management:** Build a mana conduit network linking generators to machines.  
5. **Artifact Assembly:** Deliver components into the Grand Conduit to synthesize the final artifact.  
6. **Upgrade & Expand:** Unlock new modules and improve golem abilities to increase throughput.

---

## 3. Features & Mechanics
| Feature                  | Description                                                                                   |
|--------------------------|-----------------------------------------------------------------------------------------------|
| **Resource Siphons**     | Automated extractors yielding raw materials at fixed intervals; consume mana per cycle.        |
| **Golem Couriers**       | AI workers carrying items along player-defined waypoint paths; configurable via command posts.|
| **Alchemy Processor**    | Station with input/output slots for recipe-based transmutations (e.g., Herb → Essence).        |
| **Ritual Circle**        | Area-effect unit performing continuous, batch-style conversions (e.g., Essence + Crystal).      |
| **Mana Conduit Network** | Infrastructure linking mana wells to machinery; supports conduit upgrades for efficiency.      |
| **Grand Conduit**        | End-game assembly machine combining cores and essences into the final enchanted artifact.       |
| **Modules & Upgrades**   | Unlock new siphons, processor modules, golem speed/capacity, and conduit improvements.          |

---

## 4. System Details

### 4.1 Resource Extraction
- **Nodes:** Trees Patches, Crystal Veins, Herb Patches, Stone Quarries.  
- **Siphon Rates:** Wood: 1 log/10s; Crystal: 1 shard/8s; Herb: 1 leaf/6s.  
- **Mana Cost:** 1 mana per extraction.

### 4.2 Golem Transport
- **Command Post:** Spawns golems; defines working radius.  
- **Capacity:** 5 items or 10 liquid units per golem.  
- **Pathing:** Player draws waypoint loops; golems follow in sequence.  
- **Speed:** Base 1 tile/sec; upgradeable.

### 4.3 Processing Stations
- **Alchemy Processor:**  
  - 3 input slots, 1 output slot  
  - **Recipe A:** Herb ×3 → Essence ×1 (10s, 5 mana)  
  - **Recipe B:** Wood ×2 + Essence ×1 → Charcoal Infusion ×1 (12s, 8 mana)  
- **Ritual Circle:**  
  - **Ritual A:** Essence ×2 + Crystal ×1 → Arcane Dust (15s, 12 mana)  
  - **Ritual B:** Charcoal Infusion ×1 + Arcane Dust ×1 → Arcane Core (18s, 15 mana)

### 4.4 Mana Network
- **Mana Wells:** Generate 10 mana/sec; place strategically.  
- **Conduits:** Transfer mana to power machines;   
- **Modules:** Mana Capacitor (+50 storage).

### 4.5 Final Assembly
- **Grand Conduit Inputs:** Arcane Core ×2, Essence ×3, Rune (no cost).  
- **Assembly Time:** 20s; **Mana Cost:** 30.  
- **Outcome:** Final Artifact (win condition).

---

## 5. Progression & Upgrades
- **Week 1:** Base siphons, golem command posts, mana wells, conduit Tier 1.  
- **Week 2:** Alchemy Processor, golem capacity upgrade, Mana Capacitor.  
- **Week 3:** Ritual Circle, Conduit Tier 2, Golem speed upgrade.  
- **Week 4:** Grand Conduit, “Rune Stabilizer” module (reduces assembly time by 20%).

---

## 6. UI & UX
- **Build Menu:** Sections for Siphons, Golems, Processing, Power, Modules.  
- **Waypoint Editor:** Click-to-place waypoints; visualize loops.  
- **Overlay Modes:**  
  - **Power:** Shows mana flow & loss.  
  - **Transport:** Highlights golem paths and congestion.

---

## 7. Art & Audio
- **Visuals:** Modular pixel art/tilemap assets, glowing effects for magic.  
- **Animations:** Golem walk cycles, alchemy bubbling, conduit pulses.  
- **SFX:** Resource extraction, machinery hum, ritual chimes.

---

## 8. Technical Stack
- **Engine:** Unity 2D (Tilemap + ECS)  
- **Language:** C#  
- **Data:** ScriptableObjects for machine/recipe definitions

---

## 9. Development Roadmap (4 Weeks)
| Week | Milestones                                                                                     |
|------|------------------------------------------------------------------------------------------------|
| 1    | Project setup; implement resource nodes, siphons, golems with waypoint system; basic UI overlays. |
| 2    | Alchemy Processor; golem upgrades (capacity); Mana Capacitor; upgrade menu.                      |
| 3    | Ritual Circle; Conduit Tier 2 & pipeline; Golem speed upgrade; overlay polish.                   |
| 4    | Grand Conduit; “Rune Stabilizer” module; audio & visual polish; playtesting & bug fixes; build. |

---

## 10. Success Criteria
- **Automation:** Seamless golem-driven pipeline from extraction to final artifact.  
- **UX:** Clear overlays, intuitive building, minimal micromanagement.  
- **Performance:** Stable 60+ FPS with medium-scale factory setups.  
- **Completion:** MVP feature-complete within 4 weeks.  
