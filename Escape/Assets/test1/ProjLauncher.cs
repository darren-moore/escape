using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjLauncher : MonoBehaviour
{
    [Header("Some Properties")]
    //public float maxRange;
    public string name_;
    public float launchVel;
    public float frequency;
    public float spread; //angle in deg
    public float launchOffset;
    public float delTime;
    public GameObject proj;
    public KeyCode input;

    /*[System.Serializable]
    public class ProjList
    {
        public GameObject projectile;
        public KeyCode key;
    }*/

    [Header("Cam Stuff")]
    public Camera cam;


    private Vector3 mpos;
    private Vector3 dir;
    private GameObject parentObj;
    private List<float> ti;

    // Start is called before the first frame update
    void Start()
    {
        if (cam == null)
            cam = GameObject.FindObjectOfType<Camera>();
        parentObj = this.gameObject;

        ti = new List<float>();
        ti.Add(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        FindMousePos();
        dir = (mpos - parentObj.transform.position).normalized;
        if (proj != null && (Input.GetKey(input) || Input.GetKeyDown(input)))
        {
            if (Time.time > ti[0] + (1 / frequency))
            {
                FireProj(launchOffset, spread, launchVel);
                ti[0] = Time.time;
            }
        }
    }

    private void FindMousePos()
    {
        Plane xy = new Plane(Vector3.forward, 0f);
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float d = 0;
        if (xy.Raycast(ray, out d))
        {
            mpos = ray.GetPoint(d);
        }
    }

    private void FireProj(float offset, float spread, float vel)
    {
        float s = Random.Range(-spread* Mathf.PI/180, spread * Mathf.PI / 180);
        Vector2 ddir = new Vector2(dir.x * Mathf.Cos(s) - dir.y * Mathf.Sin(s), dir.x * Mathf.Sin(s) + dir.y * Mathf.Cos(s));
        Vector3 launchPos = transform.position + offset * dir;
        Quaternion rot = Quaternion.Euler(0, 0, Vector3.Angle(Vector3.right, ddir)*(Vector3.Dot(Vector3.up,ddir)/(Mathf.Abs((Vector3.Dot(Vector3.up, ddir))))));
        GameObject ob = Instantiate(proj, launchPos, rot) as GameObject;
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
                //STUFF
            }
        }
    }
}
