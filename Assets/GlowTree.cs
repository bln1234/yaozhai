using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowTree : MonoBehaviour
{
    public float GlowIntensity = 20f;
    public float GlowRange = 100f;
    // Start is called before the first frame update
    void Start()
    {
        LightUpStars();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LightUpStars()
    {
        // ������������������
        foreach (Transform child in transform)
        {
            // ���������������а��������ǡ�
            if (child.name.Contains("����"))
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // �����Է���Ч��
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", Color.yellow * 5f); // ���÷�����ɫ������
                }
                if (child.GetComponent<Light>() == null)
                {
                    Light starLight = child.gameObject.AddComponent<Light>();
                    starLight.color = Color.yellow; // ����Ը�������������ɫ������
                    starLight.intensity = GlowIntensity; // ��������
                    starLight.range = GlowRange; // ���ù�ķ�Χ
                }
            }
        }
    }
}
