using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [Header("HP STUFF")]
    public int MaxHP;
    public float defense;
    public float regenRate;

    [Header("OPTIONAL Health Bar")]
    public GameObject bar;

    public int HP;
    private bool ded;
    private List<float> ti;
    private Vector3 initScale;
    private int state = 0; //1 Fire, 2 Poison
    // Start is called before the first frame update
    void Start()
    {
        HP = MaxHP;
        ti = new List<float>();
            ti.Add(0);
        if (defense < 0) defense = 0;
        if (bar != null)
            initScale = bar.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (HP <= 0)
            ded = true;

        if (!ded && ti[0] + (1 / (regenRate)) < Time.time && HP < MaxHP)
        {
            HP += 1;
            ti[0] = Time.time;
        }

        if (bar != null)
        {
            float mult = (Mathf.Max(0, HP) * Mathf.Pow(MaxHP, -1)) * initScale.x;
            Vector3 scale = new Vector3(mult, initScale.y, initScale.z);
            bar.transform.localScale = scale;
        }
    }

    private void ScaleBar()
    {

    }

    public void SetHealth(int hp)
    {
        HP = hp;
    }

    public int GetHealth()
    {
        return HP;
    }

    public void DoDamage(int dmg)
    {
        HP -= (int)Mathf.Ceil((float)(dmg/(defense + 1)));
    }

    public bool isDead()
    {
        return ded;
    }

    public void Tensei(int hp)
    {
        HP = hp;
        ded = false;
    }
}
