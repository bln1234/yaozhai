using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class master : MonoBehaviour
{
    public GameObject fox;
    public GameObject AirWall2;
    public GameObject AirWall3;
    public GameObject AirWall4;
    public GameObject slime1;
    public GameObject slime2;
    public GameObject slime3;
    public FoxMove foxMoveScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(fox.transform.position.z > AirWall2.transform.position.z)
        {
            AirWall2.SetActive(true);
        }
        else
        {
            if (!slime1.activeSelf)
            {
                foxMoveScript.isSide = false;
                AirWall2.SetActive(false);
            }
        }
        if (fox.transform.position.z > AirWall3.transform.position.z)
        {
            AirWall3.SetActive(true);
        }
        else
        {
            if (!slime2.activeSelf)
            {
                foxMoveScript.isSide = false;
                AirWall3.SetActive(false);
            }
        }
        if (fox.transform.position.z > AirWall4.transform.position.z)
        {
            AirWall4.SetActive(true);
        }
        else
        {
            if (!slime3.activeSelf)
            {
                foxMoveScript.isSide = false;
                AirWall4.SetActive(false);
            }
        }
    }
    public void Load()
    {
        SceneManager.LoadScene("second_part", LoadSceneMode.Single);
    }
}
