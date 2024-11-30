using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowMushroom : MonoBehaviour
{
    private Light pointLight;
    private Material mushroomMaterial;
    private Material mushroomStemMaterial;
    public float glowDelay;  // ������ӳ�ʱ�䣨�룩

    void Start()
    {
        // ��ȡĢ���Ĳ���
        Renderer renderer = GetComponent<Renderer>();
        mushroomMaterial = renderer.material;

        // ��ʼЭ�̣��ӳٷ���
        StartCoroutine(GlowAfterDelay(glowDelay));
    }

    void Update()
    {
        
    }

    // Э�̣��ӳٷ���
    IEnumerator GlowAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ���÷���Ч��
        mushroomMaterial.EnableKeyword("_EMISSION");
        mushroomMaterial.SetColor("_EmissionColor", Color.white * 0.3f);

        // �������Դ
        CreatePointLight();
    }

    // �������Դ
    void CreatePointLight()
    {
        // ������Դ�����丽�ӵ�Ģ����
        GameObject lightGameObject = new GameObject("MushroomLight");
        pointLight = lightGameObject.AddComponent<Light>();
        pointLight.type = LightType.Point;

        // ���õ��Դ��λ��ΪĢ������
        lightGameObject.transform.position = transform.position;

        // ���õ��Դ����ɫ��ǿ�Ⱥͷ�Χ
        pointLight.color = Color.white;
        pointLight.intensity = 5f;  // �������ǿ��
        pointLight.range = 100f;      // ������ķ�Χ

        pointLight.shadows = LightShadows.Soft; // ���һ������Ӱ����ǿɢ��Ч��
    }
}
