using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Levels
{
    public string first;
    public string second;
    public string third;

    // 获取键值
    public string GetKey(string level)
    {
        return GetType().GetField(level)?.GetValue(this)?.ToString();
    }
    // 检查是否包含某键
    public bool ContainsKey(string level)
    {
        return GetType().GetField(level) != null;
    }
}

public class CheckPointSetting : MonoBehaviour
{
    public string configFileName = "CheckPoint.json"; // JSON 文件名
    public string Point; // 关卡名称
    private Levels Level; // JSON 数据结构
    private string jsonFilePath; // JSON 文件路径
    private Color originalColor; // 原始颜色
    private Color disabledColor = Color.gray; // 禁用时的颜色
    private Button button; // 自动获取的按钮组件

    void Start()
    {
        // 自动获取挂载的按钮组件
        button = GetComponent<Button>();
        // 保存原始颜色
        originalColor = button.colors.normalColor;

        // 初始化 JSON 文件路径
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, configFileName);

        // 如果文件存在，读取配置
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            Level = JsonUtility.FromJson<Levels>(jsonData);
            Debug.Log(jsonData);

            // 初始化按钮状态
            if (Level != null && Level.ContainsKey(Point))
            {
                bool isEnabled = Level.GetKey(Point).Equals("true", StringComparison.OrdinalIgnoreCase);
                SetButtonState(isEnabled);
            }
            else
            {
                Debug.LogWarning($"关卡 '{Point}' 在 JSON 中未找到");
                SetButtonState(false);
            }
        }
        else
        {
            Debug.LogWarning("文件未找到！");
            SetButtonState(false);
        }
    }

    private void SetButtonState(bool isEnabled)
    {
        if (button != null)
        {
            // 更新按钮交互性
            button.interactable = isEnabled;

            // 更新按钮颜色
            var colors = button.colors;
            colors.normalColor = isEnabled ? originalColor : disabledColor;
            button.colors = colors;
        }
    }
}
