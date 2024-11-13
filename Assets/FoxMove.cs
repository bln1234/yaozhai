using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoxMove : MonoBehaviour
{
    public Material[] materials; // ��ɫ�������в���
    private Animator animator;
    
    public GameObject sword; // ���ϵĽ�
    public GameObject flysword; // �ɽ�
    public GameObject IEffect; // I��Ч����
    public ParticleSystem runJEffect; // runJ���⹥����Ч
    public ParticleSystem KJEffect; // kJ���⹥����Ч
    public ParticleSystem IEffect1; // I��Ч
    public ParticleSystem IEffect2;
    public ParticleSystem IEffect3;
    public ParticleSystem IEffect4;
    public ParticleSystem IEffect5;
    public ParticleSystem UEffect1; // U��Ч
    public ParticleSystem UEffect2;
    public ParticleSystem HEffect1; // H��Ч
    public ParticleSystem HEffect2;
    public Image small_mp; // ħ��ֵ(����)
    public Image[] mp; // ħ��ֵ
    public Image[] hp; // ����ֵ
    public Vector3 knockbackDirection; // ���˷���

    private bool isAttacking; // ����״̬
    private bool iskAttacking; // ��Ծ����״̬
    private bool isDashing; // ���״̬
    private bool swordThrown; // ���Ƿ����ӳ�
    private bool isGround; // �ڵ�����
    private bool isJumping; // ��Ծ״̬
    private bool isFalling; // ����״̬
    private bool isIing; // �����ͷż���I
    private bool isUing; // �����ͷż���U
    private bool isKnockedBack; // �Ƿ񱻻���
    private bool isVincible; // �޵�״̬
    private bool isFadingOut; // �𽥱䰵
    private bool isHing; // �����ͷ�H
    private bool isUpHp; // ���ڻ�Ѫ

    private float HCooldown = 0.5f; //��Ѫ��ȴʱ��
    private float dashCooldown = 0.2f; // �����ȴʱ��
    private float AttackCooldown = 0.2f; // ������ȴʱ��
    private float UCooldown = 0.5f; // ����U��ȴʱ��
    private float ICooldown = 0.5f; // ����I��ȴʱ��
    private float JumpdownCooldown = 0.1f; // ����״̬��ȴʱ��
    private float lastHTime; //�ϴλ�Ѫʱ��
    private float lastDashTime; // �ϴγ��ʱ��
    private float lastAttackTime; // �ϴι���ʱ��
    private float lastUTime; //�ϴ�ʹ��U��ʱ��
    private float lastITime; //�ϴ�ʹ��I��ʱ��
    private float lastJumpdownTime; //�ϴ�����״̬ʱ��
    private float jumpSpeed = 15f; // ��Ծ��ʼ�ٶ�
    private float gravity = -30f; // �������ٶ�
    private float VerticalV; // ��ֱ�����ٶ�
    private float HorizontalV; // ˮƽ�����ٶ�
    private float HorizontalA; // ˮƽ������ٶ�
    private float flashTimer = 0f; // �޵�״̬�����л�ʱ��

    private int current_mp; // ���ڵ�ħ��ֵ
    private int current_hp; // ���ڵ�Ѫ��

    private Coroutine hCoroutine; // ���ڴ洢Э�̵�����
    private Coroutine HPCoroutine;
    private Coroutine ImgCoroutine;
    private Coroutine MPCoroutine;
    private Coroutine HShakeCoroutine;

    private Collider runJEffectCollider; // runJ��Ч����ײ��
    private Collider KJEffectCollider; // ��Ծ������ײ��
    private Collider FlyswordCollider; // �ɽ���ײ��
    private Collider ICollider; // I��ײ��

    void Start()
    {
        ICollider = IEffect.GetComponent<Collider>(); // I����ײ��
        ICollider.enabled = false;
        FlyswordCollider = flysword.GetComponent<Collider>(); // �ɽ��ϵ���ײ��
        FlyswordCollider.enabled = false;
        runJEffectCollider = runJEffect.GetComponent<Collider>(); // runJ��Ч�ϵ���ײ��
        runJEffectCollider.enabled = false;
        KJEffectCollider = KJEffect.GetComponent<Collider>(); // runJ��Ч�ϵ���ײ��
        KJEffectCollider.enabled = false;
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].EnableKeyword("_EMISSION"); // ȷ��EmissionЧ������
        }
        animator = GetComponent<Animator>();
        isHing = false; // ��ʼ��Ϊδ��H״̬
        isUpHp = false; // ��ʼ��Ϊδ���л�Ѫ
        isFadingOut = false; // ��ʼ��Ϊ��ɫ
        isVincible = false; // ��ʼ״̬���޵�
        isAttacking = false; // ��ʼ��Ϊ������
        iskAttacking = false; // ��ʼ��Ϊδ��Ծ����
        isDashing = false; // ��ʼ��Ϊ�����
        swordThrown = false; // ��ʼ��Ϊ��δ�ӳ�
        isIing = false; // ��ʼ��Ϊδ����I�����ͷ�
        isUing = false; // ��ʼ��Ϊδ����U�����ͷ�
        isKnockedBack = false; // ��ʼ��Ϊδ������
        flysword.SetActive(false); // �ʼ���������Ϊ���ص�
        lastHTime = -HCooldown; // ��ʼ��Ϊ����ȴʱ��
        lastDashTime = -dashCooldown; // ��ʼ��Ϊ����ȴʱ��
        lastAttackTime = -AttackCooldown; // ��ʼ��Ϊ����ȴʱ��
        lastUTime = -UCooldown; // ��ʼ��Ϊ����ȴʱ��
        lastITime = -ICooldown; // ��ʼ��Ϊ����ȴʱ��
        lastJumpdownTime = -JumpdownCooldown; // ��ʼ��Ϊ����ȴʱ��
        VerticalV = 0f; // ��ʼ���޴�ֱ�ٶ�
        current_mp = 0; // ��ʼ��ħ��ֵΪ0
        current_hp = 7; // ��ʼ��Ѫ��Ϊ7(���ֵ)
        small_mp.fillAmount = 0f; // ��ʼ��ħ��ֵȫΪ��ɫ
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
                Color emissionColor = mat.color * 0f; // ���޵�ʱԭɫ
                mat.SetColor("_EmissionColor", emissionColor);
            }
        }
        // ���湥��
        if (Input.GetKeyDown(KeyCode.J)
            && !isAttacking
            && !iskAttacking
            && !swordThrown
            && isGround
            && !isKnockedBack
            && !iskAttacking
            && Time.time >= lastAttackTime + AttackCooldown)
        {
            StartCoroutine(Attack()); // ��������״̬
        }

        // ���й���
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
            StartCoroutine(AirAttack()); // ���й���
        }
        // �ո���
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
        //H��Ѫ
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
        //����I
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
        //��Ծ
        if (Input.GetKeyDown(KeyCode.K)
            && isGround
            && !isHing
            && !isAttacking
            && !isDashing
            && !isKnockedBack)
        {
            StartJump();
        }
        // ��ɫ�ƶ�
        if (!isAttacking 
            && !isHing
            && !iskAttacking 
            && !isDashing 
            && !isIing
            && !isKnockedBack)
        {
            // ˮƽ��
            float horizontal = Input.GetAxis("Horizontal");
            Vector3 dir = new Vector3(0, 0, horizontal);

            if (dir != Vector3.zero)
            {
                // �泯����
                transform.rotation = Quaternion.LookRotation(dir);
                animator.SetBool("isWalk", true);
                // ����ǰ���ƶ�
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
                // �ڹ���ʱ������ǰ�ƶ�
                transform.Translate(Vector3.forward * 1 * Time.deltaTime);
            }
        }
        else if (iskAttacking)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A))
            {
                // �ڹ���ʱ������ǰ�ƶ�
                transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            }
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true; // ����Ϊ���״̬
        isVincible = true;
        animator.SetTrigger("isRun"); // ���ų�̶���

        // ����߼�
        float dashStartTime = Time.time;
        while (Time.time < dashStartTime + 0.2f)
        {
            VerticalV = 0f; // ���г��ʱ����ֱ�ٶ�ʼ��Ϊ0
            transform.Translate(Vector3.forward * 20 * Time.deltaTime);
            yield return null; // �ȴ���һ֡
        }
        isVincible = false;
        isDashing = false; // ��̽���������״̬
        lastDashTime = Time.time; // �����ϴγ��ʱ��
    }

    private IEnumerator Attack()
    {
        
        AttackMp();
        isAttacking = true; // ����Ϊ����״̬
        animator.SetTrigger("isJ");
        if (runJEffect != null)
        {
            runJEffect.Play();
        }
        // �ȴ�����������ɵ�ʱ��
        yield return new WaitForSeconds(0.4f);
        runJEffectCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        runJEffectCollider.enabled = false;
        isAttacking = false; // ��������������״̬
        lastAttackTime = Time.time; // �����ϴι���ʱ��
    }

    private IEnumerator AirAttack()
    {
        AttackMp();
        iskAttacking = true; // ����Ϊ����״̬
        
        if (KJEffect != null)
        {
            KJEffect.Play();
        }
        animator.SetTrigger("iskJ");
        
        // �ȴ�����������ɵ�ʱ��
        yield return new WaitForSeconds(0.3f);
        KJEffectCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        KJEffectCollider.enabled = false;
        iskAttacking = false;
        lastAttackTime = Time.time; // �����ϴι���ʱ��
    }

    private void ThrowSword()
    {
        isVincible = true;
        UEffect1.Play();
        VerticalV = 0f;
        isUing = true;
        animator.SetTrigger("isU"); // �����ӽ�����
        sword.SetActive(false); // ���ý�����
        swordThrown = true; // ���ý����ӳ�
        flysword.transform.SetParent(null);
        isVincible = false;
        StartCoroutine(SwordFlying());
    }

    private IEnumerator SwordFlying()
    {
        FlyswordCollider.enabled = true;
        UEffect2.Play();
        float throwDuration = 1.0f; // ������ʱ��
        float startTime = Time.time;
        flysword.SetActive(true); // ���÷ɽ���ʾ
        Vector3 direction = flysword.transform.forward;
        while (Time.time < startTime + throwDuration)
        {
            if (!swordThrown)
            {
                break;
            }
            // ÿ֡�ƶ�����λ��
            flysword.transform.position = flysword.transform.position + direction * 15 * Time.deltaTime;
            yield return null;
        }
    }

    private void GotoSword()
    {
        isVincible = true;
        UEffect2.Stop();
        FlyswordCollider.enabled = false;
        // ��ȡ����λ��
        Vector3 swordPosition = flysword.transform.position;

        // �ƶ���ɫ������λ��
        transform.position = new Vector3(swordPosition.x, swordPosition.y - 1.2f, swordPosition.z);
        UEffect1.Play();
        VerticalV = 0; //���������ٶ�Ϊ0
        flysword.SetActive(false); //����ʾ����ɽ�
        flysword.transform.SetParent(transform); //���÷ɽ��ĸ���
        flysword.transform.localPosition = new Vector3(0, 1.2f, 0); //���÷ɽ��ľֲ�λ��
        flysword.transform.rotation = transform.rotation;
        swordThrown = false; // ����Ϊ��δ�ӳ�
        sword.SetActive(true); //��ʾ��
        lastUTime = Time.time; // �����ϴ�ʹ��U��ʱ��
        isUing = false;
        isVincible = false;
    }

    private void I()
    {
        UseMp();
        isVincible = true;
        isIing = true;
        animator.SetTrigger("isI"); // ����I���ܶ���
        VerticalV = -10f;
        lastITime = Time.time;
    }
    private void StartJump()
    {
        isGround = false; // ��ǽ�ɫ���ڵ���
        VerticalV = jumpSpeed;
        animator.SetTrigger("isJump");
    }
    private void CalculateGravity()
    {
        if (isGround || isDashing)
        {
            //VerticalV = 0f; // �ڵ���ͳ��ʱ����ֱ�ٶ�Ϊ0
            if (isIing == true)
            {
                // ���븸��
                IEffect.transform.SetParent(null);
                ICollider.enabled = true;
                IEffect1.Play();
                IEffect2.Play();
                IEffect3.Play();
                IEffect4.Play();
                IEffect5.Play();
                isIing = false; // ���ü���״̬
                // ����Э�̴�����Ч�ָ�����
                StartCoroutine(ResetIEffectParent());
                
                StartCoroutine(Camera.main.GetComponent<CameraMove>().Shake(0.3f, 0.2f)); // ��ʼ��
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
            VerticalV += gravity * Time.deltaTime; // ���´�ֱ�ٶ�
            Vector3 move = new Vector3(0, VerticalV, 0);
            transform.position += move * Time.deltaTime; // ����λ��
            // ���������ߵ㣬����Ϊ����״̬
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
            isGround = true; // ��ɫ�Ӵ�������
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (!isKnockedBack
            && !isVincible
            && !collision.gameObject.name.Contains("Plane"))
        {
            // ִ�л��˲���
            StartCoroutine(Knockback((transform.position - collision.transform.position).z));
            Down_Hp();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // �����ײ�����Ƿ��ǵ���
        if (collision.gameObject.name.Contains("Plane"))
        {
            isGround = false; // ��ɫ�뿪����
        }
    }

    private IEnumerator ResetIEffectParent()
    {
        // �ȴ���Ч������ʱ��
        yield return new WaitForSeconds(0.4f);
        isVincible = false;
        yield return new WaitForSeconds(0.6f);
        // ����Ч���û�ԭ���ĸ���
        ICollider.enabled = false;
        IEffect.transform.SetParent(transform);
        IEffect.transform.localPosition = new Vector3(0, 0, 0); // ����Ϊ��Ը�����λ��

    }

    private void AttackMp() // ������
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

    private void UseMp() // ʹ������
    {
        StartCoroutine(EmptyImage(mp[current_mp - 1], 0f,0.2f));
        current_mp--;
    }

    private IEnumerator Up_Hp() // ��Ѫ
    {
        
        if (current_hp < hp.Length)
        {
            current_hp++;
            ImgCoroutine = StartCoroutine(FillImage(hp[current_hp-1], 1f, 1f));
            yield return new WaitForSeconds(1f);
            lastHTime = Time.time;
        }
    }

    private void Down_Hp() // ��Ѫ
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
    //������
    private IEnumerator Knockback(float deltaZ)
    {
        StartCoroutine(Camera.main.GetComponent<CameraMove>().Shake(0.3f, 0.2f)); // ��ʼ��
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
        // ÿ֡����ɫ����˷����ƶ�
        while (HorizontalV * deltaZ > 0)
        {
            HorizontalV += HorizontalA * Time.deltaTime; // ����ˮƽ�ٶ�
            Vector3 move = new Vector3(0, 0, HorizontalV);
            transform.position += move * Time.deltaTime; // ����λ��
            yield return null;
        }
        isKnockedBack = false;
    }
    //���������Լ�
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
        // ÿ֡����ɫ����˷����ƶ�
        while (HorizontalV * deltaZ > 0)
        {
            HorizontalV += HorizontalA * Time.deltaTime; // ����ˮƽ�ٶ�
            Vector3 move = new Vector3(0, 0, HorizontalV);
            transform.position += move * Time.deltaTime; // ����λ��
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
                    Color emissionColor = mat.color * emissionValue; // �Բ��ʵ���ɫ��������
                    mat.SetColor("_EmissionColor", emissionColor);
                }

                yield return null;
            }
            i++;
        }
        
    }
    private IEnumerator H() // ����H״̬�ͻ�Ѫ
    {
        if (isHing && current_hp < hp.Length)
        {
            HShakeCoroutine = StartCoroutine(Camera.main.GetComponent<CameraMove>().Shake(1f, 0.05f)); // ��ʼ��
            HEffect1.Play();
            HEffect2.Play();
            UseMp();
            yield return HPCoroutine = StartCoroutine(Up_Hp());
        }

        isHing = false;
        animator.SetBool("isHing", false);

    }
}
