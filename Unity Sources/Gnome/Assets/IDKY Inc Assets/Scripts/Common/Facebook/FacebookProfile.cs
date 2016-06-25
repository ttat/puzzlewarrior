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
    public class FacebookProfile
    {
        #region Constructors and Destructors

        public FacebookProfile()
        {
            this.Id = string.Empty;
            this.FirstName = string.Empty;
            this.LastName = string.Empty;
        }

        public FacebookProfile(string id, string firstName, string lastName)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        #endregion

        #region Public Properties

        public string FirstName { get; set; }

        public string Id { get; set; }

        public string LastName { get; set; }

        #endregion
    }
}