using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public delegate void PlayerDelegate();
    public static event PlayerDelegate OnPlayerDied;
    public static event PlayerDelegate OnPlayerScored;

    public float jumpHeight = 10;
    public float tiltSmooth = 5;
    public Vector3 startPos;

    Rigidbody2D rigidbody;
    Quaternion downRotation; // Rotation
    Quaternion forwardRotation;

    GameHandler game;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        downRotation = Quaternion.Euler(0, 0, -90);
        forwardRotation = Quaternion.Euler(0, 0, 35);
        game = GameHandler.Instance;

    }

    void OnEnable()
    {
        GameHandler.OnGameStart += OnGameStart;
        GameHandler.OnGameEnd += OnGameEnd;
    }

    void OnDisable()
    {
        GameHandler.OnGameStart -= OnGameStart;
        GameHandler.OnGameEnd -= OnGameEnd;
    }

    void OnGameStart()
    {
        rigidbody.velocity = Vector3.zero;
        rigidbody.simulated = true;
    }

    void OnGameEnd()
    {
        transform.localPosition = startPos;
        transform.rotation = Quaternion.identity;
    }

    void Update()
    {
        if (game.GameEnded) return;
        if (Input.GetMouseButtonDown(0))
            {
                transform.rotation = forwardRotation;
                rigidbody.velocity = Vector3.zero;
                rigidbody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Force);
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, downRotation, tiltSmooth * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ScoreZone")
        {
            OnPlayerScored(); // sent event to GameHandler
        }

        if (collision.gameObject.tag == "DeadZone")
        {
            rigidbody.simulated = false;
            OnPlayerDied(); // event sent to GameHandler
        }
    }
}
