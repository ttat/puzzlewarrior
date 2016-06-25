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
    using System;

    public static class FacebookAppRequestDataParser
    {
        #region Public Methods and Operators

        /// <summary>
        /// Generates the data string.
        /// </summary>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string GenerateDataString(
            FacebookAppRequest.AppRequestType requestType, string dataType, string data)
        {
            string dataString = string.Format("{0};{1};{2}", requestType.ToString(), dataType, data);

            return dataString;
        }

        /// <summary>
        /// Generates the data string.
        /// </summary>
        /// <param name="requestType">Type of the request.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="data">The data.</param>
        /// <param name="additionalData">The additional data.</param>
        /// <returns></returns>
        public static string GenerateDataString(
            FacebookAppRequest.AppRequestType requestType, string dataType, string data, string additionalData)
        {
            string dataString = string.Format("{0};{1};{2};{3}", requestType.ToString(), dataType, data, additionalData);

            return dataString;
        }

        /// <summary>
        /// Gets the type of the app data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string GetAppDataType(string data)
        {
            string[] requestData = data.Split(new[] { ';' });

            return requestData[1];
        }

        /// <summary>
        /// Gets the app request additional data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string GetAppRequestAdditionalData(string data)
        {
            string additionalData = string.Empty;
            string[] requestData = data.Split(new[] { ';' });

            if (requestData.Length >= 4)
            {
                additionalData = requestData[3];
            }

            return additionalData;
        }

        /// <summary>
        /// Gets the app request object.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static string GetAppRequestObject(string data)
        {
            string requestObject = string.Empty;
            string[] requestData = data.Split(new[] { ';' });

            if (requestData.Length >= 3)
            {
                requestObject = requestData[2];
            }

            return requestObject;
        }

        /// <summary>
        /// Gets the type of the app request.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static FacebookAppRequest.AppRequestType GetAppRequestType(string data)
        {
            FacebookAppRequest.AppRequestType requestType;

            try
            {
                string[] requestData = data.Split(new[] { ';' });

                requestType = (FacebookAppRequest.AppRequestType)Enum.Parse(typeof(FacebookAppRequest.AppRequestType), requestData[0], true);
            }
            catch (Exception ex)
            {
                requestType = FacebookAppRequest.AppRequestType.Request;
            }

            return requestType;
        }

        #endregion
    }
}