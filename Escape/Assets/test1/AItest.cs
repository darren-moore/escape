using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AItest : MonoBehaviour
    { 
    [Header("Properties")]
    public GameObject ply;
    public Rigidbody2D rb;
    public string[] HostileTags;
    public Vector2 eyeOffset;
    public float sightRange;
    public LayerMask layerMask;

    [Header("AI stuff")]
    public float closeEnough;
    public bool isMelee;
    public bool isSlimeLike;
    public GameObject[] projectiles;

    [Header("Stats")]
    public int maxHP;
    public float defense;
    public float speed;
    public float regenRate;


    private int HP;

    private Vector2 ePos;
    private List<GameObject> enemies;
    private int state = 0;  // 0 = wander, 1 = aggro
    private Vector2 pos;
    private Vector2 rpos;
    private bool dead = false;
    private float ti = 0;
    private Vector3 scale;

    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
        if (ply != null) enemies.Add(ply);
        HP = maxHP;
        scale = this.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void FixedUpdate()
    {
        ePos = new Vector2(ply.transform.position.x, ply.transform.position.y);
        Vector2 pos = new Vector2(rb.transform.position.x, rb.transform.position.y);
        rpos = ePos - pos;

        if (HP <= 0)
            dead = true;  //ded even if recieved regen, use revive
        if (!dead)
        {
            if (state == 0 && Time.time + 1 > ti)
            {
                Scan();
                ti = Time.time;
            }
            if (state == 1)
            {
                if (enemies.Contains(ply))
                {
                    Attacc(ply);
                }
            }
        }
        print(state);
    }

    private void Scan()
    {
        for (int i = 0; i < 60; i++)
        {
            Quaternion quat = Quaternion.AngleAxis(3*Mathf.Pow(-1, i) * i, Vector3.forward);
            RaycastHit2D hit = Physics2D.Raycast(pos + eyeOffset,  quat*rpos.normalized, sightRange, layerMask);
            print(Mathf.Rad2Deg*Mathf.Pow(-1, i) * Mathf.Deg2Rad * i);
            GameObject obj;
            if (hit && hit.collider.gameObject != null)
            {
                obj = hit.collider.gameObject;
                if (CheckTag(obj))
                {
                    state = 1;
                    if (!enemies.Contains(obj)) enemies.Add(obj);
                    print("OK");
                    break;
                }
            }
            if (i >= 60) state = 0;
        }

    }

    private void Attacc(GameObject obj)
    {
        if (Mathf.Pow(closeEnough, 2) < Vector2.SqrMagnitude(rpos))
        {
            if (isSlimeLike)
            {
                float rate = Random.Range(3,4);
                float mult = Mathf.Max(Mathf.Sin(Time.time * rate), 0);
                float squishx = 1 - (0.3f * mult) * Mathf.Abs(Vector3.Dot(rpos.normalized, Vector3.right));
                float squishy = 1 - (0.3f * mult) * Mathf.Abs(Vector3.Dot(rpos.normalized, Vector3.up));
                this.transform.localScale = new Vector3(squishx * scale.x, squishy * scale.y, scale.z);
                //transform.rotation = (Quaternion.Euler(new Vector3(0, 0, Vector3.Angle(this.transform.right, rpos))));
                rb.velocity = 2 * mult * speed * rpos.normalized;
            }
            else
            {
                rb.velocity = speed * rpos.normalized;
            }
        }
        
    }

    public void DealDmg(int dmg)
    {
        this.HP -= dmg; // negative2heal
    }

    public void Tensei()
    {
        dead = false;
    }

    public float GetHealth()
    {
        return HP;
    }

    private bool CheckTag(GameObject obj)
    {
        for (int i = 0; i < HostileTags.Length; i++)
        {
            if (obj.CompareTag(HostileTags[i]))
                return true;
        }
        return false;
    }
}
