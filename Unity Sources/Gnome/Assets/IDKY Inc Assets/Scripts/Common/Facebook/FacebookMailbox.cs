// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using System.Collections.Generic;

using Idky;

using UnityEngine;

public class FacebookMailbox : MonoBehaviour
{
    #region Fields

    public UIRect.AnchorPoint BottomAnchor;

    public FacebookItem FacebookAcceptItem;

    public UIGrid Grid;

    public UIRect.AnchorPoint LeftAnchor;

    public UIRect.AnchorPoint RightAnchor;

    public UIRect.AnchorPoint TopAnchor;

    #endregion

    #region Public Methods and Operators

    public void SetUpMailbox(List<FacebookAppRequest> appRequests)
    {
        foreach (Transform childTransform in this.Grid.transform)
        {
            Destroy(childTransform.gameObject);
        }

        foreach (FacebookAppRequest appRequest in appRequests)
        {
            string appDataTypeString = FacebookAppRequestDataParser.GetAppDataType(appRequest.Data);

            FacebookAppRequest.AppDataType appDataType = FacebookAppRequest.AppDataType.MonsterUnlock;

            // Ignore unknown types for future proofing
            if (appDataType.TryParse(appDataTypeString, out appDataType))
            {
                GameObject item = NGUITools.AddChild(this.Grid.gameObject, this.FacebookAcceptItem.gameObject);

                FacebookItem facebookItem = item.GetComponent<FacebookItem>();
                facebookItem.SetItem(appRequest);
                facebookItem.Grid = this.Grid;

                UIWidget widgetItem = item.GetComponent<UIWidget>();
                widgetItem.leftAnchor = this.LeftAnchor;
                widgetItem.rightAnchor = this.RightAnchor;
                widgetItem.topAnchor = this.TopAnchor;
                widgetItem.bottomAnchor = this.BottomAnchor;
                widgetItem.ResetAnchors();
            }
        }

        this.Grid.Reposition();
    }

    #endregion
}