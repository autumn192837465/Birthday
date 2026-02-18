# Project: Birthday Tycoon (Gallery Edition)

**CONTEXT**
This project is a **Casual Gallery Management Simulation** developed in **Unity 6** (C#) as a **Birthday Gift**.
The goal is to create a heartwarming experience where the player (the girlfriend) creates art, manages her own gallery, earns passive income through renting/selling paintings, and eventually buys a gift.

**CORE PHILOSOPHY**
1.  **Artist & Landlord Fantasy:** The game validates the player's artistic talent and love for passive income.
2.  **No Stress:** "Bankruptcy" is impossible. "Sleep" is free and fully restores Fatigue.
3.  **Data-Driven:** All probabilities, prices, and stats must be controlled via a `ScriptableObject`.

---

## 🛠 Tech Stack & Coding Standards
* **Engine:** **Unity 6** (2D).
* **Language:** C#.
* **Animation:** **None** (Focus strictly on logic and UI functionality for now).
* **UI:** **TextMeshProUGUI**.
* **Data:** **ScriptableObject** (Mandatory for balancing).
* **Comments:** **English comments only**.

---

## 🏗 Architecture Overview

### 1. Managers (Singletons)
* **`GameManager`:**
    * Manages Global State: `Money`, `Fatigue` (0 to Max), `DayCount`.
    * Handles the game loop: Create Art -> Sleep -> Market Calculation -> Next Day.
* **`UIManager`:**
    * Manages UI Panels (`HomePanel`, `GalleryPanel`, `TarotPanel`, `ShopPanel`).
    * Updates text elements (Money text, Fatigue text).
    * Displays market results as a Toast message after Sleep.
* **`GalleryManager` (Core System):**
    * Manages the **Inventory** of paintings.
    * Manages **Display Slots** (Which paintings are on the wall).
    * Handles the **Creation Process** (Increases Fatigue to generate Art).
* **`MarketManager` (Logic System):**
    * Handles the **Daily Calculation** of Renting vs. Selling.
    * Reads probabilities directly from `GameSettings`.

### 2. Data Structure
* **`Painting` (Class):**
    * `string ID`: Unique identifier.
    * `string Title`: Name of the art.
    * `Sprite Image`: The visual.
    * `int BasePrice`: Value used for rent/sell calc.
    * `PaintingState State`: { `Inventory`, `Displayed`, `Rented`, `Sold` }.
    * `int RentDaysLeft`: Tracks when a rented painting returns.

* **`GameSettings` (ScriptableObject):**
    * **Resource:** `MaxFatigue`, `PaintingFatigueCost`.
    * **Economy:** `PaintingBasePrice` (Range min/max), `PromotionCost`.
    * **Probabilities (RNG):** `BaseRentChance` (e.g., 0.3), `BaseSellChance` (e.g., 0.05).
    * **Rent Logic:** `RentIncomeMultiplier` (e.g., 0.1 per day), `MinRentDays`, `MaxRentDays`.

---

## 🎨 Game Logic Details

### A. Core Loop: The Artist's Life
1.  **Create Art:** Player clicks "Canvas".
    * *Condition:* `Fatigue` + `Cost` <= `MaxFatigue`.
    * *Action:* `Fatigue` += `PaintingFatigueCost`.
    * *Result:* Add new `Painting` to Inventory.
2.  **Curate:** Player opens "Gallery UI" and selects paintings from Inventory to move to `Displayed` slots.
3.  **Sleep (End Day):** Player clicks "Sleep".
    * *Action:* **Reset `Fatigue` to 0**.
    * *Trigger:* `MarketManager.ProcessDailyMarket()`.
    * *Action:* `DayCount`++.
4.  **Profit:** Show a Toast summary of what happened overnight (Rent collected / Paintings sold).

### B. Market Logic (The "Rent" System)
When the day changes, iterate through all **Displayed** and **Rented** paintings:

**1. For `Displayed` Paintings (On the Wall):**
* **Load Rates:** Get `BaseRentChance` and `BaseSellChance` from `GameSettings`.
    * *Promotion Check:* If Promotion is active, double these rates.
* **Check Sell (Low Chance):**
    * If `Random.value < SellChance`:
        * Painting State = `Sold` (Removed from gallery forever).
        * Player gets **Huge Lump Sum** (`BasePrice * 10`).
* **Check Rent (High Chance):**
    * *Else If* `Random.value < RentChance`:
        * Painting State = `Rented` (Removed from wall temporarily).
        * Set `RentDaysLeft` to Random range (`MinRentDays`, `MaxRentDays`).
        * Player starts earning daily rent from tomorrow.

**2. For `Rented` Paintings (With Customer):**
* **Collect Rent:** Player gains `BasePrice * RentIncomeMultiplier` immediately.
* **Countdown:** Decrement `RentDaysLeft`.
* **Return:** If `RentDaysLeft` <= 0, Painting State = `Displayed` (Returns to wall available for rent again).

### C. Promotion System
* **Action:** Player pays money to "Buy Promotion".
* **Effect:** For the **Next Day Only**, `RentChance` and `SellChance` are doubled.
* **Reset:** Flag resets to false after `ProcessDailyMarket` runs.

### D. Fatigue (Resource)
* **Logic:** Starts at 0. Max is defined in Settings (e.g., 100).
* **Consumption:** Creating a painting ADDS Fatigue.
* **Recovery:** "Sleep" resets Fatigue to 0.



**INSTRUCTION TO AI**
When implementing the code:
1.  **Strictly use `GameSettings` for ALL magic numbers.** Do not hardcode probabilities (0.3, 0.05) inside the logic scripts.
2.  **Unity 6 Compatibility:** Use standard `MonoBehaviour` and `SerializeField`.
3.  **Market Logic:** Ensure "Sell" is checked **before** "Rent".
4.  **Prioritize Rent:** The game design emphasizes passive income (Landlord style), so default Rent chance should be significantly higher than Sell chance in the settings.