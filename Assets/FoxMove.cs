using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoxMove : MonoBehaviour
{
    public Material[] materials; // 角色身上所有材质
    private Animator animator;
    
    public GameObject sword; // 手上的剑
    public GameObject flysword; // 飞剑
    public GameObject IEffect; // I特效父级
    public ParticleSystem runJEffect; // runJ剑光攻击特效
    public ParticleSystem KJEffect; // kJ剑光攻击特效
    public ParticleSystem IEffect1; // I特效
    public ParticleSystem IEffect2;
    public ParticleSystem IEffect3;
    public ParticleSystem IEffect4;
    public ParticleSystem IEffect5;
    public ParticleSystem UEffect1; // U特效
    public ParticleSystem UEffect2;
    public ParticleSystem HEffect1; // H特效
    public ParticleSystem HEffect2;
    public Image small_mp; // 魔法值(中心)
    public Image[] mp; // 魔法值
    public Image[] hp; // 生命值
    public Vector3 knockbackDirection; // 击退方向

    private bool isAttacking; // 攻击状态
    private bool iskAttacking; // 跳跃攻击状态
    private bool isDashing; // 冲刺状态
    private bool swordThrown; // 剑是否已扔出
    private bool isGround; // 在地面上
    private bool isJumping; // 跳跃状态
    private bool isFalling; // 下落状态
    private bool isIing; // 正在释放技能I
    private bool isUing; // 正在释放技能U
    private bool isKnockedBack; // 是否被击退
    private bool isVincible; // 无敌状态
    private bool isFadingOut; // 逐渐变暗
    private bool isHing; // 正在释放H
    private bool isUpHp; // 正在回血

    private float HCooldown = 0.5f; //回血冷却时间
    private float dashCooldown = 0.2f; // 冲刺冷却时间
    private float AttackCooldown = 0.2f; // 攻击冷却时间
    private float UCooldown = 0.5f; // 技能U冷却时间
    private float ICooldown = 0.5f; // 技能I冷却时间
    private float JumpdownCooldown = 0.1f; // 下落状态冷却时间
    private float lastHTime; //上次回血时间
    private float lastDashTime; // 上次冲刺时间
    private float lastAttackTime; // 上次攻击时间
    private float lastUTime; //上次使用U的时间
    private float lastITime; //上次使用I的时间
    private float lastJumpdownTime; //上次下落状态时间
    private float jumpSpeed = 15f; // 跳跃初始速度
    private float gravity = -30f; // 重力加速度
    private float VerticalV; // 垂直方向速度
    private float HorizontalV; // 水平方向速度
    private float HorizontalA; // 水平方向加速度
    private float flashTimer = 0f; // 无敌状态材质切换时间

    private int current_mp; // 现在的魔法值
    private int current_hp; // 现在的血量

    private Coroutine hCoroutine; // 用于存储协程的引用
    private Coroutine HPCoroutine;
    private Coroutine ImgCoroutine;
    private Coroutine MPCoroutine;
    private Coroutine HShakeCoroutine;

    private Collider runJEffectCollider; // runJ特效的碰撞器
    private Collider KJEffectCollider; // 跳跃攻击碰撞器
    private Collider FlyswordCollider; // 飞剑碰撞器
    private Collider ICollider; // I碰撞器

    void Start()
    {
        ICollider = IEffect.GetComponent<Collider>(); // I的碰撞器
        ICollider.enabled = false;
        FlyswordCollider = flysword.GetComponent<Collider>(); // 飞剑上的碰撞器
        FlyswordCollider.enabled = false;
        runJEffectCollider = runJEffect.GetComponent<Collider>(); // runJ特效上的碰撞器
        runJEffectCollider.enabled = false;
        KJEffectCollider = KJEffect.GetComponent<Collider>(); // runJ特效上的碰撞器
        KJEffectCollider.enabled = false;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].EnableKeyword("_EMISSION"); // 确保Emission效果启用
        }
        animator = GetComponent<Animator>();
        isHing = false; // 初始化为未用H状态
        isUpHp = false; // 初始化为未进行回血
        isFadingOut = false; // 初始化为暗色
        isVincible = false; // 初始状态不无敌
        isAttacking = false; // 初始化为不攻击
        iskAttacking = false; // 初始化为未跳跃攻击
        isDashing = false; // 初始化为不冲刺
        swordThrown = false; // 初始化为剑未扔出
        isIing = false; // 初始化为未进行I技能释放
        isUing = false; // 初始化为未进行U技能释放
        isKnockedBack = false; // 初始化为未被击退
        flysword.SetActive(false); // 最开始设置这个剑为隐藏的
        lastHTime = -HCooldown; // 初始化为负冷却时间
        lastDashTime = -dashCooldown; // 初始化为负冷却时间
        lastAttackTime = -AttackCooldown; // 初始化为负冷却时间
        lastUTime = -UCooldown; // 初始化为负冷却时间
        lastITime = -ICooldown; // 初始化为负冷却时间
        lastJumpdownTime = -JumpdownCooldown; // 初始化为负冷却时间
        VerticalV = 0f; // 初始化无垂直速度
        current_mp = 0; // 初始化魔法值为0
        current_hp = 7; // 初始化血量为7(最高值)
        small_mp.fillAmount = 0f; // 初始化魔法值全为无色
        foreach (Image img in mp)
        {
            img.fillAmount = 0f;
        }
    }

    void Update()
    {
        CalculateGravity();
        if (isVincible)
        {
            StartCoroutine(FadeCoroutine());
        }
        else
        {
            foreach (var mat in materials)
            {
                Color emissionColor = mat.color * 0f; // 不无敌时原色
                mat.SetColor("_EmissionColor", emissionColor);
            }
        }
        // 地面攻击
        if (Input.GetKeyDown(KeyCode.J)
            && !isAttacking
            && !iskAttacking
            && !swordThrown
            && isGround
            && !isKnockedBack
            && !iskAttacking
            && Time.time >= lastAttackTime + AttackCooldown)
        {
            StartCoroutine(Attack()); // 启动攻击状态
        }

        // 空中攻击
        if (Input.GetKeyDown(KeyCode.J)
            && !iskAttacking
            && !isIing
            && !isUing
            && !isAttacking
            && !isDashing
            && !swordThrown
            && !isGround
            && !isKnockedBack
            && Time.time >= lastAttackTime + AttackCooldown)
        {
            StartCoroutine(AirAttack()); // 空中攻击
        }
        // 空格冲刺
        if (Input.GetKeyDown(KeyCode.Space)
            && !swordThrown
            && !isAttacking
            && !iskAttacking
            && !isDashing
            && !isIing
            && !isKnockedBack
            && Time.time >= lastDashTime + dashCooldown)
        {
            StartCoroutine(Dash());
        }
        //H回血
        if(Input.GetKey(KeyCode.H)
            && !isAttacking
            && isGround
            && !isIing
            && !isKnockedBack
            && !isDashing)
        {
            
            
            if (!isHing 
                && current_mp != 0
                && Time.time >= lastHTime + HCooldown) 
            {
                animator.SetBool("isHing", true);
                isHing = true;
                hCoroutine = StartCoroutine(H());
            }
            
        }
        else
        {
            if(isHing)
            {
                HEffect1.Stop();
                HEffect2.Stop();
                HEffect1.Clear();
                HEffect2.Clear();
                isHing = false;
                animator.SetBool("isHing", false);
                StopCoroutine(HShakeCoroutine);
                StopCoroutine(ImgCoroutine);
                StopCoroutine(HPCoroutine);
                StopCoroutine(hCoroutine);
                Down_Hp();
            }
        }
        // U
        if (Input.GetKeyDown(KeyCode.U)
            && !isAttacking
            && !isDashing
            && !iskAttacking
            && !isIing
            && !isKnockedBack
            && Time.time >= lastUTime + UCooldown)
        {
            if (!swordThrown && current_mp != 0)
            {
                UseMp();
                ThrowSword();
            }
            else if (swordThrown)
            {
                GotoSword();
            }
        }
        //技能I
        if (Input.GetKeyDown(KeyCode.I)
            && !isDashing
            && current_mp != 0
            && !isAttacking
            && !iskAttacking
            && !isGround
            && !isUing
            && !isKnockedBack
            && Time.time >= lastITime + ICooldown)
        {
            I();
        }
        //跳跃
        if (Input.GetKeyDown(KeyCode.K)
            && isGround
            && !isHing
            && !isAttacking
            && !isDashing
            && !isKnockedBack)
        {
            StartJump();
        }
        // 角色移动
        if (!isAttacking 
            && !isHing
            && !iskAttacking 
            && !isDashing 
            && !isIing
            && !isKnockedBack)
        {
            // 水平轴
            float horizontal = Input.GetAxis("Horizontal");
            Vector3 dir = new Vector3(0, 0, horizontal);

            if (dir != Vector3.zero)
            {
                // 面朝向量
                transform.rotation = Quaternion.LookRotation(dir);
                animator.SetBool("isWalk", true);
                // 朝向前方移动
                transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            }
            else
            {
                animator.SetBool("isWalk", false);
            }
        }
        else if (isAttacking)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                // 在攻击时缓慢向前移动
                transform.Translate(Vector3.forward * 1 * Time.deltaTime);
            }
        }
        else if (iskAttacking)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                // 在攻击时缓慢向前移动
                transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            }
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true; // 设置为冲刺状态
        isVincible = true;
        animator.SetTrigger("isRun"); // 播放冲刺动画

        // 冲刺逻辑
        float dashStartTime = Time.time;
        while (Time.time < dashStartTime + 0.2f)
        {
            VerticalV = 0f; // 空中冲刺时，垂直速度始终为0
            transform.Translate(Vector3.forward * 20 * Time.deltaTime);
            yield return null; // 等待下一帧
        }
        isVincible = false;
        isDashing = false; // 冲刺结束，重置状态
        lastDashTime = Time.time; // 更新上次冲刺时间
    }

    private IEnumerator Attack()
    {
        
        AttackMp();
        isAttacking = true; // 设置为攻击状态
        animator.SetTrigger("isJ");
        if (runJEffect != null)
        {
            runJEffect.Play();
        }
        // 等待攻击动画完成的时间
        yield return new WaitForSeconds(0.4f);
        runJEffectCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        runJEffectCollider.enabled = false;
        isAttacking = false; // 攻击结束，重置状态
        lastAttackTime = Time.time; // 更新上次攻击时间
    }

    private IEnumerator AirAttack()
    {
        AttackMp();
        iskAttacking = true; // 设置为攻击状态
        
        if (KJEffect != null)
        {
            KJEffect.Play();
        }
        animator.SetTrigger("iskJ");
        
        // 等待攻击动画完成的时间
        yield return new WaitForSeconds(0.3f);
        KJEffectCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        KJEffectCollider.enabled = false;
        iskAttacking = false;
        lastAttackTime = Time.time; // 更新上次攻击时间
    }

    private void ThrowSword()
    {
        isVincible = true;
        UEffect1.Play();
        VerticalV = 0f;
        isUing = true;
        animator.SetTrigger("isU"); // 播放扔剑动画
        sword.SetActive(false); // 设置剑隐藏
        swordThrown = true; // 设置剑已扔出
        flysword.transform.SetParent(null);
        isVincible = false;
        StartCoroutine(SwordFlying());
    }

    private IEnumerator SwordFlying()
    {
        FlyswordCollider.enabled = true;
        UEffect2.Play();
        float throwDuration = 1.0f; // 剑飞行时间
        float startTime = Time.time;
        flysword.SetActive(true); // 设置飞剑显示
        Vector3 direction = flysword.transform.forward;
        while (Time.time < startTime + throwDuration)
        {
            if (!swordThrown)
            {
                break;
            }
            // 每帧移动剑的位置
            flysword.transform.position = flysword.transform.position + direction * 15 * Time.deltaTime;
            yield return null;
        }
    }

    private void GotoSword()
    {
        isVincible = true;
        UEffect2.Stop();
        FlyswordCollider.enabled = false;
        // 获取剑的位置
        Vector3 swordPosition = flysword.transform.position;

        // 移动角色到剑的位置
        transform.position = new Vector3(swordPosition.x, swordPosition.y - 1.2f, swordPosition.z);
        UEffect1.Play();
        VerticalV = 0; //设置下落速度为0
        flysword.SetActive(false); //不显示这个飞剑
        flysword.transform.SetParent(transform); //设置飞剑的父级
        flysword.transform.localPosition = new Vector3(0, 1.2f, 0); //设置飞剑的局部位置
        flysword.transform.rotation = transform.rotation;
        swordThrown = false; // 设置为剑未扔出
        sword.SetActive(true); //显示剑
        lastUTime = Time.time; // 更新上次使用U的时间
        isUing = false;
        isVincible = false;
    }

    private void I()
    {
        UseMp();
        isVincible = true;
        isIing = true;
        animator.SetTrigger("isI"); // 播放I技能动画
        VerticalV = -10f;
        lastITime = Time.time;
    }
    private void StartJump()
    {
        isGround = false; // 标记角色不在地面
        VerticalV = jumpSpeed;
        animator.SetTrigger("isJump");
    }
    private void CalculateGravity()
    {
        if (isGround || isDashing)
        {
            //VerticalV = 0f; // 在地面和冲刺时，垂直速度为0
            if (isIing == true)
            {
                // 脱离父级
                IEffect.transform.SetParent(null);
                ICollider.enabled = true;
                IEffect1.Play();
                IEffect2.Play();
                IEffect3.Play();
                IEffect4.Play();
                IEffect5.Play();
                isIing = false; // 重置技能状态
                // 启动协程处理特效恢复父级
                StartCoroutine(ResetIEffectParent());
                
                StartCoroutine(Camera.main.GetComponent<CameraMove>().Shake(0.3f, 0.2f)); // 开始震动
            }

            if (isGround && !isDashing)
            {
                animator.ResetTrigger("isJump");
                animator.ResetTrigger("isHighest");
                animator.ResetTrigger("isI");
                animator.SetBool("isStand", true);
            }
        }
        else
        {
            if (Time.time >= lastJumpdownTime + JumpdownCooldown)
            {
                animator.SetBool("isStand", false);
                lastJumpdownTime = Time.time;
            }
            VerticalV += gravity * Time.deltaTime; // 更新垂直速度
            Vector3 move = new Vector3(0, VerticalV, 0);
            transform.position += move * Time.deltaTime; // 更新位置
            // 如果到了最高点，设置为下落状态
            if (VerticalV < -0.1f && !isGround && !isIing)
            {
                animator.SetTrigger("isHighest");
            }
        }
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(!collision.gameObject.name.Contains("Plane") && (isAttacking || iskAttacking))
        {
            if (collision.enabled)
            {
                collision.enabled = false;
                StartCoroutine(Attackback((transform.position - collision.transform.position).z));
                collision.enabled = true;
            }
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Plane"))
        {
            VerticalV = 0;
            transform.position = new Vector3(transform.position.x, collision.transform.position.y, transform.position.z);
            isGround = true; // 角色接触到地面
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!isKnockedBack
            && !isVincible
            && !collision.gameObject.name.Contains("Plane"))
        {
            // 执行击退操作
            StartCoroutine(Knockback((transform.position - collision.transform.position).z));
            Down_Hp();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 检查碰撞对象是否是地面
        if (collision.gameObject.name.Contains("Plane"))
        {
            isGround = false; // 角色离开地面
        }
    }

    private IEnumerator ResetIEffectParent()
    {
        // 等待特效持续的时间
        yield return new WaitForSeconds(0.4f);
        isVincible = false;
        yield return new WaitForSeconds(0.6f);
        // 将特效设置回原来的父级
        ICollider.enabled = false;
        IEffect.transform.SetParent(transform);
        IEffect.transform.localPosition = new Vector3(0, 0, 0); // 设置为相对父级的位置

    }

    private void AttackMp() // 攒能量
    {
        if (small_mp.fillAmount == 0f)
        {
            StartCoroutine(FillImage(small_mp, 1f, 0.2f));
        }
        else if (small_mp.fillAmount == 1f)
        {
            if (current_mp < mp.Length)
            {
                StartCoroutine(EmptyImage(small_mp, 0f,0.2f));
                StartCoroutine(FillImage(mp[current_mp], 1f, 0.2f));
                current_mp++;
            }
        }
    }

    private void UseMp() // 使用能量
    {
        StartCoroutine(EmptyImage(mp[current_mp - 1], 0f,0.2f));
        current_mp--;
    }

    private IEnumerator Up_Hp() // 回血
    {
        
        if (current_hp < hp.Length)
        {
            current_hp++;
            ImgCoroutine = StartCoroutine(FillImage(hp[current_hp-1], 1f, 1f));
            yield return new WaitForSeconds(1f);
            lastHTime = Time.time;
        }
    }

    private void Down_Hp() // 扣血
    {
        if (current_hp > 0)
        {
            StartCoroutine(EmptyImage(hp[current_hp - 1], 0f,0.2f));
            current_hp--;
        }
    }

    private IEnumerator FillImage(Image img, float targetFill, float fillTime)
    {
        float startFill = img.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < fillTime)
        {
            elapsedTime += Time.deltaTime;
            img.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / fillTime);
            yield return null;
        }

        img.fillAmount = targetFill;
    }
    private IEnumerator EmptyImage(Image img, float targetEmpty, float fillTime)
    {
        float startEmpty = img.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < fillTime)
        {
            elapsedTime += Time.deltaTime;
            img.fillAmount = Mathf.Lerp(startEmpty, targetEmpty, elapsedTime / fillTime);
            yield return null;
        }

        img.fillAmount = targetEmpty;
    }
    //被击退
    private IEnumerator Knockback(float deltaZ)
    {
        StartCoroutine(Camera.main.GetComponent<CameraMove>().Shake(0.3f, 0.2f)); // 开始震动
        isVincible = true;
        StartCoroutine(Collision_Vincible());
        Vector3 direction = new Vector3(0f, 0f, deltaZ);
        transform.rotation = Quaternion.LookRotation(-direction);
        animator.SetTrigger("isBeAttacked");
        isKnockedBack = true;
        float elapsedTime = 0;
        VerticalV = 5f;
        if(deltaZ >0)
        {
            HorizontalA = -20f;
            HorizontalV = 12.5f;
        }
        else
        {
            HorizontalA = 20f;
            HorizontalV = -12.5f;
        }
        // 每帧将角色向击退方向移动
        while (HorizontalV * deltaZ > 0)
        {
            HorizontalV += HorizontalA * Time.deltaTime; // 更新水平速度
            Vector3 move = new Vector3(0, 0, HorizontalV);
            transform.position += move * Time.deltaTime; // 更新位置
            yield return null;
        }
        isKnockedBack = false;
    }
    //攻击击退自己
    private IEnumerator Attackback(float deltaZ)
    {
        Vector3 direction = new Vector3(0f, 0f, deltaZ);
        float elapsedTime = 0;
        if (deltaZ > 0)
        {
            HorizontalA = -15f;
            HorizontalV = 3f;
        }
        else
        {
            HorizontalA = 15f;
            HorizontalV = -3f;
        }
        // 每帧将角色向击退方向移动
        while (HorizontalV * deltaZ > 0)
        {
            HorizontalV += HorizontalA * Time.deltaTime; // 更新水平速度
            Vector3 move = new Vector3(0, 0, HorizontalV);
            transform.position += move * Time.deltaTime; // 更新位置
            yield return null;
        }
    }
    private IEnumerator Collision_Vincible()
    {
        yield return new WaitForSeconds(1f);
        isVincible = false;
    }

    private IEnumerator FadeCoroutine()
    {
        float elapsed;
        float emissionValue;
        int i = 0;
        while(isVincible)
        {
            elapsed = 0f;
            while (elapsed < 0.1f)
            {
                elapsed += Time.deltaTime;
                if(i % 2 == 0)
                {
                    emissionValue = Mathf.Lerp(0f, 1f, elapsed / 0.1f);
                }
                else
                {
                    emissionValue = Mathf.Lerp(1f, 0f, elapsed / 0.1f);
                }
                foreach (var mat in materials)
                {
                    Color emissionColor = mat.color * emissionValue; // 以材质的颜色调整亮度
                    mat.SetColor("_EmissionColor", emissionColor);
                }

                yield return null;
            }
            i++;
        }
        
    }
    private IEnumerator H() // 控制H状态和回血
    {
        if (isHing && current_hp < hp.Length)
        {
            HShakeCoroutine = StartCoroutine(Camera.main.GetComponent<CameraMove>().Shake(1f, 0.05f)); // 开始震动
            HEffect1.Play();
            HEffect2.Play();
            UseMp();
            yield return HPCoroutine = StartCoroutine(Up_Hp());
        }

        isHing = false;
        animator.SetBool("isHing", false);

    }
}
