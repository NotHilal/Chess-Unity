using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    [Header("UI")]
    public Text msgTxt;
    public InputField EmailInput;
    public InputField UsernameInput;
    public InputField PasswordInput;
    public GameObject TourneLoading;
    public GameObject RowPrefab;
    public Transform RowsParent;

    //Save Username
    
    private void Awake()
    {
        
    }
    void Start()
    {
        TourneLoading.SetActive(false);
    }
    public void RegisterButton()
    {
        if (PasswordInput.text.Length == 0 || UsernameInput.text.Length == 0 || EmailInput.text.Length == 0)
        {
            msgTxt.text = "Please fill all the required fields..";
            msgTxt.color = Color.red;
            Invoke("ResetTxtError", 2.5f);
            return;
        }
        if (PasswordInput.text.Length<6)
        {
            msgTxt.text = "Password too short..";
            msgTxt.color = Color.red;
            Invoke("ResetTxtError", 2.5f);
            return;
        }
        if (UsernameInput.text.Length < 3)
        {
            msgTxt.text = "Username too short..";
            msgTxt.color = Color.red;
            Invoke("ResetTxtError", 2.5f);
            return;
        }
        TourneLoading.SetActive(true);

        var request = new RegisterPlayFabUserRequest
        {
            Email = EmailInput.text,
            Username = UsernameInput.text,
            Password = PasswordInput.text,
            RequireBothUsernameAndEmail = true
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        msgTxt.text = "Account created ! Now loading menu..";
        UpdateDisplayName(UsernameInput.text);
        
        msgTxt.color = Color.green;
        GoToMenu();
    }
    void OnError(PlayFabError error)
    {
        
        msgTxt.text = "An error has occured : "+ error.ErrorMessage;
        msgTxt.color = Color.red;
        TourneLoading.SetActive(false);
        Invoke("ResetTxtError", 4f);
    }
    private void UpdateDisplayName(string displayname)
    {
        
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = displayname };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnError);
    }
    private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log($"You have updated the displayname of the playfab account!");
        
    }
    void GoToMenu()
    {
        
        StartCoroutine(LoadAsynchronously(0));


    }
    IEnumerator LoadAsynchronously(int val)
    {
        string a = "";
        if(val== 0)
        {
            a = "MainMenu";
        }else
        {
            a = "Login";
        }
        AsyncOperation ope = SceneManager.LoadSceneAsync(a);
        //LoadingBar.SetActive(true);
        while (!ope.isDone)
        {
            float progress = Mathf.Clamp01(ope.progress/.9f);
            //slider.value = progress;
            //ProgressTxt.text = progress * 100 + "%";

            yield return null;
        }
    }
    void GoToLogin()
    {
        StartCoroutine(LoadAsynchronously(1));
    }

    public void LoginButton()
    {
        TourneLoading.SetActive(true);
        
        var request = new LoginWithPlayFabRequest
        {
            
            Username = UsernameInput.text,
            Password = PasswordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnError);
    }
    void OnLoginSuccess(LoginResult result)
    {
        msgTxt.text = "Logged in ! Now loading menu..";
        msgTxt.color = Color.green;
        UpdateDisplayName(UsernameInput.text);
        
        GoToMenu();
    }
    public void ForgotPassButton()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = EmailInput.text,
            TitleId = "CD746"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnPasswordReset, OnError);
    }
    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        
        msgTxt.text =  "Passowrd reset mail sent !";
        msgTxt.color = Color.green;
        GoToLogin();
    }
    void ResetTxtError()
    {
        msgTxt.text = "";
    }



    //LeaderBoard

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName="Wins",
                    Value =score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Updated Leaderboard !");
    }
    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Wins",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError);
    }
    void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach (Transform item in RowsParent)
        {
            Destroy(item.gameObject);
        }
        foreach (var item in result.Leaderboard)
        {
            GameObject newGO = Instantiate(RowPrefab, RowsParent);
            Text[] texts = newGO.GetComponentsInChildren<Text>();
            int a = item.Position  +1;
            texts[0].text = a.ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();
            Debug.Log(item.Position+" "+item.DisplayName + " "+item.StatValue);
        }
    }
}
