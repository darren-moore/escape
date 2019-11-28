using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepletionScript : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D col)
    {
        HealthControlScript.health -= 1;
    }
}
