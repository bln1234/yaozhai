using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowMushroom : MonoBehaviour
{
    private Light pointLight;
    private Material mushroomMaterial;
    private Material mushroomStemMaterial;
    public float glowDelay;  // 发光的延迟时间（秒）

    void Start()
    {
        // 获取蘑菇的材质
        Renderer renderer = GetComponent<Renderer>();
        mushroomMaterial = renderer.material;

        // 开始协程，延迟发光
        StartCoroutine(GlowAfterDelay(glowDelay));
    }

    void Update()
    {
        
    }

    // 协程：延迟发光
    IEnumerator GlowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 启用发光效果
        mushroomMaterial.EnableKeyword("_EMISSION");
        mushroomMaterial.SetColor("_EmissionColor", Color.white * 0.3f);

        // 创建点光源
        CreatePointLight();
    }

    // 创建点光源
    void CreatePointLight()
    {
        // 创建光源并将其附加到蘑菇上
        GameObject lightGameObject = new GameObject("MushroomLight");
        pointLight = lightGameObject.AddComponent<Light>();
        pointLight.type = LightType.Point;

        // 设置点光源的位置为蘑菇中心
        lightGameObject.transform.position = transform.position;

        // 设置点光源的颜色、强度和范围
        pointLight.color = Color.white;
        pointLight.intensity = 5f;  // 调整光的强度
        pointLight.range = 100f;      // 调整光的范围

        pointLight.shadows = LightShadows.Soft; // 添加一个软阴影来增强散射效果
    }
}
