// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

using Idky;

using UnityEngine;

public class FacebookItem : MonoBehaviour
{
    #region Fields

    public FacebookAppRequest AppRequest;

    public UILabel ButtonLabel;

    public UIGrid Grid;

    public UILabel Message;

    #endregion

    #region Public Methods and Operators

    public void OnClick()
    {
        FacebookManager.Instance.FacebookAppRequestDeletedEvent += this.OnFacebookAppRequestDeleted;
        FacebookManager.Instance.DeleteAppRequest(this.AppRequest);
    }

    public void SetItem(FacebookAppRequest appRequest)
    {
        this.AppRequest = appRequest;

        string description;
        string person = appRequest.FromName;

        string data = FacebookAppRequestDataParser.GetAppRequestObject(appRequest.Data);
        string dataType = FacebookAppRequestDataParser.GetAppDataType(appRequest.Data);

        switch (dataType)
        {
            case "MonsterUnlock":
                description = data;
                break;

            case "Inventory":
                description = ShopManager.VirtualGoodsDictionary[data].Name;

                break;

            default:
                description = "Unknown";
                break;
        }

        switch (FacebookAppRequestDataParser.GetAppRequestType(appRequest.Data))
        {
            case FacebookAppRequest.AppRequestType.Gift:
                this.Message.text = string.Format("Accept {0} from {1}", description, person);
                this.ButtonLabel.text = "Accept";
                break;

            case FacebookAppRequest.AppRequestType.Request:
                this.Message.text = string.Format("Send {0} to {1}", description, person);
                this.ButtonLabel.text = "Send";
                break;
        }
    }

    #endregion

    #region Methods

    private void OnFacebookAppRequestDeleted(object sender, FacebookAppRequestDeletedEventArgs args)
    {
        FacebookManager.Instance.FacebookAppRequestDeletedEvent -= this.OnFacebookAppRequestDeleted;

        if (args.DeletedAppRequest == this.AppRequest)
        {
            // Delete was successful, so it's safe to process the request now
            GameManager.Instance.ProcessAppRequest(args.DeletedAppRequest);

            this.gameObject.transform.parent = null;
            Destroy(this.gameObject);
            this.Grid.Reposition();
        }
    }

    #endregion
}