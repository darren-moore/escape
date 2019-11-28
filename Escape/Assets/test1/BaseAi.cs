using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAi : MonoBehaviour
{
    /*[Header("BaseAI")]
    public Rigidbody2D rb;
    public string[] HostileTags;
    public float sightRange;
    public LayerMask layerMask;
    public float closeEnough;
    public bool isMelee;
    public HealthSystem hp;

    private Vector2 ePos;
    private GameObject target;*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {

    }

    private bool SimpleScan(Vector2 eyePos, GameObject targ, float range, LayerMask mask)
    {
        Vector2 ppos = new Vector2(targ.transform.position.x, targ.transform.position.y);
        Vector2 dir = (ppos - eyePos).normalized;
        RaycastHit2D hit = Physics2D.Raycast(eyePos, dir, range, mask);
        GameObject obj;
        if (hit && hit.collider.gameObject != null)
        {
            obj = hit.collider.gameObject;
            if (targ == obj)
                return true;
        }
        //else return false;
        return false;
    }

    private void findPotentialTargets(List<GameObject> targets, string tag)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        foreach(GameObject obj in objs)
        {
            targets.Add(obj);
        }
    }

 
}
