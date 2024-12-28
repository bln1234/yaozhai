using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class feather : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Die());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.up * -10f * Time.deltaTime;
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
    }
}
