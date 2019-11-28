using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileTest : MonoBehaviour
{
    [Header("Settings")]
    public GameObject parentObj;
    public int dmg;
    public bool continuousDamage;
    public float hitRate;
    public bool hasProjectileHp = false;
    public float deleteTime;

    [Header("Motion")]
    public AnimationCurve forceCurve;
    public bool isPhysical;
    public bool destroyOnCollide;
    public bool isTracking;
    public bool turnTowards = true;

    private int hp;
    private bool useCurve;
    private Vector3 targetPos;
    private Rigidbody2D rb;
    private List<float> ti;

    // Start is called before the first frame update
    void Start()
    {
        if (tag == null) tag = "Default";
        if (forceCurve == null) useCurve = false;
        if (gameObject.GetComponent<Rigidbody2D>())
            rb = gameObject.GetComponent<Rigidbody2D>();
        ti = new List<float>();
        ti.Add(0);
        ti.Add(0);
        ti[1] = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (Time.time > ti[1] + deleteTime)
            Destroy(gameObject);

        if (turnTowards)
        {
            Vector2 dir = rb.velocity;
            float dc = (Vector3.Dot(Vector3.up, dir) / (Mathf.Abs((Vector3.Dot(Vector3.up, dir)))));
            Quaternion rot = Quaternion.Euler(0, 0, Vector3.Angle(Vector3.right, dir) * dc);
            rb.transform.rotation = rot;
        }
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        if (col.gameObject != null)
        {
            GameObject obj = col.gameObject;
            if (col != null && col.gameObject != parentObj && !obj.CompareTag(tag) && col.gameObject.tag != parentObj.tag)
            {
                if (obj.GetComponent<HealthSystem>() != null)
                {
                    HealthSystem hp = obj.GetComponent<HealthSystem>();
                    hp.DoDamage(dmg);
                    if (destroyOnCollide)
                        Destroy(gameObject, 0.1f);
                    //De-attach children with timed destruction
                }

            }
        }
        
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (continuousDamage && ti[0] + 1 / hitRate > Time.time && col.gameObject != null)
        {
            GameObject obj = col.gameObject;
            if (col != null && obj != parentObj && !obj.CompareTag(tag) && col.gameObject.tag != parentObj.tag)
            {
                if (obj.GetComponent<HealthSystem>() != null)
                {
                    HealthSystem hp = obj.GetComponent<HealthSystem>();
                    hp.DoDamage(dmg);
                    ti[0] = Time.time;
                }

            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {

    }

    void OnCollisionStay2D(Collision2D col)
    {

    }

    public void SetParent(GameObject obj)
    {
        parentObj = obj;
    }

}
