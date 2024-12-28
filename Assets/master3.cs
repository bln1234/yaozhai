using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class master3 : MonoBehaviour
{
    public GameObject Plane1;
    public GameObject Plane2;
    public GameObject Bird;
    public GameObject fox;
    private GameObject bird1;
    private GameObject bird2;
    private GameObject bird3;
    private bool start;
    private bool start2;
    // Start is called before the first frame update
    void Start()
    {
        Plane1.SetActive(false);
        Plane2.SetActive(false);
        start = false;
        bird1 = Instantiate(Bird, new Vector3(Bird.transform.position.x, Bird.transform.position.y, Bird.transform.position.z), Quaternion.identity);
        bird2 = Instantiate(Bird, new Vector3(Bird.transform.position.x, Bird.transform.position.y, Bird.transform.position.z), Quaternion.identity);
        bird3 = Instantiate(Bird, new Vector3(Bird.transform.position.x, Bird.transform.position.y, Bird.transform.position.z), Quaternion.identity);
        start2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!start)
        {
            if (Input.anyKey)
            {
                bird1.transform.position = new Vector3(1.15f, 7.02f, -16.49f);
                start = true;
            }
        }
        if(fox.transform.position.y > 7f)
        {
            if(!start2)
            {
                bird2.transform.position = new Vector3(1.15f, 16f, -16.49f);
                bird3.transform.position = new Vector3(1.15f, 16f, -29.49f);
                start2 = true;
            }
            
        }
        if(!bird1.activeSelf)
        {
            Plane1.SetActive(true);
        }
        if (!bird2.activeSelf && !bird3.activeSelf)
        {
            Plane2.SetActive(true);
        }
    }
}
