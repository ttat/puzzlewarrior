// ----------------------------------------------
// 
//             2 Upper
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class OptionOnOff : MonoBehaviour
{
    #region Fields

    public bool CurrentState;

    /// <summary>
    /// The state the option will be if it can't be read from the PlayerPrefs.
    /// </summary>
    public bool DefaultState = true;

    public GameObject DisabledObject;

    public GameObject EnabledObject;

    public string OptionKey = "Option";

    #endregion

    #region Public Methods and Operators

    public void OnStateChanged(bool state)
    {
        // Set the state
        this.CurrentState = state;
        this.EnableObjects();

        // Save the state
        PlayerPrefsFast.SetBool(this.OptionKey, state);
        PlayerPrefsFast.Flush();

        SoundManager.Instance.RecheckSounds();
    }

    public void ToggleState()
    {
#if UNITY_ANDROID
        // Prevent clicking if the Ad is visible (one of the gotchas for Chartboost on Android)
        if (AdManager.Instance != null && AdManager.Instance.IsImpressionVisible())
        {
            return;
        }
#endif

        this.OnStateChanged(!this.CurrentState);
    }

    #endregion

    #region Methods

    private void Awake()
    {
        try
        {
            this.CurrentState = PlayerPrefsFast.GetBool(this.OptionKey, this.DefaultState);
            this.EnableObjects();
        }
        catch
        {
            // Set a default for this new key
            this.OnStateChanged(this.DefaultState);
        }
    }

    private void EnableObjects()
    {
        this.EnabledObject.SetActive(this.CurrentState);
        this.DisabledObject.SetActive(!this.CurrentState);
    }

    #endregion
}