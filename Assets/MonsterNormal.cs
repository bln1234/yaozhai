using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNormal : MonoBehaviour
{
    public GameObject fox;
    public Material[] materials; // �����������в���
    public Color hitColor = Color.red; // ��ײ���ɵ���ɫ

    public float MaxDistance;
    private float JumpCooldown = 2f; // ��Ծ������ȴʱ��
    private float lastJumpTime; //�ϴ���Ծ����ʱ��
    private float VerticalV; // ��ֱ�����ٶ�
    private float VerticalA; // ��ֱ������ٶ�
    private float HorizontalV; // ˮƽ�����ٶ�
    private float HorizontalA; // ˮƽ������ٶ�

    private bool isJump;

    private Animator animator;
    private Collider MonsterCollider; // monster����ײ��
    private Color[] originalColor; // ���ڱ���ԭ������ɫ
    private Vector3 move = Vector3.zero; // �ƶ�����

    // Start is called before the first frame update
    void Start()
    {
        isJump = false; // ��ʼ����Ծ����
        animator = GetComponent<Animator>();
        originalColor = new Color[materials.Length]; // ԭʼ��ɫ
        for ( int i = 0; i < materials.Length;  i++ )
        {
            originalColor[i] = materials[i].color;
        }
        MonsterCollider = transform.GetComponent<Collider>(); // �������ײ��
        MonsterCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        // ���ﲻ�ƶ�
        if (fox.transform.position.z - transform.position.z > MaxDistance
            || transform.position.z - fox.transform.position.z > MaxDistance)
        {
            animator.SetBool("isWalk", false);
        }
        else
        {
            // ��Ծ����
            if (Time.time >= lastJumpTime + JumpCooldown)
            {
                animator.SetTrigger("isJump");
                isJump = true;
                VerticalV = 1f;
                VerticalA = -2f;
                if (fox.transform.position.z - transform.position.z > 0)
                {
                    move.z = 0.01f;
                    HorizontalV = transform.position.z - fox.transform.position.z;
                }
                else
                {
                    move.z = -0.01f;
                    HorizontalV = fox.transform.position.z - transform.position.z;
                }
                transform.rotation = Quaternion.LookRotation(move);
                move = new Vector3(0, VerticalV, HorizontalV);
                transform.position += move * Time.deltaTime; // ����λ��
                lastJumpTime = Time.time;
                isJump = false;
            }
            // �����ƶ�
            if(!isJump)
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
            Debug.Log(2);
            StartCoroutine(BeAttacked());
        }
    }
    private IEnumerator BeAttacked()
    {
        for( int i = 0; i < materials.Length; i++)
        {
            materials[i].color = hitColor;
        }
        Vector3 originalPosition = transform.position;
        for (int i = 0; i < 5; i++) 
        {
            transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z - 0.05f);
            yield return new WaitForSeconds(0.05f); // �ȴ�һС��ʱ��
            transform.position = new Vector3(originalPosition.x, originalPosition.y, originalPosition.z + 0.05f);
            yield return new WaitForSeconds(0.05f);
        }
        transform.position = originalPosition;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColor[i];
        }
    }
}
