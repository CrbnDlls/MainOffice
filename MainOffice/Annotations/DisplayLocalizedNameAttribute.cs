using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainOffice.Annotations
{

    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Property | System.AttributeTargets.Interface, AllowMultiple = true)]
    public sealed class LocalizedAdditionalMetadataAttribute : Attribute, IMetadataAware
    {
        private object _typeId = new object();

        /// <summary>Инициализирует новый экземпляр класса <see cref="T:System.Web.Mvc.AdditionalMetadataAttribute" />.</summary>
        /// <param name="AtrKey">Имя метаданных модели.</param>
        /// <param name="ResourceName">Ключь ресурса значения метаданных модели.</param>
        /// <param name="ResourceType">Ресурс где хранятся значения</param>
        public LocalizedAdditionalMetadataAttribute(string ResourceName, Type ResourceType, string AtrKey)
        {
            PropertyInfo property = ResourceType.GetProperty("EmployeeShortName");
            string test = (string)property.GetValue(null, null);
            if (ResourceName == null)
                throw new ArgumentNullException(nameof(ResourceName));
            ResourceManager mngr = new ResourceManager(ResourceType);
            object value = mngr.GetObject(ResourceName);
            this.Name = AtrKey;
            this.Value = value;
           
        }

        /// <summary>Получает тип дополнительного атрибута метаданных.</summary>
        /// <returns>Тип дополнительного атрибута метаданных.</returns>
        public override object TypeId
        {
            get
            {
                return this._typeId;
            }
        }

        /// <summary>Получает имя дополнительного атрибута метаданных.</summary>
        /// <returns>Имя дополнительного атрибута метаданных.</returns>
        public string Name { get; private set; }

        /// <summary>Получает значение дополнительного атрибута метаданных.</summary>
        /// <returns>Значение дополнительного атрибута метаданных.</returns>
        public object Value { get; private set; }

        /// <summary>Предоставляет метаданные в процессе создания метаданных модели.</summary>
        /// <param name="metadata">Метаданные.</param>
        public void OnMetadataCreated(ModelMetadata metadata)
        {
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));
            metadata.AdditionalValues[this.Name] = this.Value;
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Interface, AllowMultiple = false)]
    public sealed class DisplayClassAttribute : Attribute, IMetadataAware
    {
        #region Member Fields

        private Type _resourceType;
        private LocalizableString _shortName = new LocalizableString("ShortName");
        private LocalizableString _name = new LocalizableString("Name");
        private LocalizableString _groupName = new LocalizableString("GroupName");
        private string _controllerName;

        #endregion

        #region All Constructors

        /// <summary>
        /// Default constructor for DisplayAttribute.  All associated string properties and methods will return <c>null</c>.
        /// </summary>
        public DisplayClassAttribute()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ShortName attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetShortName"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// short name for display.
        /// <para>
        /// The <see cref="GetShortName"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// The short name is generally used as the grid column label for a UI element bound to the member
        /// bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string ShortName
        {
            get
            {
                return this._shortName.Value;
            }
            set
            {
                if (this._shortName.Value != value)
                {
                    this._shortName.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Name attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetName"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// name for display.
        /// <para>
        /// The <see cref="GetName"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// The name is generally used as the field label for a UI element bound to the member
        /// bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string Name
        {
            get
            {
                return this._name.Value;
            }
            set
            {
                if (this._name.Value != value)
                {
                    this._name.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the Description attribute property, which may be a resource key string.
        /// <para>
        /// Consumers must use the <see cref="GetDescription"/> method to retrieve the UI display string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The property contains either the literal, non-localized string or the resource key
        /// to be used in conjunction with <see cref="ResourceType"/> to configure a localized
        /// description for display.
        /// <para>
        /// The <see cref="GetDescription"/> method will return either the literal, non-localized
        /// string or the localized string when <see cref="ResourceType"/> has been specified.
        /// </para>
        /// </remarks>
        /// <value>
        /// Description is generally used as a tool tip or description a UI element bound to the member
        /// bearing this attribute.  A <c>null</c> or empty string is legal, and consumers must allow for that.
        /// </value>
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string GroupName
        {
            get
            {
                return this._groupName.Value;
            }
            set
            {
                if (this._groupName.Value != value)
                {
                    this._groupName.Value = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Type"/> that contains the resources for <see cref="ShortName"/>,
        /// <see cref="Name"/>, <see cref="Description"/>, <see cref="Prompt"/>, and <see cref="GroupName"/>.
        /// Using <see cref="ResourceType"/> along with these Key properties, allows the <see cref="GetShortName"/>,
        /// <see cref="GetName"/>, <see cref="GetDescription"/>, <see cref="GetPrompt"/>, and <see cref="GetGroupName"/>
        /// methods to return localized values.
        /// </summary>
        public Type ResourceType
        {
            get
            {
                return this._resourceType;
            }
            set
            {
                if (this._resourceType != value)
                {
                    this._resourceType = value;

                    this._shortName.ResourceType = value;
                    this._name.ResourceType = value;
                    this._groupName.ResourceType = value;
                }
            }
        }
        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "The property and method are a matched pair")]
        public string ControllerName
        {
            get
            {
                return this._controllerName;
            }
            set
            {
                if (this._controllerName != value)
                {
                    this._controllerName = value;
                }
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Gets the UI display string for ShortName.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="ShortName"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="ShortName"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="ShortName"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="ShortName"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// <para>
        /// If <see cref="ShortName"/> is <c>null</c>, the value from <see cref="GetName"/> will be returned.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="ShortName"/> property,
        /// but a public static property with a name matching the <see cref="ShortName"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetShortName()
        {
            return this._shortName.GetLocalizableValue() ?? this.GetName();
        }

        /// <summary>
        /// Gets the UI display string for Name.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="Name"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="Name"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="Name"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="Name"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// <para>
        /// Can return <c>null</c> and will not fall back onto other values, as it's more likely for the
        /// consumer to want to fall back onto the property name.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="Name"/> property,
        /// but a public static property with a name matching the <see cref="Name"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetName()
        {
            return this._name.GetLocalizableValue();
        }

        /// <summary>
        /// Gets the UI display string for GroupName.
        /// <para>
        /// This can be either a literal, non-localized string provided to <see cref="GroupName"/> or the
        /// localized string found when <see cref="ResourceType"/> has been specified and <see cref="GroupName"/>
        /// represents a resource key within that resource type.
        /// </para>
        /// </summary>
        /// <returns>
        /// When <see cref="ResourceType"/> has not been specified, the value of
        /// <see cref="GroupName"/> will be returned.
        /// <para>
        /// When <see cref="ResourceType"/> has been specified and <see cref="GroupName"/>
        /// represents a resource key within that resource type, then the localized value will be returned.
        /// </para>
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        /// After setting both the <see cref="ResourceType"/> property and the <see cref="GroupName"/> property,
        /// but a public static property with a name matching the <see cref="GroupName"/> value couldn't be found
        /// on the <see cref="ResourceType"/>.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This method does work using a property of the same name")]
        public string GetGroupName()
        {
            return this._groupName.GetLocalizableValue();
        }

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            metadata.DisplayName = GetName();
            metadata.ShortDisplayName = GetShortName();
            metadata.AdditionalValues["List"] = GetGroupName();
            
        }
        #endregion
        private class LocalizableString
        {
            public string Value { get; set; }
            public Type ResourceType { get; set; }
            private string _metaDataKey { get; set; }
            public LocalizableString(string MetaDataKey)
            {
                _metaDataKey = MetaDataKey;
            }

            public string GetLocalizableValue()
            {
                if (Value == null)
                { return null; }
                ResourceManager mngr = new ResourceManager(ResourceType);
                return mngr.GetObject(Value) != null ? mngr.GetObject(Value).ToString() : null;
            }
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public sealed class IncludeAttribute : Attribute
    {
        public IncludeAttribute()
        {

        }
    }

    public static class T4Helpers
    {
        /// <summary>Возвращает строку с типом данных свойства модели, указанном в DataTypeAttribute</summary>
        /// <returns>строку</returns>
        /// <param name="viewDataTypeName">Класс модели</param>
        /// <param name="propertyName">Имя свойства модели</param>
        public static string GetDataTypeName(string viewDataTypeName, string propertyName)
        {
            DataTypeAttribute dataType = null;
            Type typeModel = Type.GetType(viewDataTypeName);

            if (typeModel != null)
            {
                dataType = (DataTypeAttribute)Attribute.GetCustomAttribute(typeModel.GetProperty(propertyName), typeof(DataTypeAttribute));
                if (dataType != null)
                {
                    return dataType.DataType.ToString();
                }
            }
            return "Unknown";
        }
        /// <summary>Показывает значение атрибута Hidden</summary>
        /// <returns>строку</returns>
        /// <param name="viewDataTypeName">Класс модели</param>
        /// <param name="propertyName">Имя свойства модели</param>
        public static bool IsHidden(string viewDataTypeName, string propertyName)
        {
            HiddenInputAttribute attr = null;
            Type typeModel = Type.GetType(viewDataTypeName);

            if (typeModel != null)
            {
                attr = (HiddenInputAttribute)Attribute.GetCustomAttribute(typeModel.GetProperty(propertyName), typeof(HiddenInputAttribute));
                if (attr != null)
                {
                    return !attr.DisplayValue;
                }
            }
            return false;
        }
        /// <summary>Возвращает строку с названием контроллера модели, указанном в DisplayClassAttribute</summary>
        /// <returns>строку</returns>
        /// <param name="viewDataTypeName">Класс модели</param>
        public static string GetControllerName(string viewDataTypeName)
        {
            DisplayClassAttribute dispAttr = null;
            Type typeModel = Type.GetType(viewDataTypeName);

            if (typeModel != null)
            {
                dispAttr = (DisplayClassAttribute)Attribute.GetCustomAttribute(typeModel, typeof(DisplayClassAttribute));
                if (dispAttr != null)
                {
                    return dispAttr.ControllerName ?? "@*Введите наименование контроллера";
                }
            }
            return "@*Введите наименование контроллера";
        }
        /// <summary>Возвращает список строк с именами параметров необходимых для includeExpression</summary>
        /// <returns>строку</returns>
        /// <param name="viewDataTypeName">Класс модели</param>
        public static List<string> GetIncludeProperties(string viewDataTypeName)
        {
            IncludeAttribute Attr = null;
            List<string> result = new List<string>();
            Type typeModel = Type.GetType(viewDataTypeName);

            if (typeModel != null)
            {
                PropertyInfo[] properties = typeModel.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    Attr = (IncludeAttribute)Attribute.GetCustomAttribute(property, typeof(IncludeAttribute));
                    if (Attr != null)
                    {
                        result.Add(property.Name);
                    }
                }
                return result;
            }
            throw new Exception("viewDataTypeName not found");
        }

        public static bool DelayedUpdateExists(string modelName)
        {
            Type typeModel = Type.GetType("MainOffice.Models.DelayedUpdate" + modelName);
            if (typeModel != null)
            {
                return true;
            }
            return false;
        }
        public static bool IsNullable(string viewDataTypeName, string propertyName)
        {
            Type typeModel = Type.GetType("MainOffice.Models." + viewDataTypeName);

            if (typeModel != null)
            {
                return Nullable.GetUnderlyingType(typeModel.GetProperty(propertyName).PropertyType) != null;
            }
            throw new Exception("viewDataTypeName not found");

        }
        public static bool FilterExists(string FilterModel, string FilterProperty)
        {
            Type typeModel = Type.GetType("MainOffice.Models." + FilterModel);

            if (typeModel != null)
            {
                try
                { 
                    PropertyInfo propertyInfo = typeModel.GetProperty(FilterProperty);
                    if (propertyInfo != null)
                    {
                        return true;
                    }
                }
                catch {}
                return false;
            }
            throw new Exception("viewDataTypeName not found");
        }

        public static string FilterBindAttributeIncludeText(string viewDataTypeName)
        {
            Type typeModel = Type.GetType("MainOffice.Models." + viewDataTypeName);

            if (typeModel != null)
            {
                PropertyInfo[] Properties = typeModel.GetProperties();
                string result = null;
                foreach (PropertyInfo property in Properties)
                {
                    result = result + property.Name + ",";
                }
                result = result.Remove(result.LastIndexOf(","));

                return result;
            }
            return null;
        }

        public static PropertyInfo[] GetProperties(string viewDataTypeName)
        {
            Type typeModel = Type.GetType("MainOffice.Models." + viewDataTypeName);

            if (typeModel != null)
            {
                return typeModel.GetProperties();
            }
            throw new Exception("viewDataTypeName not found");
        }

        public static IEnumerable<PropertyInfo> GetUniqueProperties(string viewDataTypeName)
        {
            return T4Helpers.GetProperties(viewDataTypeName).Where(p => p.GetCustomAttributes<IndexAttribute>().Any(f => f.Name.Contains("Unique")));
        }
        
    }
}