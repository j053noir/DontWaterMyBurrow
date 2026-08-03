# 🐰 Don't Water My Burrow 🌊

> **"Don't Water My Burrow"** is a 2D Free-Flow Tower Defense & Survival Action game built in Unity 6 using C#. Players control a brave rabbit who must construct channels, place sandbags, and manage water pumps to divert incoming water currents away from its burrow towards designated drains.

This project is being developed as a 100% human-crafted preparation project for the **Brackeys Game Jam**, completely **free of AI-generated assets or code**.

---

## 🎯 Game Features & Mechanics

* **Free-Flow Water System:** Water spawns from the bottom of the map and flows upward dynamically.
* **Strategic Redirection:** Use **Wood Channels** to bend water flow vectors directly into side drains.
* **Defensive Structures:** * 🧱 **Sand Bags:** Block water temporarily (Vulnerable to heavy rocks).
  * 🪣 **Water Pumps:** Reduce local water levels (Can be clogged by leaves).
  * 🪵 **Wood Channels:** Change water flow directions.
* **Dynamic Hazards:** Watch out for floating **Rocks**, pump-clogging **Leaves**, path-blocking **Logs**, and movement-slowing **Mud**.

---

## 🏛️ Technical Architecture & Principles

Built with maintainability, scalability, and clean code principles in mind:

* 📡 **Event Bus Pattern:** Completely decoupled communication system using generic C# events.
* 📊 **Data-Driven Design (ScriptableObjects):** Entity stats, building metrics, and wave parameters managed isolated from code.
* ⚙️ **Finite State Machine (FSM):** Player behavior decoupled into clean, isolated state classes (`IdleState`, `WalkState`, `InteractState`, `DeadState`).
* 🧩 **SOLID Principles:** Strict adherence to single-responsibility, open-closed extensions, and dependency inversion.
* 🎨 **UI Toolkit:** Flexbox-based UI layout and styling using UXML & TSS.

---

## 🛠️ Built With

* **Engine:** Unity 6 (2D Engine)
* **Language:** C#
* **Graphics:** Pixel Art (Custom / CC0 Assets from Kenney.nl)
* **Audio:** BFXR / Chiptone (SFX) & BeepBox (8-bit BGM)

---

## 👥 Community & Team

Developed live as part of a YouTube Devlog series preparing for upcoming Game Jams. 

* **YouTube:** [@josegamedev](https://www.youtube.com/@josegamedev)
* **Discord Community:** Join our team [Discord](https://discord.gg/Kmn3yHq4cq)
