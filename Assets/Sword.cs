using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Die());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * 15f * Time.deltaTime;
    }
    private IEnumerator Die()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider collision)
    {
        if (collision.name.Contains("J")
            || collision.name.Contains("½£")
            || collision.name.Contains("¾µ×Ó")
            || collision.name.Contains("I"))
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180f, transform.eulerAngles.z);
        }
    }
}
