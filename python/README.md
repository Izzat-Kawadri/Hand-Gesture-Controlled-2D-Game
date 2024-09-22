# Hand Gesture Detection and Control Script

This Python script leverages the **MediaPipe** library to detect hand gestures from a webcam feed and sends commands to a connected Unity game via **socket communication**. The detected hand gestures (such as moving left, right, jumping, or stopping) are translated into movement commands for the game character.

## Project Overview

The script captures real-time hand movements using **OpenCV** and **MediaPipe's Hands solution** to detect hand landmarks. Based on the relative positions of the hand landmarks, the script identifies specific gestures. The recognized gestures are then translated into commands (like `RIGHT`, `LEFT`, `JUMP`, `STOP`) and sent to the Unity game via a socket connection.

### Key Features:
- **Real-time Hand Gesture Detection** using the MediaPipe library and OpenCV for webcam capture.
- **Gesture Recognition** based on the relative hand landmark positions.
- **Socket Communication** to send commands (such as `MOVE_RIGHT`, `MOVE_LEFT`, `JUMP`, or `STOP`) to a Unity game.
- **Visual Feedback** by drawing hand landmarks on the webcam feed.

## Prerequisites

Before running the script, ensure you have the following dependencies installed:

### Python Libraries:
- **OpenCV** for real-time video capture:
    ```
    pip install opencv-python
    ```

- **MediaPipe** for hand gesture detection:
    ```
    pip install mediapipe
    ```

- **Socket Library** is already included in the Python standard library.

## How the Script Works

1. **Webcam Capture (OpenCV)**:
    - The script captures the video stream from your computer’s camera using OpenCV.

2. **Hand Gesture Detection (MediaPipe)**:
    - **MediaPipe Hands** is used to detect and track hand landmarks in real-time.
    - The relative positions of the landmarks are analyzed to determine the current hand gesture.

3. **Gesture Recognition**:
    - The script calculates hand dimensions (width and height) based on the x and y coordinates of hand landmarks.
    - Based on the position and size of the hand, the script identifies gestures:
        - **Move Left**: Detected when the hand is predominantly on the left side of the screen.
        - **Move Right**: Detected when the hand is predominantly on the right side of the screen.
        - **Jump**: Detected when the hand is raised to the top of the frame.
        - **Stop**: Detected when no movement or hand is centered.

4. **Socket Communication**:
    - The script connects to a Unity game via a socket and sends the recognized gesture as a command.
    - The commands are encoded as strings (`RIGHT`, `LEFT`, `JUMP`, `STOP`) and transmitted to control the game character.

5. **Visualization**:
    - The script provides visual feedback by drawing hand landmarks on the webcam feed, displaying the current detected gesture in real-time.

## Code Breakdown

### 1. **MediaPipe Initialization**
```
mp_drawing = mp.solutions.drawing_utils
mp_hands = mp.solutions.hands
```

MediaPipe is initialized to detect hand landmarks and provide drawing utilities for visualization.
### 2. **Socket Setup**

```
server_address = ('localhost', 65432)
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(server_address)
```

The socket connects to the Unity game server running on the local machine (`localhost`) on port `65432`.

### 3. **Gesture Detection Logic**

```

if hand_width > hand_height:
    if hand_width > GESTURE_THRESHOLD:
        if min_x < GESTURE_THRESHOLD:
            current_gesture = gestures['MOVE_LEFT']
        elif max_x > 1 - GESTURE_THRESHOLD:
            current_gesture = gestures['MOVE_RIGHT']
        else:
            current_gesture = gestures['STOP']
else:
    if hand_height > GESTURE_THRESHOLD:
        if min_y < GESTURE_THRESHOLD:
            current_gesture = gestures['JUMP']
        else:
            current_gesture = gestures['STOP']

```

This block calculates the relative dimensions of the hand and detects gestures based on the hand's width and height. Different gestures are mapped to corresponding commands.

### 4. **Sending Commands to Unity**

```
sock.sendall(current_gesture.encode())
```

Once a gesture is detected, the corresponding string command (`RIGHT`, `LEFT`, `JUMP`, or `STOP`) is sent over the socket to Unity.

### 5. **Drawing Hand Landmarks**

```
mp_drawing.draw_landmarks(image, hand_landmarks, mp_hands.HAND_CONNECTIONS)
```

Hand landmarks are drawn on the webcam feed, providing visual feedback.

## Running the Script

 ### 1. **Start the Unity Game**:
Make sure the Unity game server is running and listening on localhost:`65432`.

 ###  2. **Run the Python Script**:
Open a terminal and run the following command to start the script:
 ```
 python hand_gesture_control.py
 ```

 ### 3.  **Control the Game Using Gestures** :
Move your hand in front of the camera to control the game character:
  - **Move hand left**: Move `Left` command.
  - **Move hand right**: Move `Right` command.
  - **Raise hand up**: `Jump` command.
  - **Hold hand still**: `Stop` command.

 ### 4.  **Quit** :
Press q to stop the script and close the window.

## Customization

-  **Adjust Gesture Threshold**: The sensitivity of gesture detection can be fine-tuned by changing the `GESTURE_THRESHOLD` value in the script.
- **Add More Gestures**: You can extend the script by adding new gesture recognition patterns in the gestures dictionary.

## Example Output

When the script detects a gesture, it prints the detected action:
```
MOVE_LEFT
MOVE_RIGHT
JUMP
STOP
```

## Troubleshooting

- **Camera Not Detected**: Ensure that your webcam is connected properly and accessible by OpenCV.
- **Unity Connection Issues**: Ensure the Unity server is running on the correct localhost and port (`65432`). Double-check the socket connection details in both the Python script and Unity.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.

Feel free to modify and expand this script to add more complex gestures or game mechanics!
