using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Web;
using System.Web.Script.Serialization;

namespace AddPulseShowManagement.Common
{
    #region Class - ParameterItem

    /// <summary>
    /// The ParameterItem class is used to store a single key/value pair in the ParameterList class.
    /// </summary>
    [Serializable]
    public class ParameterItem
    {
        /// <summary>
        /// Gets or sets the key for the ParameterItem.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the value for the ParameterItem.
        /// </summary>
        public object Value { get; set; }

        //
        //  When true, all SqlXxxx properties must be set.
        //
        internal bool _SqlParameterInfoSet = false;
        internal SqlDbType SqlDbType { get; set; }
        internal ParameterDirection SqlParameterDirection { get; set; }
    }

    #endregion

    #region Class - ParameterList

    /// <summary>
    /// The ParameterList class represents a sequential, indexed list of name/value pair parameters.
    /// </summary>
    /// <remarks>
    /// This is used to build and manage lists of parameters for URL, form.
    /// <br/><br/>
    /// The order is important for ParameterList.  This class preserves the order of values, as they 
    /// are added, and provides indexed access via List[Key] as well.  This is not a classic dictionary
    /// or hash table with respect to the order of entries returned by IEnumerable.  The order of items
    /// returned by the Keys and Values properties in ParameterList are always the order that the
    /// Key/Value pairs were added to the list.  This is important for hashing algorithms that
    /// depend on the order of elements, and can be useful in other circumstances too.
    /// </remarks>
    /// <example>
    /// The ParameterList class is not a list or collection itself.  Instead, it exposes the Keys
    /// and Values properties for enumerations.  The following code would be a typical example
    /// of how to enumerate through a ParameterList:
    /// <br/>
    /// <code>
    ///      foreach (string key in myList.Keys)
    ///      {
    ///          object value = myList[key];
    ///          DoDomething(key, object);
    ///      }
    /// </code>
    /// </example>
    [Serializable]
    public class ParameterList
    {
        #region Fields

        private List<ParameterItem> _List = new List<ParameterItem>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a list of keys - one for each value in the ParameterList.
        /// </summary>
        [XmlIgnore]
        [ScriptIgnore]
        public List<string> Keys
        {
            get
            {
                return (from pi in _List select pi.Key).ToList();
            }
        }

        /// <summary>
        /// Gets a list of values in the ParameterList.
        /// </summary>
        [XmlIgnore]
        [ScriptIgnore]
        public List<object> Values
        {
            get
            {
                return (from pi in _List select pi.Value).ToList();
            }
        }

        /// <summary>
        /// Gets the value for a specific key in the ParameterList.
        /// <br/>
        /// If the key does not exist, then NULL will be returned.
        /// </summary>
        /// <param name="key">
        /// The Key of the parameter value.
        /// </param>
        [XmlIgnore]
        [ScriptIgnore]
        public object this[string key]
        {
            get
            {
                ParameterItem item = FindItemByKey(key);
                return (item == null) ? null : item.Value;
            }
        }

