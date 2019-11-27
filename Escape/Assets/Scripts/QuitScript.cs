using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitScript : MonoBehaviour
{
    // Start is called before the first frame update
    public void HasQuit()
    {
        Debug.Log("Player has quit the game.");
        Application.Quit();
    }
}
