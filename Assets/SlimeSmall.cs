using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSmall : MonoBehaviour
{
    public GameObject fox;
    private List<Material[]> originalMaterials; // 保存每个子物体的材质数组
    private List<Color[]> originalColors; // 保存每个材质的原始颜色
    public Color hitColor = Color.red; // 碰撞后变成的颜色

    public float HP = 15f;
    public float MaxDistance;
    private float DashCooldown = 1.5f; // 冲刺攻击冷却时间
    private float lastDashTime; //上次跳跃攻击时间

    private bool isDash;
    private bool isBeingAttacked;
    private Animator animator;
    private Collider MonsterCollider; // monster的碰撞器
    private Vector3 move = Vector3.zero; // 移动方向

    // Start is called before the first frame update
    void Start()
    {
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
        isBeingAttacked = false;
        isDash = false; // 初始不冲刺攻击
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(HP <= 0)
        {
            StartCoroutine(Die());
        }
        // 怪物不移动
        if (fox.transform.position.z - transform.position.z > MaxDistance
            || transform.position.z - fox.transform.position.z > MaxDistance)
        {
            lastDashTime = Time.time;
            animator.SetBool("isWalk", false);
        }
        else
        {
            // 冲刺攻击
            if (!isDash
                && Time.time >= lastDashTime + DashCooldown)
            {
                StartCoroutine(Dash());
            }
            // 怪物移动
            if (!isDash)
            {
                animator.SetBool("isWalk", true);
                if (Time.timeScale != 0f)
                {
                    if (fox.transform.position.z - transform.position.z > 0)
                    {
                        move.z = 0.01f;
                    }
                    else
                    {
                        move.z = -0.01f;
                    }
                    transform.rotation = Quaternion.LookRotation(move);
                    transform.position += move;
                }
            }

        }
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.name.Contains("J")
            || collision.name.Contains("剑")
            || collision.name.Contains("I"))
        {
            if(!isBeingAttacked)
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
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.1f);
            yield return new WaitForSeconds(0.05f); // 等待一小段时间
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
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
        // 攻击结束，重置标志
        isBeingAttacked = false;
    }
    private IEnumerator Dash()
    {

        isDash = true;
        animator.SetTrigger("isDash"); // 播放冲刺动画

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = fox.transform.position;
        targetPosition.y = startPosition.y;
        float dashDuration = 0.5f; // 冲刺所需时间

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            // 更新位置
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            yield return null;
        }
        yield return null;
        isDash = false;
        lastDashTime = Time.time;

        // 确保冲刺结束后回到行走状态
        animator.SetBool("isWalk", true);
    }
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
