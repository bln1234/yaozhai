using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public GameObject fox;
    private Collider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (fox.transform.position.y > transform.position.y)
        {
            collider.enabled = true;
        }
    }
}
