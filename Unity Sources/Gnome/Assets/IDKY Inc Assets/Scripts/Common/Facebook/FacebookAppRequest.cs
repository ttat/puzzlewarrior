// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    public class FacebookAppRequest
    {
        #region Enums

        public enum AppDataType
        {
            MonsterUnlock,

            Inventory
        }

        public enum AppRequestType
        {
            Request,

            Gift
        }

        #endregion

        #region Public Properties

        public string ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public string Data { get; set; }

        public string FromCreateTime { get; set; }

        public string FromId { get; set; }

        public string FromMessage { get; set; }

        public string FromName { get; set; }

        public string RequestId { get; set; }

        public string ToId { get; set; }

        public string ToName { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return
                string.Format(
                    "[FacebookAppRequest: RequestId={0}, ApplicationName={1}, ApplicationId={2}, ToName={3}, ToId={4}, FromName={5}, FromId={6}, FromMessage={7}, FromCreateTime={8}, Data={9}]",
                    this.RequestId,
                    this.ApplicationName,
                    this.ApplicationId,
                    this.ToName,
                    this.ToId,
                    this.FromName,
                    this.FromId,
                    this.FromMessage,
                    this.FromCreateTime,
                    this.Data);
        }

        #endregion
    }
}