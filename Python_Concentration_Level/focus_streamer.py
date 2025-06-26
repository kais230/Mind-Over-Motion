import numpy as np
from pylsl import StreamInlet, resolve_byprop
from scipy.signal import welch
import socket
import time
import matplotlib.pyplot as plt
from collections import deque

# Network settings
UDP_IP = "127.0.0.1"
UDP_PORT = 5005

# EEG & processing settings
CHANNEL_INDEX = 0
WINDOW_SIZE = 256
SAMPLING_RATE = 256

# Plotting settings
PLOT_DURATION = 5  # seconds of EEG to display
BUFFER_SIZE = PLOT_DURATION * SAMPLING_RATE
REFRESH_INTERVAL = 0.05  # seconds

# Resolve LSL EEG stream
print("Looking for EEG stream...")
streams = resolve_byprop('type', 'EEG', timeout=5)
if not streams:
    raise RuntimeError("No EEG stream found.")
inlet = StreamInlet(streams[0], max_chunklen=WINDOW_SIZE)
print("Connected to Muse stream.")

# UDP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Initialize EEG buffer
eeg_buffer = deque(maxlen=BUFFER_SIZE)
time_buffer = deque(maxlen=BUFFER_SIZE)
t = 0

# Matplotlib live plot setup
plt.ion()
fig, ax = plt.subplots()
line, = ax.plot([], [], lw=1.5)
ax.set_ylim(-150, 150)
ax.set_xlim(0, PLOT_DURATION)
ax.set_xlabel("Time (s)")
ax.set_ylabel("EEG (µV)")
ax.set_title("Live EEG Signal")

def bandpower(data, sf, band):
    freqs, psd = welch(data, sf, nperseg=WINDOW_SIZE)
    idx_band = np.logical_and(freqs >= band[0], freqs <= band[1])
    return np.trapezoid(psd[idx_band], freqs[idx_band])



# Initialize focus smoothing
prev_focus = 50

last_plot_time = time.time()

while True:
    samples, _ = inlet.pull_chunk(timeout=1.0, max_samples=WINDOW_SIZE)
    if len(samples) < WINDOW_SIZE:
        continue

    eeg_data = np.array(samples)[:, CHANNEL_INDEX]
    
    # Add to buffer for plotting
    for value in eeg_data:
        eeg_buffer.append(value)
        time_buffer.append(t)
        t += 1 / SAMPLING_RATE

    # === Focus calculation ===
    alpha = bandpower(eeg_data, SAMPLING_RATE, [8, 12])
    beta = bandpower(eeg_data, SAMPLING_RATE, [13, 30])

    ratio = beta / (alpha + 1e-6)
    ratio = min(ratio, 3.0)
    normalized_score = np.interp(ratio, [0.5, 2.5], [0, 100])
    focus_score = int(np.clip(normalized_score, 0, 100))
    
    focus_score = int(0.8 * prev_focus + 0.2 * focus_score)
    prev_focus = focus_score

    sock.sendto(str(focus_score).encode(), (UDP_IP, UDP_PORT))
    print(f"Focus: {focus_score}")

    # === Update live plot every X seconds ===
    current_time = time.time()
    if current_time - last_plot_time > REFRESH_INTERVAL:
        line.set_xdata(np.array(time_buffer) - time_buffer[0])
        line.set_ydata(eeg_buffer)
        ax.set_xlim(0, PLOT_DURATION)
        ax.figure.canvas.draw()
        ax.figure.canvas.flush_events()
        last_plot_time = current_time

    time.sleep(0.01)
#py -3.11 -m muselsl stream
#py -3.11 focus_streamer.py
