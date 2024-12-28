using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public GameObject fox;
    public GameObject feather;
    private List<Material[]> originalMaterials; // ����ÿ��������Ĳ�������
    private List<Color[]> originalColors; // ����ÿ�����ʵ�ԭʼ��ɫ
    public Color hitColor = Color.red; // ��ײ���ɵ���ɫ

    public float MoveSpeed = 5f; // �ƶ��ٶ�
    public float MaxDistance = 10f;
    public float AttackCooldown = 1f; 
    public float StopDistanceZ = 1f; // ��Ŀ���ˮƽֹͣ����
    public float StopDistanceY = 1f; // ��Ŀ��Ĵ�ֱֹͣ����
    public float StopDistanceZ2 = 3f; // ��Ŀ���ˮƽֹͣ����
    public float StopDistanceY2 = 3f; // ��Ŀ��Ĵ�ֱֹͣ����
    private float LastAttack;
    private bool isAttack;
    private bool isBeingAttacked;
    public float HP = 10f;

    private Animator animator;
    private Vector3 move = Vector3.zero; // �ƶ�����

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
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
        isAttack = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (HP <= 0)
        {
            StartCoroutine(Die());
        }
        // �����ƶ�
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
                // ��ȴʱ���ѽ��������￿��
                Vector3 targetPosition = new Vector3(
                    fox.transform.position.x,
                    fox.transform.position.y, // ��ֱĿ��λ��
                    fox.transform.position.z  // ˮƽĿ��λ��
                );

                // ���ˮƽ������Ƿ��Ѵﵽֹͣ��Χ
                if (Mathf.Abs(transform.position.z - fox.transform.position.z) <= StopDistanceZ &&
                    Mathf.Abs(transform.position.y - fox.transform.position.y) <= StopDistanceY)
                {
                    if(!isAttack)
                    {
                        StartCoroutine(Attack()); // ��ʼ����
                    }
                    
                }
                else
                {
                    // �ƶ����￿��
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, MoveSpeed * Time.deltaTime);
                }
            }
            else
            {
                // ��ȴʱ��δ����������Զ��
                Vector3 awayDirection = (transform.position - fox.transform.position).normalized; // Զ�����ķ���
                awayDirection.x = 0f;
                awayDirection.y = 1f;
                // ����Ƿ��ѳ�����Χ
                if (Mathf.Abs(transform.position.z - fox.transform.position.z) >= StopDistanceZ2 &&
                    Mathf.Abs(transform.position.y - fox.transform.position.y) >= StopDistanceY2)
                {
                    
                }
                else
                {
                    if (!isAttack)
                    {
                        // �ƶ�����Զ��
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

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