        /// <summary>
        /// Gets the value for a specific Index in the ParameterList.
        /// <br/>
        /// If the index is invalid, then NULL will be returned.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the parameter value.
        /// </param>
        [XmlIgnore]
        [ScriptIgnore]
        public object this[Int32 index]
        {
            get
            {
                if ((index >= 0) && (index < _List.Count))
                {
                    ParameterItem item = _List[index];
                    return item.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the internal list of ParameterItems.
        /// </summary>
        public List<ParameterItem> Items
        {
            get
            {
                return _List;
            }
            set
            {
                _List = value;
            }
        }

        #endregion

        #region Add

        /// <summary>
        /// Adds or updates a key/value pair in the ParameterList.
        /// <br/>
        /// If the key does not already exist, it will be added.
        /// <br/>
        /// If the key already exists, it will be updated.
        /// </summary>
        /// <param name="key">
        /// The unique key of the value being added.
        /// </param>
        /// <param name="value">
        /// The value corresponding to the specified Key.
        /// </param>
        public void Add(string key, object value)
        {
            ParameterItem item = FindItemByKey(key);

            if (item == null)
                _List.Add(new ParameterItem() { Key = key, Value = value });
            else
            {
                item.Value = value;
            }
        }

        /// <summary>
        /// Adds or updates a key/value pair in the ParameterList for use with SQL.
        /// <br/>
        /// If the key does not already exist, it will be added.
        /// <br/>
        /// If the key already exists, it will be updated.
        /// </summary>
        /// <param name="key">
        /// The unique key of the value being added.
        /// </param>
        /// <param name="value">
        /// The value corresponding to the specified Key.
        /// </param>
        /// <param name="sqlDbType">
        /// The SqlDbType to be used when calling SQL.
        /// </param>
        /// <remarks>
        /// The default parameter direction for this parameter will be ParameterDirection.Input.
        /// </remarks>
        public void Add(string key, object value, SqlDbType sqlDbType)
        {
            ParameterItem item = FindItemByKey(key);

            if (item == null)
            {
                item = new ParameterItem() { Key = key, Value = value, SqlDbType = sqlDbType, SqlParameterDirection = ParameterDirection.Input };
                _List.Add(item);
            }
            else
            {
                item.Value = value;
                item.SqlDbType = sqlDbType;
                item.SqlParameterDirection = ParameterDirection.Input;
            }

            item._SqlParameterInfoSet = true;
        }

        /// <summary>
        /// Adds or updates a key/value pair in the ParameterList for use with SQL.
        /// <br/>
        /// If the key does not already exist, it will be added.
        /// <br/>
        /// If the key already exists, it will be updated.
        /// </summary>
        /// <param name="key">
        /// The unique key of the value being added.
        /// </param>
        /// <param name="value">
        /// The value corresponding to the specified Key.
        /// </param>
        /// <param name="sqlDbType">
        /// The SqlDbType to be used when calling SQL.
        /// </param>
        /// <param name="sqlParameterDirection">
        /// The SQL ParameterDirection to be used when calling SQL.
        /// </param>
        public void Add(string key, object value, SqlDbType sqlDbType, ParameterDirection sqlParameterDirection)
        {
            ParameterItem item = FindItemByKey(key);

            if (item == null)
            {
                item = new ParameterItem() { Key = key, Value = value, SqlDbType = sqlDbType, SqlParameterDirection = sqlParameterDirection };
                _List.Add(item);
            }
            else
            {
                item.Value = value;
                item.SqlDbType = sqlDbType;
                item.SqlParameterDirection = sqlParameterDirection;
            }

            item._SqlParameterInfoSet = true;
        }

        #endregion

        #region FindItemByKey

        private ParameterItem FindItemByKey(string Key)
        {
            ParameterItem item = (from pi in _List where (string.Compare(pi.Key, Key, true) == 0) select pi).ToList().FirstOrDefault();
            return item;
        }

        #endregion

        #region Get

        /// <summary>
        /// Gets the Value of a key/value pair in the ParameterList,
        /// casting it to the type specified by &lt;T&gt;.
        /// <br/>
        /// If the Key does not already exist, an exception will be thrown.
        /// <br/>
        /// If the Value is not castable to the type specified by &lt;T&gt;, an exception will be thrown.
        /// </summary>
        /// <typeparam name="T">
        /// The type to which the value for the specified key will be converted.
        /// </typeparam>
        /// <param name="key">
        /// The unique Key of the value being added.
        /// </param>
        /// <returns>
        /// The value of the specified key converted to an object of type &lt;T&gt;.
        /// </returns>
        /// <remarks>
        /// To update a parameter, and add if if it doesn't exist, call the Add() method of this class.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The specified Key in the ParameterList does not exist.
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// The Key exists, but the specified value cannot be casted to type &lt;T&gt;.
        /// </exception>
        public T Get<T>(string key)
        {
            if (FindItemByKey(key) == null)
                throw new ArgumentException("The specified Key in the ParameterList does not exist.");

            object value = this[key];

            return (T)value;
        }

        #endregion

        #region GetDelimitedKeyList

        /// <summary>
        /// Returns a concatenated list of keys, delimited with the specified Delimiter.  
        /// </summary>
        /// <param name="delimiter">
        /// The character(s) that will be used to delimit each value in the concatenated list.
        /// </param>
        /// <returns>
        /// A string containing all keys, delimited by the specified Delimiter.
        /// </returns>
        public string GetDelimitedKeyList(string delimiter)
        {
            StringBuilder delimitedList = new StringBuilder();
            bool isFirst = true;

            foreach (ParameterItem item in _List)
            {
                if (isFirst == true)
                    isFirst = false;
                else
                    delimitedList.Append(delimiter);

                string strValue = item.Key;

                delimitedList.Append(strValue);
            }

            return delimitedList.ToString();
        }

        #endregion

        #region GetDelimitedKeyValueList

        /// <summary>
        /// Returns a concatenated list of keys and values where each key and value are separated 
        /// by the '=' character and each key/value pair is delimited with the '&amp;' character.
        /// This will call ToString() to get the string value of each ParameterList object.
        /// </summary>
        /// <param name="includeEmptyValues">
        /// If <b>true</b>, then null or empty (string.Emtpy) values will be included in the
        /// delimited list.  If <b>false</b>, then null or empty (string.Emtpy) values will not be 
        /// included in the delimited list.
        /// </param>
        /// <param name="urlEncode">
        /// Determines whether the key and value contents are URL encoded (HttlUtility.UrlEncode()).
        /// <br/>
        /// Note: The KeyValueSeparator and Delimiter will never be URL encoded.
        /// </param>
        /// <returns>
        /// A string containing all key/value pairs with keys and values separated by KeyValueSeparator
        /// and key/value pairs delimited by the specified KeyValuePairDelimiter.
        /// </returns>
        public string GetDelimitedKeyValueList(bool includeEmptyValues, bool urlEncode)
        {
            return GetDelimitedKeyValueList("=", "&", includeEmptyValues, urlEncode);
        }

        /// <summary>
        /// Returns a concatenated list of keys and values where each key and value are separated 
        /// by the specified KeyValueSeparator and each key/value pair is delimited with the 
        /// specified KeyValuePairDelimiter. This will call ToString() to get the string value of 
        /// each ParameterList object.
        /// </summary>
        /// <param name="keyValueSeparator">
        /// The character(s) that will be used to separate each key and value in the returned string.
        /// </param>
        /// <param name="keyValuePairDelimiter">
        /// The character(s) that will be used to delimit each key/value pair in the returned string.
        /// </param>
        /// <param name="includeEmptyValues">
        /// If <b>true</b>, then null or empty (string.Emtpy) values will be included in the
        /// delimited list.  If <b>false</b>, then null or empty (string.Emtpy) values will not be 
        /// included in the delimited list.
        /// </param>
        /// <param name="urlEncode">
        /// Determines whether the key and value contents are URL encoded (HttlUtility.UrlEncode()).
        /// <br/>
        /// Note: The KeyValueSeparator and Delimiter will never be URL encoded.
        /// </param>
        /// <returns>
        /// A string containing all key/value pairs with keys and values separated by KeyValueSeparator
        /// and key/value pairs delimited by the specified KeyValuePairDelimiter.
        /// </returns>
        public string GetDelimitedKeyValueList(string keyValueSeparator, string keyValuePairDelimiter, bool includeEmptyValues, bool urlEncode)
        {
            StringBuilder delimitedList = new StringBuilder();
            bool isFirst = true;

            foreach (ParameterItem item in _List)
            {
                string value = (item.Value == null) ? string.Empty : item.Value.ToString();

                if ((includeEmptyValues == false) && (value == string.Empty))
                    continue;

                if (isFirst == true)
                    isFirst = false;
                else
                    delimitedList.Append(keyValuePairDelimiter);

                if (urlEncode == true)
                {
                    string encodedKey = HttpUtility.UrlEncode(item.Key);
                    string encodedValue = HttpUtility.UrlEncode(value);
                    delimitedList.Append(string.Format("{0}{1}{2}", encodedKey, keyValueSeparator, encodedValue));
                }
                else
                    delimitedList.Append(string.Format("{0}{1}{2}", item.Key, keyValueSeparator, value));
            }

            return delimitedList.ToString();
        }

        #endregion

        #region GetDelimitedValueList

        /// <summary>
        /// Returns a concatenated list of values, delimited with the specified Delimiter.  
        /// This will call ToString() to get the string value of each ParameterList object.
        /// </summary>
        /// <param name="delimiter">
        /// The character(s) that will be used to delimit each value in the concatenated list.
        /// </param>
        /// <returns>
        /// A string containing all values, delimited by the specified Delimiter.
        /// </returns>
        public string GetDelimitedValueList(string delimiter)
        {
            StringBuilder delimitedList = new StringBuilder();
            bool isFirst = true;

            foreach (ParameterItem item in _List)
            {
                if (isFirst == true)
                    isFirst = false;
                else
                    delimitedList.Append(delimiter);

                string strValue = (item.Value == null) ? string.Empty : item.Value.ToString();

                delimitedList.Append(strValue);
            }

            return delimitedList.ToString();
        }

        #endregion

        #region JsonDeserialize

        /// <summary>
        /// Deserializes the contents of the specified JSON string into a ParameterList object.
        /// </summary>
        /// <param name="json">
        /// A string containing serialized JSON for ParameterList items.
        /// This is typically obtained from the JsonSerialize() method.
        /// </param>
        /// <returns>
        /// A new ParameterList object with items initialized to the contents of the specified JSON string.
        /// </returns>
        public static ParameterList JsonDeserialize(string json)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            
            ParameterList parameters = serializer.Deserialize<ParameterList>(json);

            return parameters;
        }

        #endregion

        #region JsonSerialize

        /// <summary>
        /// Serializes the contents of this ParameterList into a JSON formatted string.
        /// </summary>
        /// <returns>
        /// The contents of this ParameterList in a JSON formatted string.
        /// </returns>
        public string JsonSerialize()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string json = serializer.Serialize(this);

            return json;
        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes the parameter item for the specified Key.
        /// <br/>
        /// If the Key does not exist, then no action will be performed.
        /// </summary>
        /// <param name="key">
        /// The unique Key of the value being removed.
        /// </param>
        public void Remove(string key)
        {
            ParameterItem item = FindItemByKey(key);

            if (item != null)
                _List.Remove(item);
        }

        /// <summary>
        /// Removes the parameter item at the specified Index.
        /// <br/>
        /// If the index is invalid, then no action will be performed.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the value being removed.
        /// </param>
        public void Remove(Int32 index)
        {
            if ((index >= 0) && (index < _List.Count))
                _List.RemoveAt(index);
        }

        #endregion

        #region Set

        /// <summary>
        /// Sets the value of a key/value pair in the ParameterList.
        /// <br/>
        /// If the key does not already exist, an exception will be thrown.
        /// </summary>
        /// <param name="key">
        /// The unique key of the value.
        /// </param>
        /// <param name="value">
        /// The value corresponding to the specified Key.
        /// </param>
        /// <remarks>
        /// To update a parameter, and add if if it doesn't exist, call the Add() method of this class.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The specified Key in the ParameterList does not exist.
        /// </exception>
        public void Set(string key, object value)
        {
            if (FindItemByKey(key) == null)
                throw new ArgumentException("The specified Key in the ParameterList does not exist.");

            Add(key, value);    // Will update existing items
        }

        /// <summary>
        /// Sets the Value and SqlDbType of a key/value pair in the ParameterList.
        /// <br/>
        /// If the Key does not already exist, an exception will be thrown.
        /// </summary>
        /// <param name="key">
        /// The unique key of the value.
        /// </param>
        /// <param name="value">
        /// The value corresponding to the specified Key.
        /// </param>
        /// <param name="sqlDbType">
        /// The SqlDbType to be used when calling SQL.
        /// </param>
        /// <remarks>
        /// To update a parameter, and add if if it doesn't exist, call the Add() method of this class.
        /// <br/><br/>
        /// The default parameter direction for this parameter will be ParameterDirection.Input.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The specified Key in the ParameterList does not exist.
        /// </exception>
        public void Set(string key, object value, SqlDbType sqlDbType)
        {
            if (FindItemByKey(key) == null)
                throw new ArgumentException("The specified Key in the ParameterList does not exist.");

            Add(key, value, sqlDbType);
        }

        /// <summary>
        /// Sets the Value, SqlDbType and SqlParameterDirection of a key/value pair in the ParameterList.
        /// <br/>
        /// If the Key does not already exist, an exception will be thrown.
        /// </summary>
        /// <param name="key">
        /// The unique key of the value.
        /// </param>
        /// <param name="value">
        /// The value corresponding to the specified Key.
        /// </param>
        /// <param name="sqlDbType">
        /// The SqlDbType to be used when calling SQL.
        /// </param>
        /// <param name="sqlParameterDirection">
        /// The SQL ParameterDirection to be used when calling SQL.
        /// </param>
        /// <remarks>
        /// To update a parameter, and add if if it doesn't exist, call the Add() method of this class.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// The specified Key in the ParameterList does not exist.
        /// </exception>
        public void Set(string key, object value, SqlDbType sqlDbType, ParameterDirection sqlParameterDirection)
        {
            if (FindItemByKey(key) == null)
                throw new ArgumentException("The specified Key in the ParameterList does not exist.");

            Add(key, value, sqlDbType, sqlParameterDirection);
        }

        #endregion

        #region QueryStringMake

        /// <summary>
        /// Creates a query string containing all key/value pairs in this ParameterList object.
        /// </summary>
        /// <returns>
        /// A query string containing all key/value pairs in this ParameterList object.
        /// <br/><br/>
        /// The query string will be formatted as "?Key1=Value1&amp;Key2=Value2..."
        /// <br/><br/>
        /// If this ParameterList contains no items, then string.Empty will be returned.
        /// </returns>
        public string QueryStringMake()
        {
            return QueryStringMake(true);
        }

        /// <summary>
        /// Creates a query string containing all key/value pairs in this ParameterList object.
        /// </summary>
        /// <param name="includeQuestionMark">
        /// If <b>true</b>, a question mark will be prepended to the query string.
        /// </param>
        /// <returns>
        /// A query string containing all key/value pairs in this ParameterList object.
        /// </returns>
        /// <remarks>
        /// If includeQuestionMark is <b>true</b>, then the query string will 
        /// be formatted as "?Key1=Value1&amp;Key2=Value2..."
        /// <br/><br/>
        /// If includeQuestionMark is <b>false</b>, then the query string will 
        /// be formatted as "Key1=Value1&amp;Key2=Value2..."
        /// <br/><br/>
        /// If this ParameterList contains no items, then string.Empty will be returned.
        /// </remarks>
        public string QueryStringMake(bool includeQuestionMark)
        {
            string queryString = (includeQuestionMark) ? "?" : string.Empty;

            if (_List.Count > 0)
                queryString += GetDelimitedKeyValueList("=", "&", true, true);

            return queryString;
        }

        #endregion

        #region QueryStringSplit

        /// <summary>
        /// Parses the specified QueryString and returns a ParameterList object
        /// containing all key/value pairs in the QueryString.
        /// </summary>
        /// <param name="queryString">
        /// A typical web query string formatted as "Key1=Value1&amp;Key2=Value2..."
        /// <br/>
        /// The query string may begin with the "?" character, but does not have to.
        /// <br/>
        /// If the query string begins with the "?" character, this cahracter will be ignored.
        /// </param>
        /// <returns>
        /// A ParameterList object containing all key/value pairs in the QueryString.
        /// <br/>
        /// All values in the returned list will be of type string.
        /// </returns>
        public static ParameterList QueryStringSplit(string queryString)
        {
            ParameterList parameters = new ParameterList();

            if (queryString.Length > 0)
            {
                if (queryString[0] == '?')
                    queryString = queryString.Substring(1);

                string[] keyValuePairs = queryString.Split('&');

                foreach (string kvp in keyValuePairs)
                {
                    string[] keyValue = kvp.Split('=');

                    if (keyValue.Length != 2)
                        throw new ArgumentException("The QueryString is not formatted properly - Key=Value expected.");

                    parameters.Add(keyValue[0], keyValue[1]);
                }
            }

            return parameters;
        }

        #endregion
    }

    #endregion
}
