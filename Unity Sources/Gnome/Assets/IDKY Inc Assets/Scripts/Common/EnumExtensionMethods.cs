// ----------------------------------------------
// 
//             Terrible Monster Slap
// 
//  Copyright © 2013-2014 IDKY Developments
//  Authors: Thomson Tat, Dinh Cao
// 
// ----------------------------------------------

namespace Idky
{
    using System;
    using System.ComponentModel;
    using System.Reflection;

    public static class EnumExtensionMethods
    {
        #region Public Methods and Operators

        public static string GetDescription(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            string enumDescription = attributes.Length > 0 ? attributes[0].Description : enumValue.ToString();

            return enumDescription;
        }

        public static string GetMonsterId(this Enum enumValue)
        {
            return "monster_" + enumValue.ToString().ToLowerInvariant() + "_id";
        }

        public static bool TryGetEnumFromId<T>(this Enum enumValue, string id, out T returnValue)
        {
            string[] split = id.Split('_');

            string monsterString = split[1];

            return enumValue.TryParse(monsterString, true, out returnValue);
        }

        public static bool TryParse<T>(this Enum theEnum, string valueToParse, out T returnValue)
        {
            returnValue = default(T);
            if (Enum.IsDefined(typeof(T), valueToParse))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                returnValue = (T)converter.ConvertFromString(valueToParse);

                return true;
            }

            return false;
        }

        public static bool TryParse<T>(this Enum theEnum, string valueToParse, bool isCaseInsensitive, out T returnValue)
        {
            bool found = false;
            returnValue = default(T);
            string[] names = Enum.GetNames(typeof(T));

            foreach (string name in names)
            {
                if (name.Equals(valueToParse, StringComparison.InvariantCultureIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                returnValue = (T)Enum.Parse(typeof(T), valueToParse, true);
            }

            return found;
        }

        #endregion
    }
}