using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
public class EnemyTest : MonoBehaviour
=======
public class Enemy : MonoBehaviour
>>>>>>> 48318022f5a2a95f9f90bf43a18e38f27a4105b0
{
    public int health = 3; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0) {
            Destroy(gameObject); 
        }
    }

    public void TakeDamage(int damage) {
        health -= damage; 
        Debug.Log("took " + damage + " damage. "); 
    }
}
