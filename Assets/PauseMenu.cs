using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public string configFileName = "CheckPoint.json"; // JSON 文件名
    private Levels level;
    private string jsonFilePath; // JSON 文件路径
    public GameObject pauseMenu; // 指向暂停菜单
    public GameObject DieMenu; // 死亡菜单
    public GameObject PassMenu; // 通关菜单
    public GameObject Pass; // 通关光
    public GameObject fox;

    public FoxMove foxMoveScript;
    public int current_hp;
    private bool isPaused = false;

    void Start()
    {
        // 初始化 JSON 文件路径
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, configFileName);
        // 如果文件存在，读取配置
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            level = JsonUtility.FromJson<Levels>(jsonData);
        }
        Time.timeScale = 1f;
        isPaused = false;
        pauseMenu.SetActive(false); // 开始时隐藏暂停菜单
        DieMenu.SetActive(false); // 开始时隐藏死亡菜单
        PassMenu.SetActive(false); // 开始时隐藏通关菜单
    }

    void Update()
    {
        RaycastHit hitInfo;
        Ray ray = new Ray(fox.transform.position, fox.transform.forward); // 从角色位置发射，方向是角色前方

        if (Physics.Raycast(ray, out hitInfo, 3f)) // 如果射线碰到物体
        {
            // 检查射线碰撞到的物体是否是过关
            if (hitInfo.collider.name.Contains("通关"))
            {
                // 显示通关菜单
                PassMenu.SetActive(true);
                Time.timeScale = 0f; // 暂停游戏
                Debug.Log("通关");
            }
            if (hitInfo.collider.name.Contains("通关1"))
            {
                level.GetType().GetField("second").SetValue(level, "true");
                // 将 keyBindings 对象转换为 JSON 字符串
                string jsonData = JsonUtility.ToJson(level, true);

                // 写入 JSON 文件
                File.WriteAllText(jsonFilePath, jsonData);
            }
            if (hitInfo.collider.name.Contains("通关2"))
            {
                level.GetType().GetField("third").SetValue(level, "true");
                // 将 keyBindings 对象转换为 JSON 字符串
                string jsonData = JsonUtility.ToJson(level, true);

                // 写入 JSON 文件
                File.WriteAllText(jsonFilePath, jsonData);
            }
        }
        current_hp = foxMoveScript.current_hp;
        if (current_hp <= 0)
        {
            Time.timeScale = 0f; // 暂停游戏
            DieMenu.SetActive(true); // 显示死亡菜单
        }
        else
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
        
    public void Restart()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        DieMenu.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏
        // 获取当前场景的名字并重新加载
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    public void ExitGame()
    {
        // 退出游戏
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
}
