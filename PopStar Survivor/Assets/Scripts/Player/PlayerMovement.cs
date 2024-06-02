using System.Collections;
using System.Collections.Generic;
using Terresquall;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Movimiento

    
    [HideInInspector]
    public float lastHorizontalVector;
    [HideInInspector]
    public float lastVerticalVector;
    [HideInInspector]
    public Vector2 moveDir;
    [HideInInspector]
    public Vector2 lastMovedVector;

    // Referencias
    Rigidbody2D rigidbody2D;
    PlayerStats player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStats>();

        rigidbody2D = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); // De serie, las armas de proyectil dispararán hacia la derecha.
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();

    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {

        if (GameManager.instance.isGameOver)
        {
            return;
        }

        float moveX, moveY;
        if (VirtualJoystick.CountActiveInstances() > 0)
        {
            moveX = VirtualJoystick.GetAxisRaw("Horizontal");
            moveY = VirtualJoystick.GetAxisRaw("Vertical");

        }
        else
        {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        }
        

        moveDir = new Vector2(moveX, moveY).normalized;

        if (moveDir.x != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastMovedVector = new Vector2(lastHorizontalVector, 0f);
        }

        if (moveDir.y != 0)
        {
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(0f, lastVerticalVector);
        }

        if (moveDir.x != 0 && moveDir.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rigidbody2D.velocity = new Vector2(moveDir.x * player.CurrentMoveSpeed, moveDir.y * player.CurrentMoveSpeed);
    }
}
