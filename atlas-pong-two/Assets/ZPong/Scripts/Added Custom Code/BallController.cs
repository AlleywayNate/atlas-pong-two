using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Transform playerPaddle; // The paddle from which the ball will start
    public float serveForce = 10f; // The force applied when serving the ball

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartBall();
    }

    void StartBall()
    {
        // Position the ball at the paddle's location
        transform.position = playerPaddle.position;

        // Stop any existing motion
        rb.velocity = Vector2.zero;

        // Optionally, you can add a delay before the ball is served
        Invoke("ServeBall", 1f); // Adjust delay as needed
    }

    void ServeBall()
    {
        // Add initial force to the ball to start gameplay
        // Example: Launch the ball at a specific angle
        rb.velocity = new Vector2(serveForce, Random.Range(-serveForce, serveForce));
    }
}
