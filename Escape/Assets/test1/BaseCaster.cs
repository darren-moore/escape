using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCaster : MonoBehaviour
{
    //public Camera cam;

    private Vector3 mouseP;

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
        //FindMousePos();
    }

    /*private void FindMousePos()
    {
        Plane xy = new Plane(Vector3.forward, 0f); 
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        float d = 0;
        if (xy.Raycast(ray, out d))
        {
            mouseP = ray.GetPoint(d);
        }

    }*/
}
