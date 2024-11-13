using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // ָ����ͣ�˵�
    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false); // ��ʼʱ������ͣ�˵�
    }

    void Update()
    {
        // �����ͣ��ť
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f; // ��ͣ��Ϸ
        isPaused = true;
        pauseMenu.SetActive(true); // ��ʾ��ͣ�˵�
    }

    public void Resume()
    {
        Time.timeScale = 1f; // �ָ���Ϸ
        isPaused = false;
        pauseMenu.SetActive(false); // ������ͣ�˵�
    }

    public void ExitGame()
    {
        // �˳���Ϸ
        Application.Quit();
    }
}
