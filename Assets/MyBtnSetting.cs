using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyBtnSetting : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    public string configFileName = "keybindings.json"; // JSON 文件名
    public string action; // 要修改的动作名称（如 Jump, Attack 等）
    private bool _isFixing; // 是否正在修改键位
    private KeyBindings keyBindings; // 键位映射类
    private string jsonFilePath; // JSON 文件路径
    public Text text; // UI Text 元素
    private Color originalColor; // 用于保存原始文本颜色

    void Start()
    {
        // 初始化 JSON 文件路径
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, configFileName);

        // 如果文件存在，读取配置
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            keyBindings = JsonUtility.FromJson<KeyBindings>(jsonData);
        }
        else
        {
            keyBindings = new KeyBindings(); // 初始化默认值
            Debug.LogWarning("Keybindings file not found! Using default values.");
        }

        // 初始化 Text 文本和原始颜色
        if (keyBindings.ContainsKey(action) && text != null)
        {
            text.text = keyBindings.GetKey(action);

            // 保存原始颜色
            originalColor = text.color;
        }
        else
        {
            Debug.LogWarning($"Action '{action}' not found in keybindings or Text component is missing.");
        }
    }

    void Update()
    {
        if (_isFixing)
        {
            // 检测任意按键输入
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // 修改指定 action 键值
                        if (keyBindings.ContainsKey(action))
                        {
                            keyBindings.GetType().GetField(action).SetValue(keyBindings, keyCode.ToString());

                            // 更新 Text 文本
                            if (text != null)
                            {
                                text.text = keyCode.ToString();
                            }

                            // 保存到 JSON 文件
                            SaveKeyBindings();

                            // 退出修改模式
                            _isFixing = false;

                            Debug.Log($"{action} key updated to: {keyCode}");
                        }
                        else
                        {
                            Debug.LogWarning($"Action '{action}' not found in keybindings.");
                        }

                        break;
                    }
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 修改文本颜色为灰色
        if (text != null)
        {
            text.color = Color.gray;
        }

        _isFixing = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标移出时退出修改模式，并恢复原始颜色
        if (text != null)
        {
            text.color = originalColor;
        }

        if (_isFixing)
        {
            _isFixing = false;
        }
    }

    private void SaveKeyBindings()
    {
        // 将 keyBindings 对象转换为 JSON 字符串
        string jsonData = JsonUtility.ToJson(keyBindings, true);

        // 写入 JSON 文件
        File.WriteAllText(jsonFilePath, jsonData);

        Debug.Log("Keybindings saved successfully.");
    }
}
