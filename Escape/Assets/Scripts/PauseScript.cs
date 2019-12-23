using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    bool isPaused = false;
    public GameObject pause;

    private void Start()
    {
        pause.gameObject.SetActive(false);
    }
    public void PauseGame()
    {
            if (isPaused)
        {
            Time.timeScale = 1;
            isPaused = false;
            pause.gameObject.SetActive(false);
        } else
        {
            Time.timeScale = 0;
            isPaused = true;
            pause.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) {
            PauseGame();
        }
    }
}