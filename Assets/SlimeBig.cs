using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBig : MonoBehaviour
{
    public GameObject fox;
    public GameObject ball;
    public GameObject SmallSlime;
    public GameObject DieEffect; // ������ը��ײ��
    private List<Material[]> originalMaterials; // ����ÿ��������Ĳ�������
    private List<Color[]> originalColors; // ����ÿ�����ʵ�ԭʼ��ɫ
    public Color hitColor = Color.red; // ��ײ���ɵ���ɫ

    public float MaxDistance;
    private float DashCooldown = 3f; // ��̹�����ȴʱ��
    private float Cooldown1 = 4f; // Զ�̹�����ȴʱ��
    private float Cooldown2 = 10f; // �ٻ���ȴʱ��
    private float last2Time; //�ϴ��ٻ�ʱ��
    private float last1Time; //�ϴ�Զ�̹���ʱ��
    private float lastDashTime; //�ϴ���Ծ����ʱ��
    public float skillBallSpeed = 20f; // ��ķ����ٶ�
    public float ballLifetime = 2f; // ������ڵ����ʱ��
    public float HP = 40f;

    public ParticleSystem DieEffect1;
    public ParticleSystem DieEffect2;

    private bool isDash;
    private bool is1;
    private bool is2;
    private bool isBeingAttacked;
    private bool isDie;

    private Animator animator;
    private Collider MonsterCollider; // monster����ײ��
    private Vector3 move = Vector3.zero; // �ƶ�����
    private Collider DieEffectCollider; // Die��Ч����ײ��

    // Start is called before the first frame update
    void Start()
    {
        DieEffectCollider = DieEffect.GetComponent<Collider>(); // Die����ײ��
        DieEffectCollider.enabled = false;
        originalMaterials = new List<Material[]>();
        originalColors = new List<Color[]>();
        // ���������壬��ȡ���ʲ�����ԭʼ��ɫ
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] materials = renderer.materials;
            Material[] instantiatedMaterials = new Material[materials.Length];
            Color[] colors = new Color[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                instantiatedMaterials[i] = Instantiate(materials[i]); // ʵ��������
                colors[i] = materials[i].color; // ����ԭʼ��ɫ
            }

            renderer.materials = instantiatedMaterials; // �滻Ϊʵ��������
            originalMaterials.Add(instantiatedMaterials); // ����ʵ��������
            originalColors.Add(colors); // ������ɫ
        }
        isDie = false;
        is2 = false;
        is1 = false;
        isDash = false; // ��ʼ����̹���
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            StartCoroutine(Die());
        }
        // ���ﲻ�ƶ�
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
            // ��̹���
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

            // �����ƶ�
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
            || collision.name.Contains("��")
            || collision.name.Contains("I"))
        {
            if (!isBeingAttacked)
            {
                StartCoroutine(BeAttacked());
                if (collision.name.Contains("J")
                    || collision.name.Contains("��"))
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
            yield return new WaitForSeconds(0.05f); // �ȴ�һС��ʱ��
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.3f);
            yield return new WaitForSeconds(0.05f);
        }
        // �ָ�����������Ĳ�����ɫ
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
        animator.SetTrigger("isDash"); // ���ų�̶���

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = fox.transform.position;
        targetPosition.y = startPosition.y;
        float dashDuration = 0.5f; // �������ʱ��

        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            // ����λ��
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            yield return null;
        }
        yield return null;
        isDash = false;
        lastDashTime = Time.time;

        // ȷ����̽�����ص�����״̬
        animator.SetBool("isWalk", true);
    }
    private IEnumerator skill1()
    {
        is1 = true;
        animator.SetTrigger("is1");
        animator.SetBool("isWalk", false);
        yield return new WaitForSeconds(0.4f);
        // �³�������
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
        // �𽥸ı������λ�ã�ֱ���ﵽ������ʱ��
        while (elapsedTime < ballLifetime)
        {
            skillBall.transform.Translate(direction * skillBallSpeed * Time.deltaTime, Space.World);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // �ȴ��������һ��ʱ�����ʧ
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
        Vector3 spawnPosition = transform.position + transform.forward * 2f;  // ��ǰ��2��λ�ĵط�
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
