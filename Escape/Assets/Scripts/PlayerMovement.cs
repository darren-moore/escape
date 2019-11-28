using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 30f;
    private Rigidbody2D rb; 

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveDir.x != 0 || moveDir.y != 0) {
            rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);
        } else {
            rb.MovePosition(rb.position); 
        }
    }
}
