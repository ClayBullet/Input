using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [HideInInspector] public InputManager managerInput;
   
    public static GameManager managerGame;


    private void Awake()
    {
        if (managerGame == null)
        {
            managerGame = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
}

