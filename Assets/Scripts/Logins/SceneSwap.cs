using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwap : MonoBehaviour
{
    public void GoToMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
    public void GoToLogin()
    {
        SceneManager.LoadSceneAsync("Login");
    }
    public void GoToLeaderBoard()
    {
        SceneManager.LoadSceneAsync("");
    }
    public void GoToRegister()
    {
        SceneManager.LoadSceneAsync("Register");
    }
    public void GoToForgotPass()
    {
        SceneManager.LoadSceneAsync("ForgotPass");
    }

}
