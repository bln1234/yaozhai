using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMaster : MonoBehaviour
{
    public GameObject PassMenu;
    public GameObject InitMenu;
    public GameObject SetMenu;
    // Start is called before the first frame update
    void Start()
    {
        SetMenu.SetActive(false);
        PassMenu.SetActive(false); 
        InitMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }
    public void Load1()
    {
        SceneManager.LoadScene("init_part", LoadSceneMode.Single);
    }
    public void Load2()
    {
        SceneManager.LoadScene("second_part", LoadSceneMode.Single);
    }
    public void Load3()
    {
        SceneManager.LoadScene("third_part", LoadSceneMode.Single);
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
    public void GameSet()
    {
        SetMenu.SetActive(true);
        InitMenu.SetActive(false);
    }
    public void Back()
    {
        SetMenu.SetActive(false);
        PassMenu.SetActive(false);
        InitMenu.SetActive(true);
    }
}
