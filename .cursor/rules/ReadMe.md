# Project: Birthday Tycoon (Gift Project)

**CONTEXT**
This project is a **Casual 2D Simulation Game** developed in Unity (C#) as a **Birthday Gift**.
The goal is to create a stress-free, heartwarming experience where the player (the girlfriend) works, plays mini-games, and eventually buys a gift.

**CORE PHILOSOPHY**
1.  **Player Advantage:** The game is rigged in the player's favor (e.g., 70% win rate in gambling).
2.  **No Frustration:** Bankruptcy is impossible. "Sleep" is always free.
3.  **Romantic & Cute:** Visuals and text should be warm and encouraging.

---

## 🛠 Tech Stack & Coding Standards
* **Engine:** Unity 2022+ (2D).
* **Language:** C#.
* **UI:** **TextMeshProUGUI**.
* **Data:** **ScriptableObject**.
* **Comments:** **English comments only**.

---

## 🏗 Architecture Overview

### 1. Managers (Singletons)
* **`GameManager`:** Global State (Money, Fatigue, DayCount, Buffs).
* **`UIManager`:** Manages UI Panels. Now supports specific Game Panels.
* **`GameSystem` (New!):**
    * Acts as the **Arcade Hub Controller**.
    * Handles entering/exiting specific mini-games.
    * Manages the state of "Playing Game" vs "In Hall".

### 2. Interaction: Arcade Machines
* **`ArcadeMachine` (Script):**
    * Attached to World Objects (2D Sprites of machines in the Hall).
    * **Input:** Detects clicks (`OnMouseDown` or `Button`).
    * **Action:** Notifies `GameSystem` to launch the specific game type.

### 3. Mini-Games Logic
* **`SlotMachineGame`:** Weighted Random (70% Win).
* **`WhacAMoleGame`:** Reflex based.
* **Interface:** All games should implement a basic `IGameController` or inherit from a base class to handle `StartGame()` and `EndGame()`.

---

## 🔮 Game Logic Details

### Game Flow: Arcade Interaction
1.  **In Hall:** Player sees multiple machine sprites (Slot Machine, Mole Machine).
2.  **Click:** Player clicks a machine (`ArcadeMachine`).
3.  **Transition:** `GameSystem` triggers `UIManager` to show the specific Game Panel (e.g., `Panel_SlotMachine`).
4.  **Play:** Player plays the game.
5.  **Exit:** Player clicks "Close/Back", `GameSystem` returns to Hall.

### (Retain previous logic for Fortune, Slot Machine Odds, etc...)
* **Slot Machine:** 70% Win Rate.
* **Fortune:** Buffs/Debuffs logic remains the same.

---

**INSTRUCTION TO AI**
When implementing the `GameSystem`:
1.  Create an `Enum GameType` { None, SlotMachine, WhacAMole }.
2.  Ensure `ArcadeMachine` script is simple and reusable for any game type.