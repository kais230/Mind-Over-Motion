# EEG Focus Streamer + Unity Game Integration

This guide explains how to set up the **EEG Focus Streamer** and the **Mind Over Motion** game on a new machine. The setup involves a Python backend (for Muse 2 EEG processing) and a Unity frontend (2D brainwave-controlled platformer).

---

## 🧠 EEG Focus Streamer Setup (Python)
This section helps you set up the `focus_streamer.py` script that calculates focus (concentration) levels using Muse 2 EEG data and sends them to Unity.

---

### 1. Install Python 3.11 (or newer)

Download and install Python from:  
👉 https://www.python.org/downloads/release/  

> ✅ **IMPORTANT**: During installation, **check** "Add Python to PATH".

---

### 2. Install required Python packages

Open a terminal (CMD, PowerShell, or Bash) in the Python folder and run:

```bash
pip install -r requirements.txt
```

This installs:
- `numpy`
- `scipy`
- `matplotlib`
- `pylsl`
- `muselsl`

---

### 3. Connect & pair your Muse headset

- Enable Bluetooth on your computer.
- Power on the Muse 2 headset .

---

### 4. Start the EEG stream from the Muse headset

In terminal:

```bash
py -3.11 -m muselsl stream
```

Keep this terminal open — it starts broadcasting EEG data via **LSL (Lab Streaming Layer)**.

---

### 5. Run the focus streamer app

Open a **new terminal window** and run:

```bash
py -3.11 focus_streamer.py
```

You should see:
- A live EEG power plot.
- Console output showing the current **focus score** (0 to 100).
- The focus score is also **sent to Unity** via UDP on `localhost:5005`.

---

### 6. Troubleshooting

- 🛑 **No EEG stream found?**
  - Make sure Muse is paired and powered on.
  - Ensure `muselsl stream` is running.
- 🔥 **Firewall issues?**
  - Allow Python through firewall.
  - Ensure **UDP port 5005** is open.
- ✅ Always run things in this order:
  1. `muselsl stream`
  2. `focus_streamer.py`

---

## 🎮 Unity Game Setup

This section explains how to pull, configure, and open the Unity project on any computer using **Unity Hub**.

---

### 1. Clone the repository

Open terminal and run:

```bash
git clone https://github.com/your-username/your-repo-name.git
```

Then navigate to the project folder:

```bash
cd your-repo-name
```

---

### 2. Open the project in Unity Hub

1. Launch **Unity Hub**.
2. Click **Add**.
3. Select the cloned project folder.
4. Unity Hub will detect the version. If not, choose the correct Unity version (e.g., `2022.3 LTS`).
5. Click **Open**.

---

### 3. Run the game

1. Open the main game scene (e.g., `Assets/Scenes/Main.unity`).
2. Click **Play** in the Unity Editor.
3. `focus_streamer.py` doesn't need to run beforehand, you can connect via settings menu in the game.

**Game Features:**
- Move using **WASD**.
- **Attack** with mouse click (power depends on concentration).
- Press **X** to cast a fireball (with cooldown).
- Break walls or reveal hidden platforms using high focus.

---

## ⚙️ Unity UDP Setup (Optional Dev Info)

- Unity listens on **UDP port 5005**.
- Data format sent from Python: a float (0–100) representing focus level.

If you need to change the port or address, modify the corresponding Python and Unity scripts.

---


## 🧪 Credits

- Aplicatie de procesare a semnalelor cerebrale, Al-Hajjih Kais
- **Unity Brainwave Game**: 2D platformer powered by EEG

---
