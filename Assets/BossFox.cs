using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFox : MonoBehaviour
{
    public GameObject fox;
    public GameObject sword;
    public GameObject iObject;
    private List<Material[]> originalMaterials; // ����ÿ��������Ĳ�������
    private List<Color[]> originalColors; // ����ÿ�����ʵ�ԭʼ��ɫ
    public Color hitColor = Color.red; // ��ײ���ɵ���ɫ
    public ParticleSystem runJEffect; // runJ���⹥����Ч

    public float HP = 15f;
    public float MaxDistance;

    private float RunJCooldown = 7f; // ��̹�����ȴʱ��
    private float U1Cooldown = 3f; // U1��ȴʱ��
    private float U2Cooldown = 13f; // U2��ȴʱ��
    private float ICooldown = 17f; // I��ȴʱ��
    private float BlockCooldown = 5f; // Block��ȴʱ��
    private float lastBlockTime; //�ϴ�Block����ʱ��
    private float lastITime; //�ϴ�I����ʱ��
    private float lastU2Time; //�ϴ�U2����ʱ��
    private float lastU1Time; //�ϴ�U1����ʱ��
    private float lastRunJTime; //�ϴγ�̹���ʱ��

    private bool isBlock;
    private bool isI;
    private bool isU1;
    private bool isU2;
    private bool isSide;
    private bool isRunJ;
    private bool iswalk;
    private bool isBeingAttacked;
    private Animator animator;
    private Collider MonsterCollider; // monster����ײ��
    private Vector3 move = Vector3.zero; // �ƶ�����
    private Collider runJEffectCollider; // runJ��Ч����ײ��
    // Start is called before the first frame update
    void Start()
    {
        runJEffectCollider = runJEffect.GetComponent<Collider>(); // runJ��Ч�ϵ���ײ��
        runJEffectCollider.enabled = false;
        isBlock = false;
        isRunJ = false;
        iswalk = false;
        isU1 = false;
        isU2 = false;
        isI = false;
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
                colors[i] = materials[i].color * 0.3f; // ��ɫ

                instantiatedMaterials[i].color = colors[i]; // �����ʵ���ɫ��Ϊ��ɫ
            }

            renderer.materials = instantiatedMaterials; // �滻Ϊʵ��������
            originalMaterials.Add(instantiatedMaterials); // ����ʵ��������
            originalColors.Add(colors); // ������ɫ
        }
        isBeingAttacked = false;
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
            lastRunJTime = Time.time;
            lastU1Time = Time.time;
            lastU2Time = Time.time;
            lastITime = Time.time;
            lastBlockTime = Time.time;
            animator.SetBool("isWalk", false);
        }
        else
        {
            animator.SetBool("isWalk", true);
            if (Time.timeScale != 0f)
            {
                
                if(!isU1
                    && !isBlock
                    && !isI
                    && !isU2
                    &&!isRunJ
                    && Time.time >= lastU1Time + U1Cooldown)
                {
                    StartCoroutine(U1());
                }
                if(!isU2
                   && !isBlock
                   && !isI
                   &&!isU1
                   && !isRunJ
                   && Time.time >= lastU2Time + U2Cooldown)
                {
                    StartCoroutine(U2());
                }
                if (!isRunJ
                    && !isBlock
                    && !isI
                    && !isU2
                    && !isU1
                    && Time.time >= lastRunJTime + RunJCooldown)
                {
                    StartCoroutine(RunJ());
                }
                if (!isI
                   && !isBlock    
                   && !isU1
                   && !isU2
                   && !isRunJ
                   && Time.time >= lastITime + ICooldown)
                {
                    StartCoroutine(I());
                }
                if (!isBlock
                   && !isI
                   && !isU1
                   && !isU2
                   && !isRunJ
                   && Time.time >= lastBlockTime + BlockCooldown)
                {
                    StartCoroutine(Block());
                }

                if (!isRunJ 
                    && !isI
                    && !isU2
                    && !isU1)
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
        if (isBlock)
        {
            StartCoroutine(Blocked());
        }
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
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
    }
    private IEnumerator RunJ()
    {
        
        isRunJ = true;
        animator.SetTrigger("isRun"); // ���ų�̹�������
        animator.SetBool("isStand", false);
        
        float dashStartTime = Time.time;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + transform.forward * 5f;
        targetPosition.y = startPosition.y;
        yield return new WaitForSeconds(0.4f);
        float dashDuration = 0.3f; // �������ʱ��
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;

            // ����λ��
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / dashDuration);
            yield return null;
        }
        animator.SetTrigger("isRunJ");
        runJEffect.Play();
        yield return new WaitForSeconds(0.1f);
        runJEffectCollider.enabled = true;
        yield return new WaitForSeconds(0.22f);
        runJEffectCollider.enabled = false;
        yield return null;
        yield return new WaitForSeconds(0.4f);
        animator.SetBool("isStand", true);
        isRunJ = false;
        lastRunJTime = Time.time;

    }
    private IEnumerator U1()
    {
        isU1 = true;
        animator.SetTrigger("isU");
        animator.SetBool("isStand", false);
        yield return new WaitForSeconds(0.4f);
        GameObject Sword = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        Sword.transform.rotation = transform.rotation;
        animator.SetBool("isStand", true);
        
        isU1 = false;
        lastU1Time = Time.time;
    }
    private IEnumerator U2()
    {
        isU2 = true;
        animator.SetTrigger("isU");
        animator.SetBool("isStand", false);
        yield return new WaitForSeconds(0.4f);
        GameObject Sword1 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        Sword1.transform.rotation = transform.rotation;
        GameObject Sword2 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        GameObject Sword3 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);

        if (transform.rotation.y == 180)
        {
            Sword2.transform.rotation = transform.rotation;
            Sword2.transform.eulerAngles = new Vector3(transform.eulerAngles.x + 15f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword3.transform.rotation = transform.rotation;
            Sword3.transform.eulerAngles = new Vector3(transform.eulerAngles.x + 30f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            Sword2.transform.rotation = transform.rotation;
            Sword2.transform.eulerAngles = new Vector3(transform.eulerAngles.x - 15f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword3.transform.rotation = transform.rotation;
            Sword3.transform.eulerAngles = new Vector3(transform.eulerAngles.x - 30f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        
                
        animator.SetBool("isStand", true);
        isU2 = false;
        lastU2Time = Time.time;
    }
    private IEnumerator I()
    {
        isI = true;
        animator.SetTrigger("isI");
        animator.SetBool("isStand", false);
        yield return new WaitForSeconds(0.5f);
        GameObject IObject = Instantiate(iObject, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z), Quaternion.identity);
        animator.SetBool("isStand", true);
        isI = false;
        lastITime = Time.time;
    }
    private IEnumerator Block()
    {
        isBlock = true;
        animator.SetTrigger("isBlock");
        animator.SetBool("isStand", false);
        yield return new WaitForSeconds(1f);
        isBlock = false;
        animator.SetBool("isStand", true);
        
        lastBlockTime = Time.time;
    }
    private IEnumerator Blocked()
    {
        animator.SetTrigger("isBlocked");
        animator.SetBool("isStand", false);
        yield return new WaitForSeconds(0.5f);
        GameObject Sword1 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        GameObject Sword2 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        GameObject Sword3 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        GameObject Sword4 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        GameObject Sword5 = Instantiate(sword, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), Quaternion.identity);
        Sword1.transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        if (transform.rotation.y == 180)
        {
            Sword2.transform.eulerAngles = new Vector3(transform.eulerAngles.x + 45f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword3.transform.eulerAngles = new Vector3(transform.eulerAngles.x + 90f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword4.transform.eulerAngles = new Vector3(transform.eulerAngles.x + 135f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword5.transform.eulerAngles = new Vector3(transform.eulerAngles.x + 180f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        else
        {
            Sword2.transform.eulerAngles = new Vector3(transform.eulerAngles.x - 45f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword3.transform.eulerAngles = new Vector3(transform.eulerAngles.x - 90f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword4.transform.eulerAngles = new Vector3(transform.eulerAngles.x - 135f, transform.eulerAngles.y, transform.eulerAngles.z);
            Sword5.transform.eulerAngles = new Vector3(transform.eulerAngles.x - 180f, transform.eulerAngles.y, transform.eulerAngles.z);
        }
        animator.SetBool("isStand", true);
        lastBlockTime = Time.time;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.name.Contains("AirWall"))
        {
            isSide = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        // �����ײ�����Ƿ��ǿ���ǽ
        if (collision.gameObject.name.Contains("AirWall"))
        {
            isSide = false;
        }
    }
}
