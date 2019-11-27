using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthControlScript : MonoBehaviour {

    public GameObject Heart1, Heart2, Heart3, Heart4, Heart5, gameOver;
    public static int health;

    // Start is called before the first frame update
    public void Start()
    {
        health = 5;
        Heart1.gameObject.SetActive(true);
        Heart2.gameObject.SetActive(true);
        Heart3.gameObject.SetActive(true);
        Heart4.gameObject.SetActive(true);
        Heart5.gameObject.SetActive(true);
        gameOver.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 5)
            health = 5;

        switch (health) {


            case 5:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(true);
                Heart3.gameObject.SetActive(true);
                Heart4.gameObject.SetActive(true);
                Heart5.gameObject.SetActive(true);
                break;
            case 4:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(true);
                Heart3.gameObject.SetActive(true);
                Heart4.gameObject.SetActive(true);
                Heart5.gameObject.SetActive(false);
                break;
            case 3:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(true);
                Heart3.gameObject.SetActive(true);
                Heart4.gameObject.SetActive(false);
                Heart5.gameObject.SetActive(false);
                break;
            case 2:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(true);
                Heart3.gameObject.SetActive(false);
                Heart4.gameObject.SetActive(false);
                Heart5.gameObject.SetActive(false);
                break;
            case 1:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(false);
                Heart3.gameObject.SetActive(false);
                Heart4.gameObject.SetActive(false);
                Heart5.gameObject.SetActive(false);
                break;
            case 0:
                Heart1.gameObject.SetActive(false);
                Heart2.gameObject.SetActive(false);
                Heart3.gameObject.SetActive(false);
                Heart4.gameObject.SetActive(false);
                Heart5.gameObject.SetActive(false);
                gameOver.gameObject.SetActive(true);
                Time.timeScale = 0f;
                break;
        }
    }
    
    public void Restart() {
        health = 5;
        Heart1.gameObject.SetActive(true);
        Heart2.gameObject.SetActive(true);
        Heart3.gameObject.SetActive(true);
        Heart4.gameObject.SetActive(true);
        Heart5.gameObject.SetActive(true);
        gameOver.gameObject.SetActive(false);
    }
}
