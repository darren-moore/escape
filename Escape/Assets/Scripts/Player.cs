using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState {
    walk,
    attack,
    interact
}

public class Player : MonoBehaviour
{

    public float speed = 5f;
    private Rigidbody2D rb; 
    private Animator animator; 

    public float attackSpeed = 0.3f; 
    public float attackDelay;
    public Transform attackPos; 
    public float attackRange = 0.5f; 
    public LayerMask enemyLayer; 
    public int damage = 1; 
    public PlayerState currentState; 
    private Vector2 moveDir; 

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>(); 
        animator = gameObject.GetComponent<Animator>(); 
        currentState = PlayerState.walk; 
        animator.SetFloat("xInput", 0f); 
        animator.SetFloat("yInput", -1f); 
    }
    
    void Update() {
        moveDir = Vector2.zero; 
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.y = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Fire1") && currentState != PlayerState.attack) {
            StartCoroutine(AttackCo()); 
        } else if (currentState == PlayerState.walk) {
            Move(); 
        }
    }

    private IEnumerator AttackCo() {
        animator.SetBool("Attacking", true); 
        currentState = PlayerState.attack; 
        yield return null; 
        animator.SetBool("Attacking", false); 
        yield return new WaitForSeconds(0.25f); 
        currentState = PlayerState.walk; 
    }

    void Attack() {
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemyLayer); 
        for (int i = 0; i < enemiesToDamage.Length; i++) {
            enemiesToDamage[i].GetComponent<EnemyTest>().TakeDamage(damage); 
        }
        attackDelay = attackSpeed; 
    }

    // Update is called once per frame
    void Move()
    {
        if (moveDir != Vector2.zero) {
            rb.MovePosition(rb.position + moveDir.normalized * speed * Time.deltaTime);
            animator.SetFloat("xInput", moveDir.x); 
            animator.SetFloat("yInput", moveDir.y); 
            animator.SetBool("Moving", true); 
        } else {
            animator.SetBool("Moving", false); 
            rb.MovePosition(rb.position); 
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(attackPos.position, attackRange); 
    }
}
