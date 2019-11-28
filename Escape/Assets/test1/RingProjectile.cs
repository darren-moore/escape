using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingProjectile : MonoBehaviour
{
    [Header("Stuff")]
    public Camera cam;
    public KeyCode holdButton;
    public int mode = 0;

    [Header("Projectile")]
    public GameObject prefab;
    public float radius;
    public float maxCount;
    public float forceConst;
    public float spawnPeriod;
    public float delTime;
    public int distPower;

    private List<GameObject> projs;
    private float tVel;
    private float cForce;
    private Vector3 pPos;
    private Vector3 mPos;

    private float ti;
    private float ti2;
    private int state = 0; // 0 for generating, 1 for hold, 2 for release
    
    void Start()
    {
        projs = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        pPos = gameObject.transform.position; //FIX

        if (Time.time > ti + spawnPeriod && projs.Count <= maxCount)
        {
            SummonOrb();
            ti = Time.time;
        }

        foreach (GameObject el in projs) {
            if (el == null)
            {
                projs.Remove(el);
                break;
            }
            else
            {
                if (el != null && el.GetComponent<Rigidbody2D>() != null && el.GetComponent<ProjectileTest>() != null)
                {
                    ProjectileTest p = el.GetComponent<ProjectileTest>();
                    Rigidbody2D rb = el.GetComponent<Rigidbody2D>();
                    if (p.parentObj != null && p.parentObj == gameObject)
                    {
                        Vector3 rPos = (el.transform.position - pPos);
                        Vector2 perp = new Vector2(-rPos.y, rPos.x).normalized;
                        switch(mode)
                        {
                            case 0:
                                SwingProj(perp, rPos, rb);
                                break;
                            case 1:
                                RingV2(projs.IndexOf(el), rb);
                                break;
                        }

                    }

                }


            }

        }

    }

    private void SwingProj(Vector2 perp, Vector3 rPos, Rigidbody2D rb)
    {
        if (Input.GetKey(holdButton))
        {
            rb.AddForce(32 * perp * rb.mass, ForceMode2D.Force);
            rb.AddForce(4 * -rPos.normalized * rb.mass * Mathf.Max(Vector3.SqrMagnitude(rPos), 80));
            ti2 = Time.time;
        }
        else if (Input.GetKeyUp(holdButton) || !Input.GetKey(holdButton) && Time.time < ti2 + 0.5f)
        {
            //rb.AddForce(16 * (mPos - pPos).normalized * rb.mass, ForceMode2D.Force);
        }
        else
        {
            rb.AddForce(4 * perp * rb.mass, ForceMode2D.Force);
            rb.AddForce(4 * -rPos.normalized * rb.mass * Mathf.Max(Vector3.SqrMagnitude(rPos), 2));
            //if (Vector3.SqrMagnitude(rPos) <= 1)
            //rb.AddForce(16 * rPos.normalized * rb.mass * 10);
        }
    }

    private void Ring(int count)
    {
        float ang = (2 * Mathf.PI) / count;
        for (int i = 0; i < count; i++)
        {
            if (projs[i] == null)
                continue;
            float p = i * ang;
            projs[i].transform.position = new Vector3(Mathf.Cos(Time.time + p), Mathf.Sin(Time.time + p), 0);
        }
    }

    private void RingV2(int i, Rigidbody2D rb)
    {
        float ang = (2 * Mathf.PI) / maxCount;
            float p = i * ang;
            rb.transform.position = new Vector3(pPos.x + Mathf.Cos(Time.time + p), pPos.y + Mathf.Sin(Time.time + p), 0);
    }

    private void SummonOrb()
    {
        GameObject ob = Instantiate(prefab, pPos + Vector3.right, Quaternion.Euler(0, 0, 0)) as GameObject;
        ProjectileTest p = ob.GetComponent<ProjectileTest>();
        p.parentObj = gameObject;
        p.deleteTime = delTime;
        projs.Add(ob);
    }

    void OnTriggerStay2D(Collider2D col)
    {

    }

    private void FindMousePos()
    {
        Plane xy = new Plane(Vector3.forward, 0f);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float d = 0;
        if (xy.Raycast(ray, out d))
        {
            mPos = ray.GetPoint(d);
        }

    }
}
