using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNormal : MonoBehaviour
{
    private Collider MonsterCollider; // monster����ײ��
    private Material originalMaterial; // ���ڱ���ԭ���Ĳ���
    private Color originalColor; // ���ڱ���ԭ������ɫ
    public Color hitColor = Color.red; // ��ײ���ɵ���ɫ
    // Start is called before the first frame update
    void Start()
    {
        // ��ȡԭʼ���ʺ���ɫ
        Renderer renderer = GetComponent<Renderer>();
        originalMaterial = renderer.material;
        originalColor = originalMaterial.color;
        MonsterCollider = transform.GetComponent<Collider>(); // �������ײ��
        MonsterCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.name.Contains("J")
            || collision.name.Contains("��")
            || collision.name.Contains("I"))
        {
            Debug.Log(2);
            StartCoroutine(BeAttacked());
        }
    }
    private IEnumerator BeAttacked()
    {
        originalMaterial.color = hitColor;
        Vector3 originalPosition = transform.position;
        for (int i = 0; i < 5; i++) 
        {
            transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z - 0.05f);
            yield return new WaitForSeconds(0.05f); // �ȴ�һС��ʱ��
            transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + 0.05f);
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = originalPosition;
        originalMaterial.color = originalColor;
    }
}
