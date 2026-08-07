## 1. Executive Summary

- **Game Title:** _Don't Water My Burrow_ (Working Title)    
- **Genre:** Top-Down 2D Free-Flow Tower Defense / Survival Action    
- **Platform:** PC / WebGL (Itch.io)    
- **Target Audience:** Casual & Mid-Core Strategy Players, Game Jam Enthusiasts    
- **Core Loop:** Collect Resources $\rightarrow$ Build Channels/Defenses $\rightarrow$ Divert/Drain Water Currents $\rightarrow$ Protect the Burrow $\rightarrow$ Clear the Wave.    

## 2. Core Game Loop & Mechanics

### 2.1 Game Objective & Victory/Defeat Conditions

- **Objective:** Guide flowing water away from the **Burrow** (located at the top) toward **Drains** (located at map sides/edges) before the Burrow's flood meter reaches $100\%$.    
- **Victory Condition:** Survive all scheduled water waves with the Burrow's flood meter below $100\%$.
- **Defeat Condition:** The Burrow's flood meter reaches $100\%$.

### 2.2 Water Dynamics (Free-Flow System)

- Water spawns from the **bottom edge** of the grid and naturally advances upward toward the Burrow.
- Without player intervention, water follows a straight top-down path.
- Building **Wood Channels** forces water vectors to change directions toward designated **Drains**.
- Water volume decreases when drained by **Drains** or absorbed by **Water Pumps**.

## 3. Entities & Interactions

### 3.1 Player Character (The Rabbit)

- **Movement:** Top-down smooth movement.
- **Actions:** 
  - **Resource Collection:** Collect materials (Wood, Stone, Sand) scattered around the map or spawned between waves.
  - **Building:** Preview and place structures in the grid cell immediately in front of the rabbit's facing direction.
  - **Repairing:** Spend resources to repair damaged structures before they break.
  - **Unclogging:** Manually interact with clogged Water Pumps or Drains to clear accumulated debris.

### 3.2 Player Defenses (Structures)

|**Structure**|**Mechanics**|**Ideal Use Case**|
|---|---|---|
|**Wood Channels**|Reroutes flowing water in a specified directional vector.|Diverting main water currents toward Drains.|
|**Sand Bags**|Physical blockades that stop water flow. Can be damaged and destroyed by debris.|Temporary holding lines; stopping direct streams.|
|**Water Pumps**|Continuously drains water volume in an area. Can become clogged by Leaves.|Lowering global water pressure near critical zones.|

### 3.3 Environmental Threats & Obstacles

| **Threat / Enemy** | **Spawn & Behavior** | **Mechanical Impact** | **Countermeasure** |
| --- | --- | --- | --- |
| **Rocks** | Floats along main water streams. | Deals impact damage to Sand Bags and structures. | Divert rocks using Wood Channels or repair defenses. |
| **Leaves** | Light debris floating in water. | Clogs Water Pumps and Drains upon contact, halting their operation. | Player must manually interact to clear leaves. |
| **Logs** | Heavy debris carried by water currents. | Forms dynamic dams on impact (occupying 2–3 cells) that block and alter water flow. | Build around newly formed dams or use them strategically. |
| **Mud** | Environmental hazard expanded from water spillover. | Reduces player movement speed by $50\%$ when stepped on. | Navigate carefully or build around mud zones. |

## 4. High-Level Technical Architecture

To maintain modularity and clear separation of concerns, the system uses a decoupled event-driven architecture with data-driven configuration.

```
  [ Player Actions & Entities ]
                │
                ▼
        [ Global Event Bus ]
        /       │        \
       ▼        ▼         ▼
  [ Water ]  [ Grid ]  [ Resources ]
       │        │         │
       ▼        ▼         ▼
  [ Simulation & UI Feedback ]
```

- **Data Configuration:** Game parameters, structure costs, and wave hazard schedules are driven by data configurations (ScriptableObjects).
- **Event-Driven Bus:** Systems communicate purely through decoupled event notifications without direct cross-manager dependencies.
- **Grid Representation:** World state is mapped onto a coordinate grid to resolve spatial occupancy, water flow direction, and building alignment.