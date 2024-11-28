using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirWall : MonoBehaviour
{
    public GameObject fox;
    public float maxDistance = 5f;
    private Renderer WallRenderer;
    private Material WallMaterial;
    // Start is called before the first frame update
    void Start()
    {
        WallRenderer = GetComponent<Renderer>();
        WallMaterial = WallRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        float dis = Mathf.Abs(fox.transform.position.z - transform.position.z);
        Color color = WallMaterial.color;
        float alpha = Mathf.Clamp01(1 - (dis / maxDistance)) * 0.2f;
        color.a = alpha;
        WallMaterial.color = color;
    }
}
