using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class GestureControl : MonoBehaviour
{
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    private Thread listenerThread;

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    private bool moveLeft, moveRight, jump;
    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        listenerThread = new Thread(new ThreadStart(ListenForCommands));
        listenerThread.IsBackground = true;
        listenerThread.Start();
    }

    void Update()
    {
        // Reset horizontal velocity
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (moveLeft)
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
        else if (moveRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }

        if (jump && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
        }
    }

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
                var incommingData = new byte[length];
                System.Array.Copy(bytes, 0, incommingData, 0, length);
                string command = Encoding.ASCII.GetString(incommingData).Trim();

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void OnApplicationQuit()
    {
        listener.Stop();
        if (client != null)
        {
            client.Close();
        }
    }
}
