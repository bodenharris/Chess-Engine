using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    BoardScript boardScript;
    
    // Start is called before the first frame update
    void Start()
    {
       BoardScript boardScript = FindObjectOfType<BoardScript>();
       boardScript.boardSetUp();
       
    }

    private void Update()
    {

    }
}
