using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMaster : MonoBehaviour
{
    public GameObject PassMenu;
    public GameObject InitMenu;
    // Start is called before the first frame update
    void Start()
    {
        PassMenu.SetActive(false); 
        InitMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Load1()
    {
        SceneManager.LoadScene("init_part", LoadSceneMode.Single);
    }
    public void Load2()
    {
        SceneManager.LoadScene("second_part", LoadSceneMode.Single);
    }
    public void ExitGame()
    {
        // ÍË³öÓÎÏ·
        Application.Quit();
    }
    public void ChangeMenu()
    {
        PassMenu.SetActive(true);
        InitMenu.SetActive(false);
    }
}
