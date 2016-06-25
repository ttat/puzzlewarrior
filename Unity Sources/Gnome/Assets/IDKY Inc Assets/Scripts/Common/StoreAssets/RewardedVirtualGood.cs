// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using Soomla.Store;

    /// <summary>
    /// Special LifetimeVG for Special Stages.  We need to know if it's something the user can purchase or it's
    /// it can only be given to them so we know which pop up to show.
    /// </summary>
    public class SpecialStageLifetimeVg : LifetimeVG
    {
        #region Constructors and Destructors

        public SpecialStageLifetimeVg(string name, string description, string itemId, PurchaseType purchaseType, bool canPurchase)
            : base(name, description, itemId, purchaseType)
        {
            this.CanPurchase = canPurchase;
        }

        public SpecialStageLifetimeVg(JSONObject jsonVg, bool canPurchase)
            : base(jsonVg)
        {
            this.CanPurchase = canPurchase;
        }

        #endregion

        #region Public Properties

        public bool CanPurchase { get; private set; }

        #endregion
    }
}