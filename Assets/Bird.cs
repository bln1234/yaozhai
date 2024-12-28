using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public GameObject fox;
    public GameObject feather;
    private List<Material[]> originalMaterials; // 保存每个子物体的材质数组
    private List<Color[]> originalColors; // 保存每个材质的原始颜色
    public Color hitColor = Color.red; // 碰撞后变成的颜色

    public float MoveSpeed = 5f; // 移动速度
    public float MaxDistance = 10f;
    public float AttackCooldown = 1f; 
    public float StopDistanceZ = 1f; // 离目标的水平停止距离
    public float StopDistanceY = 1f; // 离目标的垂直停止距离
    public float StopDistanceZ2 = 3f; // 离目标的水平停止距离
    public float StopDistanceY2 = 3f; // 离目标的垂直停止距离
    private float LastAttack;
    private bool isAttack;
    private bool isBeingAttacked;
    public float HP = 10f;

    private Animator animator;
    private Vector3 move = Vector3.zero; // 移动方向

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
        isAttack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            StartCoroutine(Die());
        }
        // 怪物移动
        if (Mathf.Abs(fox.transform.position.z - transform.position.z) < MaxDistance)
        {
            if (Time.timeScale != 0f)
            {
                if (fox.transform.position.z - transform.position.z > 0)
                {
                    move.x = -0.01f;
                }
                else
                {
                    move.x = 0.01f;
                }
                transform.rotation = Quaternion.LookRotation(move);
            }

            if (Time.time >= LastAttack + AttackCooldown)
            {
                // 冷却时间已结束，怪物靠近
                Vector3 targetPosition = new Vector3(
                    fox.transform.position.x,
                    fox.transform.position.y, // 垂直目标位置
                    fox.transform.position.z  // 水平目标位置
                );

                // 检查水平轴距离是否已达到停止范围
                if (Mathf.Abs(transform.position.z - fox.transform.position.z) <= StopDistanceZ &&
                    Mathf.Abs(transform.position.y - fox.transform.position.y) <= StopDistanceY)
                {
                    if(!isAttack)
                    {
                        StartCoroutine(Attack()); // 开始攻击
                    }
                    
                }
                else
                {
                    // 移动怪物靠近
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                }
            }
            else
            {
                // 冷却时间未结束，怪物远离
                Vector3 awayDirection = (transform.position - fox.transform.position).normalized; // 远离狐狸的方向
                awayDirection.x = 0f;
                awayDirection.y = 1f;
                // 检查是否已超出范围
                if (Mathf.Abs(transform.position.z - fox.transform.position.z) >= StopDistanceZ2 &&
                    Mathf.Abs(transform.position.y - fox.transform.position.y) >= StopDistanceY2)
                {
                    
                }
                else
                {
                    if (!isAttack)
                    {
                        // 移动怪物远离
                        transform.position = transform.position + awayDirection * MoveSpeed * Time.deltaTime;
                    }
                    
                }
            }
        }
        else
        {
            LastAttack = Time.time;
        }

    }

    private IEnumerator Attack()
    {
        isAttack = true;
        animator.SetTrigger("isAttacking");
        yield return new WaitForSeconds(0.5f);
        GameObject feather1 = Instantiate(feather, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        feather1.transform.rotation = transform.rotation;
        feather1.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 45f);
        GameObject feather2 = Instantiate(feather, new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.5f), Quaternion.identity);
        feather2.transform.rotation = transform.rotation;
        feather2.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 45f);
        GameObject feather3 = Instantiate(feather, new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.5f), Quaternion.identity);
        feather3.transform.rotation = transform.rotation;
        feather3.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 45f);
        yield return new WaitForSeconds(0.8f);
        LastAttack = Time.time;
        isAttack = false;
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

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
