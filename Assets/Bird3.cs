using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird3 : MonoBehaviour
{
    private List<Material[]> originalMaterials; // 保存每个子物体的材质数组
    private List<Color[]> originalColors; // 保存每个材质的原始颜色
    public Color hitColor = Color.red; // 碰撞后变成的颜色
    public GameObject Bird;
    public GameObject feather;
    public GameObject fox;
    public float feather1y;
    public float feather1z;

    public float MoveSpeed = 5f; // 移动速度
    private bool isFlying = false;
    private Animator animator;
    private float LastAttack;
    private float AttackCooldown = 2f;
    private bool isAttack;
    private bool isBeingAttacked;
    public float HP = 50f;
    int randomInt;

    // Start is called before the first frame update
    void Start()
    {
        LastAttack = -AttackCooldown;
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
        if (HP <= 0)
        {
            StartCoroutine(Die());
        }

        if (!isFlying && !Bird.activeSelf)
        {
            StartCoroutine(fly2Stand());
            transform.position += -transform.up * MoveSpeed * Time.deltaTime;
            StartCoroutine(Camera.main.GetComponent<CameraMove>().Shake(0.3f, 0.2f)); // 开始震动
            LastAttack = Time.time;
        }
        if (!isAttack
            && Time.time >= LastAttack + AttackCooldown)
        {
            randomInt = Random.Range(0, 10);
            if(randomInt < 10)
            {
                StartCoroutine(Flapleft());
            }
            else
            {

            }
        }
        
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.name.Contains("J")
            || collision.name.Contains("剑")
            || collision.name.Contains("I"))
        {
            if (!isBeingAttacked)
            {
                StartCoroutine(BeAttacked());
                if (collision.name.Contains("J")
                    || collision.name.Contains("剑"))
                {
                    HP--;
                }
                else
                {
                    HP = HP - 2;
                }
            }
        }
    }
    private IEnumerator BeAttacked()
    {
        isBeingAttacked = true;
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
        isBeingAttacked = false;
    }
    private IEnumerator fly2Stand()
    {
        yield return new WaitForSeconds(3f);
        isFlying = true;
        yield return new WaitForSeconds(1f);
        animator.SetBool("isStanding", true);
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    private IEnumerator Flapleft()
    {
        AttackCooldown = 2f;
        isAttack = true;
        animator.SetTrigger("isFlapleft");
        yield return new WaitForSeconds(0.5f);
        GameObject feather1 = Instantiate(feather, new Vector3(fox.transform.position.x, transform.position.y + feather1y, transform.position.z + feather1z), Quaternion.identity);
        feather1.transform.rotation = transform.rotation;
        feather1.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 90f, transform.eulerAngles.z + 45f);
        yield return new WaitForSeconds(0.5f);
        isAttack = false;
        LastAttack = Time.time;
    }
}
