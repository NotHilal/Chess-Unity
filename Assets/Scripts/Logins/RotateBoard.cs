using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class RotateBoard : MonoBehaviour
{
    public float speed = 50;
    //public GameObject SpeedButton;
    //public GameObject SlowButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
        //if (speed != 110)
        //{
        //    SpeedButton.SetActive(true);
        //}
        //if (speed != 50)
        //{
        //    SlowButton.SetActive(true);
        //}
    }
    public void AddSpeed()
    {
        if(speed+20<=110)
        {
            speed += 20;
        }
        //if(speed==110)
        //{
        //    SpeedButton.SetActive(false);

        //}
        
    }
    public void RemoveSpeed()
    {
        if (speed - 20 >=50)
        {
            speed -= 20;
        }
        //if (speed == 50)
        //{
        //    SlowButton.SetActive(false);

        //}
        //else
        //{
        //    SlowButton.SetActive(true);
        //}
    }
}
