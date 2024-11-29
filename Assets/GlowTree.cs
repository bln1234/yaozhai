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
        // 遍历树的所有子物体
        foreach (Transform child in transform)
        {
            // 如果子物体的名字中包含“星星”
            if (child.name.Contains("星星"))
            {
                Renderer renderer = child.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // 启用自发光效果
                    renderer.material.EnableKeyword("_EMISSION");
                    renderer.material.SetColor("_EmissionColor", Color.yellow * 5f); // 设置发光颜色和亮度
                }
                if (child.GetComponent<Light>() == null)
                {
                    Light starLight = child.gameObject.AddComponent<Light>();
                    starLight.color = Color.yellow; // 你可以根据需求设置颜色和亮度
                    starLight.intensity = GlowIntensity; // 设置亮度
                    starLight.range = GlowRange; // 设置光的范围
                }
            }
        }
    }
}
