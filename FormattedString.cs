using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Halcyon
{
    [Serializable]
    public class FormattedString 
    {

        public string baseText;//Initial text inlcluding the interpolated string key locations
        [SerializeField]
        private List<string> dropDownCollection = new List<string>();//Used within editor to show all of the different variable options.
        [SerializeField]
        private bool useIndices = true; //Changes the use of {elementName] to {0}
        private Dictionary<string, Func<object>> keyValues = new Dictionary<string, Func<object>>();//Holds all keys and functions to retrieve values.


        /// <summary>
        /// Registers the object's referenced value to the corresponding string. Add the string to the editor options.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void RegisterValue(string key, object obj)
        {
            if (!dropDownCollection.Contains(key))
                dropDownCollection.Add(key);

            if (keyValues.ContainsKey(key))
            {
                keyValues[key] = () => obj.ToString();
            }
            else
            {
                keyValues.Add(key, () => obj.ToString());
            }
        }

        /// <summary>
        /// Removes the item from both the dropDownCollection and the Dictionary
        /// </summary>
        /// <param name="key"></param>
        public void UnRegisterValue(string key)
        {
            dropDownCollection.Remove(key);
            keyValues.Remove(key);
        }

        public void ClearValues()
        {
            dropDownCollection.Clear();
            keyValues.Clear();
        }
        
        /// <summary>
        /// Use to debug if the value is actually is what it should be
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string key)
        {

            if (keyValues.ContainsKey(key))
                return (string)keyValues[key].Invoke();
            else
                return "{Unable to find: " + key + "}";
        }

        
        /// <summary>
        /// Retrieves the formatted string and inserts the key values into the text.
        /// </summary>
        /// <returns></returns>
        public string GetFormattedString()
        {
            if (!baseText.Contains('{'))
                return baseText;
            string formattedString = "";

            foreach (var split in baseText.Split('{'))
            {
                if (!split.Contains('}'))
                {
                    formattedString += split;
                }
                else
                {
                    int index = split.IndexOf('}');

                   
                    string searchString = "";
                    if (useIndices)
                    {
                        int listIndex = Int32.Parse(split.Substring(0, index));
                        if (listIndex > dropDownCollection.Count)
                        {
                            searchString = listIndex.ToString();
                        }
                        else
                        {
                            searchString = dropDownCollection[listIndex];
                        }
                    }
                    else
                    {
                        searchString = split.Substring(0, index);
                    }

                    formattedString += GetValue(searchString);

                    if (index < split.Length)
                        formattedString += split.Substring(index + 1);
                }
            }

            return formattedString;
        }

        /// <summary>
        /// Use in the OnValidate function within Unity to find all of the [StringKey] fields.
        /// </summary>
        /// <param name="obj"></param>
        public void ValidationInitialization(object obj)
        {
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase);

            //Console.WriteLine("Number of Fields: " + fields.Length);
            AddAttributeFields(fields, obj);
           
        }
        /// <summary>
        /// Use in the OnValidate function within Unity to find all of the fields matching the enumeration names.
        /// </summary>
        /// <param name="obj"></param>
        public void ValidationInitialization<T>(object obj, bool getAttributedFields = true) where T : Enum
        {
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase);
            
            if (getAttributedFields)
            {
                AddAttributeFields(fields, obj);
            }

            T enumeration = default;
            string[] enumNames = enumeration.GetType().GetEnumNames();

            AddStringFields(fields, obj, enumNames);
        }
        /// <summary>
        /// Use in the OnValidate function within Unity to find of the fields matching the string values.
        /// </summary>
        /// <param name="obj"></param>
        public void ValidationInitialization(object obj, string[] fieldNames, bool getAttributedFields = true)
        {
            var type = obj.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (getAttributedFields)
            {
                AddAttributeFields(fields, obj);
            }
            AddStringFields(fields, obj, fieldNames);
        }

        /// <summary>
        /// Adds or overrides all of the fields containing [StringKey] to the list and dictionary.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="obj"></param>
        public void AddAttributeFields(FieldInfo [] fields, object obj)
        {
            foreach (var field in fields)
            {
                if (field.GetCustomAttribute(typeof(StringKeyAttribute)) != null)
                {

                    if (!dropDownCollection.Contains(field.Name))
                        dropDownCollection.Add(field.Name);

                    if (keyValues.ContainsKey(field.Name))
                    {
                        Console.WriteLine(field.GetValue(obj));
                        keyValues[field.Name] = () => field.GetValue(obj).ToString();
                    }
                    else
                    {
                        keyValues.Add(field.Name, () => field.GetValue(obj).ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Tries to add or overrides the value for the desired strings values.
        /// </summary>
        /// <param name="obj"></param>
        public void AddStringFields(FieldInfo [] fields, object obj, string [] names)
        {

            foreach (var field in fields)
            {
                foreach (string name in names)
                {
                    if (name != field.Name)
                        continue;
                    try
                    {
                        if (!dropDownCollection.Contains(field.Name))
                            dropDownCollection.Add(field.Name);

                        if (keyValues.ContainsKey(field.Name))
                        {
                            Console.WriteLine(field.GetValue(obj));
                            keyValues[field.Name] = () => field.GetValue(obj).ToString();
                        }
                        else
                        {
                            keyValues.Add(field.Name, () => field.GetValue(obj).ToString());
                        }
                    }
                    catch
                    {
                        //Logging the error
                    }
                }
            }
        }
    }
}
