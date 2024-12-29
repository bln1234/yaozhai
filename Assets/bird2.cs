using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bird2 : MonoBehaviour
{
    private List<Material[]> originalMaterials; // 保存每个子物体的材质数组
    private List<Color[]> originalColors; // 保存每个材质的原始颜色
    public Color hitColor = Color.red; // 碰撞后变成的颜色

    public float MoveSpeed = 5f; // 移动速度
    
    private bool isFlying = false;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        originalMaterials = new List<Material[]>();
        originalColors = new List<Color[]>();
        // 遍历子物体，获取材质并保存原始颜色
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.materials;
            Material[] instantiatedMaterials = new Material[materials.Length];
            Color[] colors = new Color[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                instantiatedMaterials[i] = Instantiate(materials[i]); // 实例化材质
                colors[i] = materials[i].color; // 保存原始颜色
            }

            renderer.materials = instantiatedMaterials; // 替换为实例化材质
            originalMaterials.Add(instantiatedMaterials); // 保存实例化材质
            originalColors.Add(colors); // 保存颜色
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isFlying)
        {
            transform.position += transform.up * MoveSpeed * Time.deltaTime;
        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.name.Contains("J")
            || collision.name.Contains("剑")
            || collision.name.Contains("I"))
        {
            StartCoroutine(BeAttacked());
        }
    }
    private IEnumerator BeAttacked()
    {
        for (int i = 0; i < originalMaterials.Count; i++)
        {
            Material[] materials = originalMaterials[i];
            foreach (Material mat in materials)
            {
                mat.color = hitColor;
            }
        }
        for (int i = 0; i < 5; i++)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.3f);
            yield return new WaitForSeconds(0.05f); // 等待一小段时间
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        // 恢复所有子物体的材质颜色
        for (int i = 0; i < originalMaterials.Count; i++)
        {
            Material[] materials = originalMaterials[i];
            Color[] colors = originalColors[i];
            for (int j = 0; j < materials.Length; j++)
            {
                materials[j].color = colors[j];
            }
        }
        yield return new WaitForSeconds(0f);
        animator.SetBool("isFlying", true);
        isFlying = true;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

    }
}
