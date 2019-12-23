using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{
    // Start is called before the first frame update
    //public void Respawned()
    //{
        //GameObject[] objects = GameObject.FindGameObjectsWithTag("Player");
        //GameObject player = objects[0];
        //player.transform.position = new Vector2(0, 0);
        //player.GetComponent<HealthControlScript>().Restart();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().SampleScene);
    //}
    public void Respawned()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);   
    }
    
}
