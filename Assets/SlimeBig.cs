using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBig : MonoBehaviour
{
    public GameObject fox;
    public GameObject ball;
    public GameObject SmallSlime;
    public GameObject DieEffect; // 死亡爆炸碰撞体
    private List<Material[]> originalMaterials; // 保存每个子物体的材质数组
    private List<Color[]> originalColors; // 保存每个材质的原始颜色
    public Color hitColor = Color.red; // 碰撞后变成的颜色

    public float MaxDistance;
    private float DashCooldown = 3f; // 冲刺攻击冷却时间
    private float Cooldown1 = 4f; // 远程攻击冷却时间
    private float Cooldown2 = 10f; // 召唤冷却时间
    private float last2Time; //上次召唤时间
    private float last1Time; //上次远程攻击时间
    private float lastDashTime; //上次跳跃攻击时间
    public float skillBallSpeed = 20f; // 球的飞行速度
    public float ballLifetime = 2f; // 球体存在的最大时间
    public float HP = 40f;

    public ParticleSystem DieEffect1;
    public ParticleSystem DieEffect2;

    private bool isDash;
    private bool is1;
    private bool is2;
    private bool isBeingAttacked;
    private bool isDie;

    private Animator animator;
    private Collider MonsterCollider; // monster的碰撞器
    private Vector3 move = Vector3.zero; // 移动方向
    private Collider DieEffectCollider; // Die特效的碰撞器

    // Start is called before the first frame update
    void Start()
    {
        DieEffectCollider = DieEffect.GetComponent<Collider>(); // Die的碰撞器
        DieEffectCollider.enabled = false;
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
        isDie = false;
        is2 = false;
        is1 = false;
        isDash = false; // 初始不冲刺攻击
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            StartCoroutine(Die());
        }
        // 怪物不移动
        if (fox.transform.position.z - transform.position.z > MaxDistance
            || transform.position.z - fox.transform.position.z > MaxDistance)
        {
            animator.SetBool("isWalk", false);
            last1Time = Time.time;
            last2Time = Time.time;
            lastDashTime = Time.time;
        }
        else
        {
            // 冲刺攻击
            if (!isDash
                && !is1
                && !is2
                && !isDie
                && Time.time >= lastDashTime + DashCooldown)
            {
                StartCoroutine(Dash());
            }
            if (!is1
               && !isDash
               && !is2
               && !isDie
               && Time.time >= last1Time + Cooldown1)
            {
                StartCoroutine(skill1());
            }
            if(!is2
               && !isDie
               && Time.time >= last2Time + Cooldown2)
            {
                StartCoroutine(skill2());
            }

            // 怪物移动
            if (!isDash && !isDie)
            {
                animator.SetBool("isWalk", true);
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
    private IEnumerator skill1()
    {
        is1 = true;
        animator.SetTrigger("is1");
        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(0.4f);
        // 吐出技能球
        GameObject skillBall = Instantiate(ball, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Quaternion.identity);
        Vector3 direction;
        if (fox.transform.position.z - transform.position.z < 0)
        {
            direction = new Vector3(0f, 0f, -1f);
        }
        else
        {
            direction = new Vector3(0f, 0f, 1f);
        }
        float elapsedTime = 0f;
        // 逐渐改变球体的位置，直到达到最大飞行时间
        while (elapsedTime < ballLifetime)
        {
            skillBall.transform.Translate(direction * skillBallSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // 等待球体存在一段时间后消失
        Destroy(skillBall);

        yield return null;
        is1 = false;
        last1Time = Time.time;
    }
    private IEnumerator skill2()
    {
        is2 = true;
        animator.SetTrigger("is2");
        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(1f);
        Vector3 spawnPosition = transform.position + transform.forward * 2f;  // 正前方2单位的地方
        GameObject smallSlime = Instantiate(SmallSlime, new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z), Quaternion.identity);
        is2 = false;
        last2Time = Time.time;
    }
    private IEnumerator Die()
    {
        isDie = true;
        animator.SetBool("isDie", true);
        yield return new WaitForSeconds(1f);
        DieEffect1.Play();
        DieEffect2.Play();
        yield return new WaitForSeconds(0.4f);
        DieEffectCollider.enabled = true;
        yield return new WaitForSeconds(3.6f);
        gameObject.SetActive(false);
    }
}
