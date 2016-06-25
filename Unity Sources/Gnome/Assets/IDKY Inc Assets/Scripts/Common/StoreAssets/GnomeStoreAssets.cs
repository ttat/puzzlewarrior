// ----------------------------------------------
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System.Collections.Generic;

    using Soomla.Store;

    public class GnomeStoreAssets : IStoreAssets
    {
        #region Constants

        public const string RewardStage01Id = "reward_stage_01";

        public const string SpecialStage01Id = "special_stage_01";

        public const string TokenId = "token";

        public const string TokenPack250Id = "token_250";

        public const string TokenPack500Id = "token_500";

        public const string TokenPack75Id = "token_75";

        #endregion

        #region Static Fields

        public static SpecialStageLifetimeVg RewardStage01 = new SpecialStageLifetimeVg(
            "Reward Stage 01", "Reward Stage 01", RewardStage01Id, new PurchaseWithVirtualItem(TokenId, 9999), false);

        public static SpecialStageLifetimeVg SpecialStage01 = new SpecialStageLifetimeVg(
            "Special Stage 01", "Special Stage 01", SpecialStage01Id, new PurchaseWithVirtualItem(TokenId, 30), true);

        public static VirtualCurrency Token = new VirtualCurrency("Token", "Token", TokenId);

        public static VirtualCurrencyPack TokenPack250 = new VirtualCurrencyPack(
            "Token 250 Pack", "Pack of 250 tokens", TokenPack250Id, 250, TokenId, new PurchaseWithMarket(TokenPack250Id, 2.99));

        public static VirtualCurrencyPack TokenPack75 = new VirtualCurrencyPack(
            "Token 75 Pack", "Pack of 75 tokens", TokenPack75Id, 75, TokenId, new PurchaseWithMarket(TokenPack75Id, 0.99));

        public static VirtualCurrencyPack TokenPack500 = new VirtualCurrencyPack(
            "Token 500 Pack", "Pack of 500 tokens", TokenPack500Id, 500, TokenId, new PurchaseWithMarket(TokenPack500Id, 4.99));

        #endregion

        #region Public Methods and Operators

        public static VirtualGood[] GetGoodsStatic()
        {
            return new VirtualGood[] { SpecialStage01, RewardStage01 };
        }

        public static Dictionary<string, SpecialStageLifetimeVg> GetSpecialStagesStatic()
        {
            return new Dictionary<string, SpecialStageLifetimeVg>
                       { { SpecialStage01.ItemId, SpecialStage01 }, { RewardStage01.ItemId, RewardStage01 } };
        }

        public VirtualCategory[] GetCategories()
        {
            return new VirtualCategory[0];
        }

        public VirtualCurrency[] GetCurrencies()
        {
            return new[] { Token };
        }

        public VirtualCurrencyPack[] GetCurrencyPacks()
        {
            return new[] { TokenPack75, TokenPack250, TokenPack500 };
        }

        public VirtualGood[] GetGoods()
        {
            return new VirtualGood[] { SpecialStage01, RewardStage01 };
        }

        public int GetVersion()
        {
            return 3;
        }

        #endregion
    }
}