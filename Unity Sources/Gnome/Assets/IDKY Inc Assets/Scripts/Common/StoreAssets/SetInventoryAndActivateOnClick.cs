// ----------------------------------------------
// 
//             Counting 2s
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

public class SetInventoryAndActivateOnClick : ActivateOnClick
{
    #region Fields

    public bool AllowClick;

    public InventoryItem[] InventoryItems;

    #endregion

    #region Methods

    protected override void OnClick()
    {
        foreach (InventoryItem inventoryItem in this.InventoryItems)
        {
            inventoryItem.AllowClick(this.AllowClick);
        }

        base.OnClick();
    }

    private void Start()
    {
        this.InventoryItems = FindObjectsOfType<InventoryItem>();
    }

    #endregion
}