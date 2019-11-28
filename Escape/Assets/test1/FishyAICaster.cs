using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AItest))]
public class FishyAICaster : MonoBehaviour
{
    [Header("Projectiles")]
    public GameObject projObj;
    public float launchVel;
    public float frequency;
    public float offsetRadius;
    public float spread;
    public float delTime;
    public Vector2 relativeFirePos;

    [Header("More Stuff")]
    public float maxDist;
    public float minDist;


    private float ti = 0;
    private Vector2 rDir;
    private Vector2 pos;
    private AItest ai;
    private HealthSystem hp;
    // Start is called before the first frame update
    void Start()
    {
        ai = gameObject.GetComponent<AItest>();
        hp = gameObject.GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    void FixedUpdate()
    {
        if (ai.GetState() == 1 && Time.time > ti + (1 / frequency) && !hp.isDead())
        {
            pos = new Vector2(transform.position.x, transform.position.y);
            rDir = ai.GetTargetPos() - (pos + relativeFirePos);

            if (rDir.sqrMagnitude <= Mathf.Pow(maxDist, 2) && rDir.sqrMagnitude >= Mathf.Pow(minDist, 2))
                AIFireProj(rDir.normalized, offsetRadius, spread, launchVel);
            ti = Time.time;
        }

    }

    private void AIFireProj(Vector2 dir, float offset, float spread, float vel)
    {
        float s = Random.Range(-spread * Mathf.PI / 180, spread * Mathf.PI / 180);
        Vector2 ddir = new Vector2(dir.x * Mathf.Cos(s) - dir.y * Mathf.Sin(s), dir.x * Mathf.Sin(s) + dir.y * Mathf.Cos(s));
        Vector3 launchPos = pos + offset * dir;
        Quaternion rot = Quaternion.Euler(0, 0, Vector3.Angle(Vector3.right, ddir) * (Vector3.Dot(Vector3.up, ddir) / (Mathf.Abs((Vector3.Dot(Vector3.up, ddir))))));
        GameObject ob = Instantiate(projObj, launchPos, rot) as GameObject;
        if (ob != null)
        {
            Rigidbody2D rp;
            ob.tag = "Projectile"; //tag
            ob.layer = 8;
            if (ob.GetComponent<Rigidbody2D>() != null)
            {
                rp = ob.GetComponent<Rigidbody2D>();
                rp.velocity = vel * ddir;
            }
            if (ob.GetComponent<ProjectileTest>())
            {
                ProjectileTest p = ob.GetComponent<ProjectileTest>();
                p.SetParent(gameObject);
                if (delTime > 0) p.deleteTime = delTime;
            }
        }
    }

}
