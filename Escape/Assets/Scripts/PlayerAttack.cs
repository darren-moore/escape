using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackSpeed = 0.3f; 
    public float attackDelay;
    public Transform attackPos; 
    public float attackRange; 
    public LayerMask enemyLayer; 
    public int damage; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (attackDelay <= 0) {
            if (Input.GetMouseButtonDown(0)) {
                Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemyLayer); 
                for (int i = 0; i < enemiesToDamage.Length; i++) {
                    enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage); 
                }
                attackDelay = attackSpeed; 
            }
        } else {
            attackDelay -= Time.deltaTime; 
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(attackPos.position, attackRange); 
    }
}
