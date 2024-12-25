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

    // ��ȡ��ֵ
    public string GetKey(string level)
    {
        return GetType().GetField(level)?.GetValue(this)?.ToString();
    }
    // ����Ƿ����ĳ��
    public bool ContainsKey(string level)
    {
        return GetType().GetField(level) != null;
    }
}

public class CheckPointSetting : MonoBehaviour
{
    public string configFileName = "CheckPoint.json"; // JSON �ļ���
    public string Point; // �ؿ�����
    private Levels Level; // JSON ���ݽṹ
    private string jsonFilePath; // JSON �ļ�·��
    private Color originalColor; // ԭʼ��ɫ
    private Color disabledColor = Color.gray; // ����ʱ����ɫ
    private Button button; // �Զ���ȡ�İ�ť���

    void Start()
    {
        // �Զ���ȡ���صİ�ť���
        button = GetComponent<Button>();
        // ����ԭʼ��ɫ
        originalColor = button.colors.normalColor;

        // ��ʼ�� JSON �ļ�·��
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, configFileName);

        // ����ļ����ڣ���ȡ����
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            Level = JsonUtility.FromJson<Levels>(jsonData);
            Debug.Log(jsonData);

            // ��ʼ����ť״̬
            if (Level != null && Level.ContainsKey(Point))
            {
                bool isEnabled = Level.GetKey(Point).Equals("true", StringComparison.OrdinalIgnoreCase);
                SetButtonState(isEnabled);
            }
            else
            {
                Debug.LogWarning($"�ؿ� '{Point}' �� JSON ��δ�ҵ�");
                SetButtonState(false);
            }
        }
        else
        {
            Debug.LogWarning("�ļ�δ�ҵ���");
            SetButtonState(false);
        }
    }

    private void SetButtonState(bool isEnabled)
    {
        if (button != null)
        {
            // ���°�ť������
            button.interactable = isEnabled;

            // ���°�ť��ɫ
            var colors = button.colors;
            colors.normalColor = isEnabled ? originalColor : disabledColor;
            button.colors = colors;
        }
    }
}
