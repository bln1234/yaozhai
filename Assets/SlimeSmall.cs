using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSmall : MonoBehaviour
{
    public GameObject fox;
    private List<Material[]> originalMaterials; // ����ÿ��������Ĳ�������
    private List<Color[]> originalColors; // ����ÿ�����ʵ�ԭʼ��ɫ
    public Color hitColor = Color.red; // ��ײ���ɵ���ɫ

    public float HP = 15f;
    public float MaxDistance;
    private float DashCooldown = 1.5f; // ��̹�����ȴʱ��
    private float lastDashTime; //�ϴ���Ծ����ʱ��

    private bool isDash;
    private bool isBeingAttacked;
    private Animator animator;
    private Collider MonsterCollider; // monster����ײ��
    private Vector3 move = Vector3.zero; // �ƶ�����

    // Start is called before the first frame update
    void Start()
    {
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
        isBeingAttacked = false;
        isDash = false; // ��ʼ����̹���
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(HP <= 0)
        {
            StartCoroutine(Die());
        }
        // ���ﲻ�ƶ�
        if (fox.transform.position.z - transform.position.z > MaxDistance
            || transform.position.z - fox.transform.position.z > MaxDistance)
        {
            lastDashTime = Time.time;
            animator.SetBool("isWalk", false);
        }
        else
        {
            // ��̹���
            if (!isDash
                && Time.time >= lastDashTime + DashCooldown)
            {
                StartCoroutine(Dash());
            }
            // �����ƶ�
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
            || collision.name.Contains("��")
            || collision.name.Contains("I"))
        {
            if(!isBeingAttacked)
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
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.1f);
            yield return new WaitForSeconds(0.05f); // �ȴ�һС��ʱ��
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.1f);
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
        // �������������ñ�־
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
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
}
