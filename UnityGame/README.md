# Unity Gesture Control and Camera Follow System

This repository contains two key C# scripts for a **2D Platformer Game** built in Unity. These scripts enable **gesture-based character control** and an **adaptive camera-follow system**.

## 1. Gesture Control Script

This script allows the player character to be controlled using hand gestures that are sent via a Python socket connection. It listens for commands like `LEFT`, `RIGHT`, `JUMP`, and `STOP`, and moves the character accordingly.

### Features
- **Real-time Gesture Control**: The game character responds to hand gestures such as moving left, right, jumping, and stopping.
- **Socket Communication**: Listens for incoming commands from the Python script and translates them into character movement.
- **Smooth Movement**: Character movement is handled using Unity’s physics system (via `Rigidbody2D`) to provide smooth, responsive control.
- **Ground Detection**: The script ensures the character can only jump when grounded.

### Script Breakdown

#### Initialization
```
void Start()
{
    rb = GetComponent<Rigidbody2D>();
    listenerThread = new Thread(new ThreadStart(ListenForCommands));
    listenerThread.IsBackground = true;
    listenerThread.Start();
}
```
- The script starts a TCP listener in a background thread, ready to accept gesture commands from a Python socket.
- The character's `Rigidbody2D` is initialized for movement control.

### Listening for Commands

```

void ListenForCommands()
{
    listener = new TcpListener(IPAddress.Any, 65432);
    listener.Start();
    while (true)
    {
        client = listener.AcceptTcpClient();
        stream = client.GetStream();
        byte[] bytes = new byte[1024];
        int length;
        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            string command = Encoding.ASCII.GetString(bytes, 0, length).Trim();
            switch (command)
            {
                case "LEFT":
                    moveLeft = true;
                    moveRight = false;
                    break;
                case "RIGHT":
                    moveRight = true;
                    moveLeft = false;
                    break;
                case "JUMP":
                    jump = true;
                    break;
                case "STOP":
                    moveLeft = false;
                    moveRight = false;
                    break;
            }
        }
    }
}

```
The script listens for incoming commands over a socket. When a gesture command (`LEFT`, `RIGHT`, `JUMP`, or `STOP`) is received, the character's movement state is updated accordingly.

### Movement and Physics

```

void Update()
{
    rb.velocity = new Vector2(0, rb.velocity.y); // Reset horizontal velocity

    if (moveLeft) rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
    if (moveRight) rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    if (jump && isGrounded)
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jump = false; // Reset jump after action
    }
}

```

- The character’s horizontal movement is controlled by adjusting its velocity based on the received commands.
- Jumping is triggered when the jump flag is set and the character is grounded.

### Ground Detection

```

void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Ground"))
    {
        isGrounded = true;
    }
}

```

This detects when the character is on the ground, allowing jumping only when grounded.

### Clean Up

```

private void OnApplicationQuit()
{
    listener.Stop();
    if (client != null) client.Close();
}
```

On application exit, the socket connection is gracefully closed.

## 2. Camera 2D Follow Script

This script is responsible for the camera following the player in a 2D platformer. It smooths the camera’s movement and ensures the player remains in view.
Features

- **Smooth Camera Motion**: The camera smoothly follows the player, adjusting its position based on the player's movement.
- **Look-Ahead**: The camera looks ahead in the direction the player is moving, providing a better field of view.
- **Reframing**: The camera can smoothly transition between multiple targets using an animation curve.

## Script Breakdown
### Initialization

```
void Start()
{
    target = GameObject.FindGameObjectWithTag("Player").transform;
    myTransform = target.transform;
    m_LastTargetPosition = target.position;
    m_OffsetZ = (transform.position - target.position).z;
    transform.parent = null;
}
```
The script automatically finds the player by tag and adjusts the initial camera offset based on the player’s position.

### Camera Follow Logic

```

private void Update()
{
    if (target == null) return;

    float xMoveDelta = (target.position - m_LastTargetPosition).x;
    bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

    if (updateLookAheadTarget)
    {
        m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
    }
    else
    {
        m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
    }

    Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
    Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
    transform.position = newPos;
    m_LastTargetPosition = target.position;
}
```
- The camera moves smoothly towards the player’s position, using the SmoothDamp function to create a smooth motion.
- It also includes a look-ahead feature, where the camera moves slightly in the direction the player is heading.

### Reframing System

```

public void setTarget(Transform newTarget)
{
    currentOffSet = myTransform.position - newTarget.position;
    target = newTarget;
    StopAllCoroutines();
    StartCoroutine(reframeCurantine());
}
```
The setTarget method allows the camera to switch targets dynamically, with a smooth transition provided by the reframeCurantine coroutine.

```

IEnumerator reframeCurantine()
{
    Vector3 frameOffset = currentOffSet;
    float elapsed = 0f;
    float progress = 0f;
    while (progress < 1f)
    {
        currentOffSet = Vector3.LerpUnclamped(frameOffset, targetOffSet, reframeCurve.Evaluate(progress));
        yield return null;
        elapsed += Time.deltaTime;
        progress = elapsed / reFaremeTime;
    }
    currentOffSet = targetOffSet;
}
```

This coroutine smooths the transition between the camera’s current and new position when switching between targets.

## How to Use These Scripts

### 1. GestureControl Script:
- Attach this script to the player character GameObject.
- Ensure the player character has a `Rigidbody2D` component.
- Ensure the ground platforms in your game have the `Ground` tag for jump detection.

### 2. Camera2DFollow Script:
- Attach this script to the main camera.
- Ensure the player character GameObject is tagged as `Player` or assign the player GameObject as the target manually in the Unity Inspector.
- Adjust the `damping`, `lookAheadFactor`, and `lookAheadReturnSpeed` to tweak the camera behavior.

## Requirements

- Unity 2020.3+ (any version supporting 2D physics and animation curves).
- The Python gesture control script (included in this project) must be running to send commands via socket.

## License

This project is licensed under the MIT License. See the LICENSE file for more information.

Feel free to adapt and expand these scripts to fit your game design needs!
