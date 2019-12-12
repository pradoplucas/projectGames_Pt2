using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scriptPilula : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.CompareTag("PowerUp")){
            transform.Rotate(28*Time.deltaTime, 40*Time.deltaTime, 10*Time.deltaTime);
        }
    }
}
