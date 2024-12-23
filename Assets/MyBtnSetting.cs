using System;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MyBtnSetting : MonoBehaviour, IPointerExitHandler, IPointerClickHandler
{
    public string configFileName = "keybindings.json"; // JSON �ļ���
    public string action; // Ҫ�޸ĵĶ������ƣ��� Jump, Attack �ȣ�
    private bool _isFixing; // �Ƿ������޸ļ�λ
    private KeyBindings keyBindings; // ��λӳ����
    private string jsonFilePath; // JSON �ļ�·��
    public Text text; // UI Text Ԫ��
    private Color originalColor; // ���ڱ���ԭʼ�ı���ɫ

    void Start()
    {
        // ��ʼ�� JSON �ļ�·��
        jsonFilePath = Path.Combine(Application.streamingAssetsPath, configFileName);

        // ����ļ����ڣ���ȡ����
        if (File.Exists(jsonFilePath))
        {
            string jsonData = File.ReadAllText(jsonFilePath);
            keyBindings = JsonUtility.FromJson<KeyBindings>(jsonData);
        }
        else
        {
            keyBindings = new KeyBindings(); // ��ʼ��Ĭ��ֵ
            Debug.LogWarning("Keybindings file not found! Using default values.");
        }

        // ��ʼ�� Text �ı���ԭʼ��ɫ
        if (keyBindings.ContainsKey(action) && text != null)
        {
            text.text = keyBindings.GetKey(action);

            // ����ԭʼ��ɫ
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
            // ������ⰴ������
            if (Input.anyKeyDown)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        // �޸�ָ�� action ��ֵ
                        if (keyBindings.ContainsKey(action))
                        {
                            keyBindings.GetType().GetField(action).SetValue(keyBindings, keyCode.ToString());

                            // ���� Text �ı�
                            if (text != null)
                            {
                                text.text = keyCode.ToString();
                            }

                            // ���浽 JSON �ļ�
                            SaveKeyBindings();

                            // �˳��޸�ģʽ
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
        // �޸��ı���ɫΪ��ɫ
        if (text != null)
        {
            text.color = Color.gray;
        }

        _isFixing = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ����Ƴ�ʱ�˳��޸�ģʽ�����ָ�ԭʼ��ɫ
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
        // �� keyBindings ����ת��Ϊ JSON �ַ���
        string jsonData = JsonUtility.ToJson(keyBindings, true);

        // д�� JSON �ļ�
        File.WriteAllText(jsonFilePath, jsonData);

        Debug.Log("Keybindings saved successfully.");
    }
}
