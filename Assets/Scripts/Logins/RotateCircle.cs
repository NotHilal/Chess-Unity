using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RotateCircle : MonoBehaviour
{
   
    Vector3 a = new Vector3(0, 0, -45);
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate( a* 10 * Time.deltaTime);

    }

}

