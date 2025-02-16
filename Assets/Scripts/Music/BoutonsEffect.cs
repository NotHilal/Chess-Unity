using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoutonsEffect : MonoBehaviour
{
    private bool IsPlaying = true;
    public GameObject Play;
    public GameObject NotPlay;
    public GameObject Play2;
    public GameObject NotPlay2;
    public GameObject Play3;
    public GameObject NotPlay3;
    public GameObject Play4;
    public GameObject NotPlay4;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsPlaying)
        {
            Play.SetActive(false);
            NotPlay.SetActive(true);
            Play2.SetActive(false);
            NotPlay2.SetActive(true);
            Play3.SetActive(false);
            NotPlay3.SetActive(true);
            Play4.SetActive(false);
            NotPlay4.SetActive(true);
        }
        else
        {
            NotPlay.SetActive(false);
            Play.SetActive(true);
            NotPlay2.SetActive(false);
            Play2.SetActive(true);
            NotPlay3.SetActive(false);
            Play3.SetActive(true);
            NotPlay4.SetActive(false);
            Play4.SetActive(true);
        }
        
    }
    public void Swap()
    {
        IsPlaying = !IsPlaying;
    }
    
}
