using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SAveUsername : MonoBehaviour
{
    // Start is called before the first frame update
    public static SAveUsername manager;
    public InputField Namefield;
    public string Username;
    public int val = -1;
    public Button LogIn;


    void Start()
    {
        
        if (manager == null)
        {
            manager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Login"))
        {
            Namefield = GameObject.Find("InputFieldUserName").GetComponent<InputField>();
            LogIn = GameObject.Find("Login").GetComponent<Button>();

            LogIn.GetComponent<Button>().onClick.AddListener(SaveUsername);
        }
        
    }
    public void SaveUsername()
    {
        if(SaveUsername2.manager!=null)
        {
            SaveUsername2.manager.Username = "";
            SaveUsername2.manager.val = -1;
        }
        
        Username = Namefield.text;
        val = 0;
    }
    public void DelUsername()
    {
        SaveUsername2.manager.val = -1;
        
    }
}
