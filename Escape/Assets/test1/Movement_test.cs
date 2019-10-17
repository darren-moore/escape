using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_test : MonoBehaviour
{
    [Header("Player Properties")]

    public float moveSpeed = 1;
    public float decel = 1;
    public float maxSpeed = 20;
    public Rigidbody2D rb;
    public float dashAccel = 10.0f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2.0f;

    public KeyCode[] Inputs;  // 0 = "W", 1 = "A", 2 = "S", 3 = "D", 4 = "Boost/Dash"

    private float dashTi;
    private int rotState; // idk maybe use for deciding what sprite to use at different rotations
    private int state = 0;  // Free state, animation state, stun state, etc. 
    private Vector2 dir = Vector2.zero;
    private float idrag;


    // Start is called before the first frame update
    void Start()
    {
        if (rb == null && this.gameObject.GetComponent<Rigidbody2D>())
            rb = this.gameObject.GetComponent<Rigidbody2D>();
        idrag = rb.drag;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        dir = new Vector2(0, 0);
        /*
        if ((Input.GetKeyDown("w") || Input.GetKey("w")) && !Input.GetKey("s"))
            dir.y = 1;
        if ((Input.GetKeyDown("d") || Input.GetKey("d")) && !Input.GetKey("a"))
            dir.x = 1;
        if ((Input.GetKeyDown("s") || Input.GetKey("s")) && !Input.GetKey("w"))
            dir.y = -1;
        if ((Input.GetKeyDown("a") || Input.GetKey("a")) && !Input.GetKey("d"))
            dir.x = -1;*/

        rb.velocity = new Vector2(moveSpeed * x, moveSpeed * y);
        rb.drag = idrag;

        if (dir == Vector2.zero && state == 0)   
        {
            //rb.drag = 20;
        }

        if (Vector2.Dot(rb.velocity, dir) <= 0 && rb.velocity.sqrMagnitude > maxSpeed * maxSpeed || rb.velocity.sqrMagnitude <= maxSpeed * maxSpeed)
        {
            rb.velocity.Set(moveSpeed * x, moveSpeed * y);
            rb.drag = idrag;
        }

        if (Input.GetKey(Inputs[4]))
            Dash();

    }

    private void Dash() //WIP
    {
        if (Time.time - 2 > dashTi && rb.velocity.magnitude > 0)
        {
            dashTi = Time.time;
            if (Time.time - dashDuration <= dashTi)
            {
                rb.AddForce(dashAccel * rb.velocity.normalized, ForceMode2D.Impulse);
                if (state == 0)
                    state = 1;
            }
            else if (Time.time - dashDuration > dashTi)
                state = 0;
                
        }
    }

    private float GetDownTime(KeyCode key) //WIP
    {
        float down;
        if (Input.GetKeyDown(key))
            down = Time.time;
        else
            down = 0;
        float pressTime = Time.time - down;
        return pressTime;
    }

}
