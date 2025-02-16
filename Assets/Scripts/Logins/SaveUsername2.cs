using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SaveUsername2 : MonoBehaviour
{
    // Start is called before the first frame update
    public static SaveUsername2 manager;
    public InputField Namefield;
    public string Username;
    public int val = -1;
    public Button SignUp;
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
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Register"))
        {
            Namefield = GameObject.Find("InputFieldUserName2").GetComponent<InputField>();
            SignUp = GameObject.Find("SignUp").GetComponent<Button>();
            SignUp.GetComponent<Button>().onClick.AddListener(SaveUsername);
        }
            

    }
    public void SaveUsername()
    {
        SAveUsername.manager.Username = "";
        SAveUsername.manager.val = -1;
        Username = Namefield.text;
        val = 0;
    }
    public void DelUsername()
    {
        Username = "";
        SAveUsername.manager.val = -1;
        
    }
}