using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(HealthSystem))]
public class AItest : MonoBehaviour //Sadly this here is a mess
    { 
    [Header("Properties")]
    public Rigidbody2D rb;
    public string[] HostileTags;
    public Vector2 eyeOffset;
    public float sightRange;
    public LayerMask layerMask;

    [Header("AI stuff")]
    public float closeEnough;
    public bool isMelee;
    public bool isSlimeLike;
    public float slimeSize = 0;
    public GameObject[] projectiles;
    public GameObject slime;

    [Header("Stats")]
    public int maxHP;
    public float defense;
    public float speed;
    public float regenRate;
    public HealthSystem hp;

    public int HP;
    private Vector2 ePos;
    private GameObject target;
    private List<Vector2> targets;
    private int state = 0;  // 0 = wander, 1 = aggro
    private Vector2 pos;
    private Vector2 rpos;
    private bool dead = false;
    private List<float> ti;
    private Vector3 scale;
    private float radii;

    // Start is called before the first frame update
    void Start()
    {
        targets = new List<Vector2>();
        if (target == null && GameObject.FindWithTag("Player")) target = GameObject.FindWithTag("Player");
        ti = new List<float>();
        for (int i = 0; i < 5; i++)
            ti.Add(0);

        if (hp == null && gameObject.GetComponent<HealthSystem>())
            hp = gameObject.GetComponent<HealthSystem>();
        scale = this.transform.localScale;
        radii = this.GetComponent<Collider2D>().bounds.extents.magnitude;
        if (radii > 0 && slimeSize <= 0)
            slimeSize = radii;
        ti[3] = Mathf.Sqrt(slimeSize);

        if (closeEnough < 0)
            closeEnough = Mathf.Infinity;
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void FixedUpdate()
    {
        pos = new Vector2(rb.transform.position.x, rb.transform.position.y);
        rpos = ePos - pos;   
        if (hp.isDead())
        {
            if (isSlimeLike)
            {
                Split();
            }
        }
            
        if (!hp.isDead())
        {
            if (state == 0 && Time.time + 1 > ti[0])
            {
                bool inDist = (ePos - pos).sqrMagnitude < Mathf.Pow(0.5f, 2);
                if (ti[1] + 5 < Time.time && (inDist || Time.time <= 6))
                {
                    rb.velocity -= rb.velocity / 2;
                    Wander();
                }
                if (!inDist)
                    Move2Attacc(ePos, 0.5f);
                Scan();
                ti[0] = Time.time;
            }
            if (state == 1)
            {
                if (SimpleScan())
                {
                    if (Mathf.Pow(closeEnough, 2) < Vector2.SqrMagnitude(rpos))
                    {
                        Move2Attacc(ePos, 1);
                    }
                }
                else
                {
                    state = 0;
                }
            } //Add Slime Cannon
            
        }
    }

    private bool SimpleScan()
    {
        Vector2 ppos = new Vector2(target.transform.position.x, target.transform.position.y);
        Vector2 dir = (ppos - pos).normalized;
        RaycastHit2D hit = Physics2D.Raycast(pos + eyeOffset, dir, sightRange, layerMask);
        GameObject obj;
        if (hit && hit.collider.gameObject != null)
        {
            obj = hit.collider.gameObject;
            if (target == obj && state == 1)
            {
                ePos = hit.point;
                return true;
            }
        }
        else return false;
        return false;
    }

    private void Scan() //to be improved
    {
        Vector2 ppos = new Vector2(target.transform.position.x, target.transform.position.y);
        Vector2 dir = (ppos - pos).normalized;
        RaycastHit2D hit = Physics2D.CircleCast(pos + eyeOffset, radii/4, dir, sightRange, layerMask);   //Physics2D.Raycast(pos + eyeOffset,  quat*dir.normalized, sightRange, layerMask);     
        GameObject obj; 
        if (hit && hit.collider.gameObject != null)
        {
            obj = hit.collider.gameObject;
            if (CheckTag(obj))
            {
                state = 1;
                ePos = hit.point;
                target = obj;
            }
        }

    }

    private void Move2Attacc(Vector2 tpos, float m)
    {
        Vector2 dir = (tpos - pos).normalized;
        if (isSlimeLike)
        {
            float rate = Random.Range(4, 5);  
            float mult = m * Mathf.Max(Mathf.Sin(Time.time * rate / slimeSize) / ti[3], 0);
            float squishx = 1 - (0.3f * mult) * Mathf.Abs(Vector3.Dot(dir, Vector3.right));
            float squishy = 1 - (0.3f * mult) * Mathf.Abs(Vector3.Dot(dir, Vector3.up));
            this.transform.localScale = new Vector3(squishx * scale.x, squishy * scale.y, scale.z);
            //transform.rotation = (Quaternion.Euler(new Vector3(0, 0, Vector3.Angle(this.transform.right, rpos))));
            rb.velocity = 2 * mult * speed * dir;
        }
        else
        {
            rb.velocity = m * speed * dir;
        }

    }
    //Some function to stop
    private void Wander()
    {
        float randDist = Random.Range(0, sightRange);
        Vector3 randPos = transform.position + randDist * (Quaternion.Euler(0, 0, Random.Range(0, 360)) * Vector2.right);
        Vector3 dirr = (randPos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(pos, dirr, Random.Range(0.2f, 1) * sightRange);
        if (hit.collider == null)
        {
            ti[1] = Time.time;
            ePos = randPos;
        }
        

    }

    private void Split()
    {
        int n = (int)Mathf.Pow(slimeSize, 2);
        if (slimeSize <= 1) n = 0;
        for (int i = 0; i < n; i++)
        {
            Vector3 randVec = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * Vector3.right;
            GameObject s = Instantiate(slime, this.transform.position + Random.Range(0, radii/8) * randVec, Quaternion.Euler(0, 0, 0)) as GameObject;
            if (s.GetComponent<AItest>() != null)
            {
                AItest obj = s.GetComponent<AItest>();
                obj.SetSlimeSize((slimeSize + Random.Range(-slimeSize / 2, slimeSize / 4)) / 2);
                s.tag = tag;
            }
        }
        Destroy(this.gameObject); //add a delayed destruction with shrink ani
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

    public int GetState()
    {
        return state;
    }

    public Vector2 GetTargetPos()
    {
        return ePos;
    }

    public void SetSlimeSize(float size) 
    {
        this.slimeSize = size;
        this.transform.localScale = size * this.transform.localScale;
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
