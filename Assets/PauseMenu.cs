using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // 指向暂停菜单
    private bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false); // 开始时隐藏暂停菜单
    }

    void Update()
    {
        // 检测暂停按钮
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
        Time.timeScale = 0f; // 暂停游戏
        isPaused = true;
        pauseMenu.SetActive(true); // 显示暂停菜单
    }

    public void Resume()
    {
        Time.timeScale = 1f; // 恢复游戏
        isPaused = false;
        pauseMenu.SetActive(false); // 隐藏暂停菜单
    }

    public void ExitGame()
    {
        // 退出游戏
        Application.Quit();
    }
}
