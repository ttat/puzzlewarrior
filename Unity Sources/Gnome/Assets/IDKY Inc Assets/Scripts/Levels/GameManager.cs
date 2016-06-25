// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Fields

    public GameObject Inventory;

    public UIPanel Panel;

    #endregion

    #region Public Properties

    public static GameManager Instance { get; private set; }

    #endregion

    #region Public Methods and Operators

    public bool EnterHighScore(float score)
    {
        bool isNewHighScore = false;
        float highScore = PlayerPrefsFast.GetFloat(SharedResources.HighScoreKey, 0);

        if (score > highScore)
        {
            isNewHighScore = true;
            PlayerPrefsFast.SetFloat(SharedResources.HighScoreKey, score);
            PlayerPrefsFast.Flush();

            FacebookManager.Instance.PostHighScore((uint)score);
        }

        return isNewHighScore;
    }

    public void ProcessAppRequest(FacebookAppRequest appRequest)
    {
        switch (FacebookAppRequestDataParser.GetAppRequestType(appRequest.Data))
        {
            case FacebookAppRequest.AppRequestType.Request:
                this.ProcessRequest(appRequest);
                break;

            case FacebookAppRequest.AppRequestType.Gift:
                this.ProcessGift(appRequest);
                break;

            default:
                break;
        }
    }

    #endregion

    #region Methods

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnLevelWasLoaded(int level)
    {
        FacebookManager.Instance.GetHighScores();
    }

    private void ProcessGift(FacebookAppRequest appRequest)
    {
        switch (FacebookAppRequestDataParser.GetAppDataType(appRequest.Data))
        {
            default:
                break;
        }
    }

    private void ProcessRequest(FacebookAppRequest appRequest)
    {
        string appRequestObject = FacebookAppRequestDataParser.GetAppRequestObject(appRequest.Data);
        string appDataType = FacebookAppRequestDataParser.GetAppDataType(appRequest.Data);
        string dataString = FacebookAppRequestDataParser.GenerateDataString(
            FacebookAppRequest.AppRequestType.Gift, appDataType, appRequestObject);

        switch (appDataType)
        {
            default:
                break;
        }
    }

    private void Start()
    {
        // Just in case we started it in the level already.  It doesn't call OnLevelWasLoaded
        // when we first start the game in the editor.
        this.OnLevelWasLoaded(Application.loadedLevel);
    }

    private void Update()
    {
    }

    #endregion
}