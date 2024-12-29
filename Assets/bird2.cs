using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bird2 : MonoBehaviour
{
    private List<Material[]> originalMaterials; // ����ÿ��������Ĳ�������
    private List<Color[]> originalColors; // ����ÿ�����ʵ�ԭʼ��ɫ
    public Color hitColor = Color.red; // ��ײ���ɵ���ɫ

    public float MoveSpeed = 5f; // �ƶ��ٶ�
    
    private bool isFlying = false;
    private Animator animator;

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
            || collision.name.Contains("��")
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
        yield return new WaitForSeconds(0f);
        animator.SetBool("isFlying", true);
        isFlying = true;
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);

    }
}
