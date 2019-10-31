using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator; 
    Vector2 movement; 
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal"); 
        movement.y = Input.GetAxisRaw("Vertical"); 
        if (movement.x != 0 || movement.y != 0) {
            animator.SetBool("Moving", true); 
        } else {
            animator.SetBool("Moving", false); 
        }
    }
}
