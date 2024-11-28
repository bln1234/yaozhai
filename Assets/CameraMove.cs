using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject fox;
    public float MaxDistance;
    // Start is called before the first frame update
    void Start()
    {
        transform.position=new Vector3(transform.position.x,transform.position.y,fox.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(fox.transform.position.z - transform.position.z > MaxDistance) 
        {
            transform.position =new Vector3(transform.position.x,transform.position.y, fox.transform.position.z - MaxDistance);
        }
        else if(transform.position.z - fox.transform.position.z > MaxDistance)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, fox.transform.position.z + MaxDistance);
        }
    }
    public IEnumerator Shake(float duration, float magnitude)
    {
        
        Vector3 originalPosition = transform.position;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            if (Time.timeScale != 0f)
            {
                // 更新摄像头位置，保持偏移
                transform.position = new Vector3(originalPosition.x + x, originalPosition.y + y, transform.position.z);
                elapsed += Time.deltaTime;
            }
                

            

            yield return null;
        }

        transform.position = new Vector3(originalPosition.x,originalPosition.y,transform.position.z); // 复位摄像头位置
    }
}
