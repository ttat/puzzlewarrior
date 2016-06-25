// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System;

using UnityEngine;

public class LogGoogleAnalyticEventOnClick : MonoBehaviour
{
    #region Fields

    public string Action;

    public string Category;

    public LogType EventLoggingType;

    public string Label;

    public string SocialAction;

    public string SocialNetwork;

    public string SocialTarget;

    public long Value;

    #endregion

    #region Enums

    public enum LogType
    {
        Event,

        Social
    }

    #endregion

    #region Methods

    private void LogEvent()
    {
        // Build the event
        EventHitBuilder eventHitBuilder = new EventHitBuilder();

        if (!string.IsNullOrEmpty(this.Action))
        {
            eventHitBuilder.SetEventAction(this.Action);
        }

        if (!string.IsNullOrEmpty(this.Category))
        {
            eventHitBuilder.SetEventCategory(this.Category);
        }

        if (!string.IsNullOrEmpty(this.Label))
        {
            eventHitBuilder.SetEventLabel(this.Label);
        }

        if (this.Value != 0)
        {
            eventHitBuilder.SetEventValue(this.Value);
        }

        // Now log it
        GoogleAnalyticsV3.instance.LogEvent(eventHitBuilder);
    }

    private void LogSocial()
    {
        // Build the event
        SocialHitBuilder socialHitBuilder = new SocialHitBuilder();

        if (!string.IsNullOrEmpty(this.SocialNetwork))
        {
            socialHitBuilder.SetSocialNetwork(this.SocialNetwork);
        }

        if (!string.IsNullOrEmpty(this.SocialAction))
        {
            socialHitBuilder.SetSocialAction(this.SocialAction);
        }

        if (!string.IsNullOrEmpty(this.SocialTarget))
        {
            socialHitBuilder.SetSocialTarget(this.SocialTarget);
        }

        // Now log it
        GoogleAnalyticsV3.instance.LogSocial(socialHitBuilder);
    }

    private void OnClick()
    {
        switch (this.EventLoggingType)
        {
            case LogType.Event:
                this.LogEvent();
                break;

            case LogType.Social:
                this.LogSocial();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    #endregion
}