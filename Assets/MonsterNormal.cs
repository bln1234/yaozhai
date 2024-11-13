using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNormal : MonoBehaviour
{
    private Collider MonsterCollider; // monster的碰撞器
    private Material originalMaterial; // 用于保存原本的材质
    private Color originalColor; // 用于保存原本的颜色
    public Color hitColor = Color.red; // 碰撞后变成的颜色
    // Start is called before the first frame update
    void Start()
    {
        // 获取原始材质和颜色
        Renderer renderer = GetComponent<Renderer>();
        originalMaterial = renderer.material;
        originalColor = originalMaterial.color;
        MonsterCollider = transform.GetComponent<Collider>(); // 怪物的碰撞器
        MonsterCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.name.Contains("J")
            || collision.name.Contains("剑")
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
            yield return new WaitForSeconds(0.05f); // 等待一小段时间
            transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + 0.05f);
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = originalPosition;
        originalMaterial.color = originalColor;
    }
}
