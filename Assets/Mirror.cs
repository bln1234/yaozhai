using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mirror : MonoBehaviour
{
    public Image MirrorFox;
    public GameObject Fox;
    public GameObject FoxBoss;
    public GameObject mirror;

    public float flickerSpeed = 1f;  // ��˸�ٶ�
    private float startAlpha;  // ��ʼ͸����

    private bool isshow = false;
    // Start is called before the first frame update
    void Start()
    {

        MirrorFox.enabled = false;
        startAlpha = MirrorFox.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isshow)
        {
            if(transform.position.z - Fox.transform.position.z <= 4f)
            {
                StartCoroutine(Show());
            }
        }
        if(!FoxBoss.activeSelf)
        {
            mirror.SetActive(false);
            gameObject.SetActive(false);
        }
    }
    private IEnumerator Show()
    {
        isshow = true;
        MirrorFox.enabled = true;
        yield return new WaitForSeconds(2f);
        int flickerCount = 0;
        while (flickerCount < 3)
        {
            float alpha = Mathf.PingPong(Time.time * flickerSpeed, 1f);
            Color color = MirrorFox.color;
            color.a = alpha;  // ����͸����
            MirrorFox.color = color;

            // ��͸���Ƚӽ�0ʱ����ʾһ����˸��ɣ����Ӽ���
            if (alpha <= 0.01f)
            {
                flickerCount++;
            }
            yield return null;
        }
        MirrorFox.enabled = false;
        FoxBoss.transform.position = new Vector3(FoxBoss.transform.position.x, FoxBoss.transform.position.y, transform.position.z - 2f);
        mirror.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 21f);
        mirror.transform.rotation = Quaternion.Euler(0, 90, 0);

    }
}
