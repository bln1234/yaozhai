using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNormal : MonoBehaviour
{
    public GameObject fox;
    public Material[] materials; // 怪物身上所有材质
    public Color hitColor = Color.red; // 碰撞后变成的颜色

    public float MaxDistance;
    private float JumpCooldown = 2f; // 跳跃攻击冷却时间
    private float lastJumpTime; //上次跳跃攻击时间
    private float VerticalV; // 垂直方向速度
    private float VerticalA; // 垂直方向加速度
    private float HorizontalV; // 水平方向速度
    private float HorizontalA; // 水平方向加速度

    private bool isJump;

    private Animator animator;
    private Collider MonsterCollider; // monster的碰撞器
    private Color[] originalColor; // 用于保存原本的颜色
    private Vector3 move = Vector3.zero; // 移动方向

    // Start is called before the first frame update
    void Start()
    {
        isJump = false; // 初始不跳跃攻击
        animator = GetComponent<Animator>();
        originalColor = new Color[materials.Length]; // 原始颜色
        for ( int i = 0; i < materials.Length;  i++ )
        {
            originalColor[i] = materials[i].color;
        }
        MonsterCollider = transform.GetComponent<Collider>(); // 怪物的碰撞器
        MonsterCollider.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        // 怪物不移动
        if (fox.transform.position.z - transform.position.z > MaxDistance
            || transform.position.z - fox.transform.position.z > MaxDistance)
        {
            animator.SetBool("isWalk", false);
        }
        else
        {
            // 跳跃攻击
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
                transform.position += move * Time.deltaTime; // 更新位置
                lastJumpTime = Time.time;
                isJump = false;
            }
            // 怪物移动
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
            || collision.name.Contains("剑")
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
            yield return new WaitForSeconds(0.05f); // 等待一小段时间
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
