using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public GameObject fox;
    public bool air;
    public float MaxDistance;
    public float MaxDistanceY;
    // Start is called before the first frame update
    void Start()
    {
        transform.position=new Vector3(transform.position.x,transform.position.y,fox.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // 使用一个中间变量保存新位置
        Vector3 newPosition = transform.position;
        
        // Z轴逻辑
        if (fox.transform.position.z - transform.position.z > MaxDistance)
        {
            newPosition.z = fox.transform.position.z - MaxDistance;
        }
        else if (transform.position.z - fox.transform.position.z > MaxDistance)
        {
            newPosition.z = fox.transform.position.z + MaxDistance;
        }
        if(air)
        {
            // Y轴逻辑
            if (fox.transform.position.y - transform.position.y > MaxDistanceY)
            {
                newPosition.y += MaxDistanceY;
            }
            else if (transform.position.y - fox.transform.position.y > MaxDistanceY)
            {
                newPosition.y -= MaxDistanceY;
            }
        }
        

        // 更新摄像头位置
        transform.position = newPosition;
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
