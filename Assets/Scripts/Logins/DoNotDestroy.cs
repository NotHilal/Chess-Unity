using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < Object.FindObjectsOfType<DoNotDestroy>().Length; i++)
        {
            if(Object.FindObjectsOfType<DoNotDestroy>()[i]!=this)
            {
                if (Object.FindObjectsOfType<DoNotDestroy>()[i].name == gameObject.name)
                {
                    Destroy(gameObject);
                }
            }
            
        }
        
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            Destroy(gameObject);
        }
    }
}
