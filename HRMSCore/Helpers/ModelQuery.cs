using Local.Classes.Attributes;
using Local.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace Local.Classes.Helpers
{
	public class ModelQuery
	{
		public class TableNameAndFields
		{
			public string FriendlyName { get; set; }
			public Dictionary<string, PropertyInfo> Fields { get; set; }
		}

		private Dictionary<string, TableNameAndFields> _TableMapping;

		public Dictionary<string, TableNameAndFields> TableMapping
		{
			get
			{
				if (_TableMapping != null)
				{
					return _TableMapping;
				}

				_TableMapping = new Dictionary<string, TableNameAndFields>();

				var AllModelTypes = FindAllModelTypes();

				foreach (var ModelType in AllModelTypes)
				{
					ITableSpecifier[] TableSpecifiers = GetTableSpecifier(ModelType);
					string DisplayName = ((Model)Activator.CreateInstance(ModelType)).GetDisplayName();

					if (TableSpecifiers != null)
					{
						foreach (var TableSpecifier in TableSpecifiers)
						{
                            string tableName;
                                tableName = Regex.Replace(TableSpecifier.TableType.GetCustomAttribute<TableAttribute>().Name, "^[^.]*\\.", "");
                                if (_TableMapping.ContainsKey(tableName))
                                {
                                    // if table break out by many models - gather all in one 
                                    var additionalFields = GetFieldNames(ModelType);
                                    foreach (var additionalField in additionalFields)
                                    {
                                        _TableMapping[tableName].Fields[additionalField.Key] = additionalField.Value;
						}
					}
                                else
                                {
                                    _TableMapping[tableName] = new TableNameAndFields()
                                    {
                                        FriendlyName = DisplayName + (TableSpecifiers.Count() > 1 ?
                                                                     (TableSpecifier.GetType() == typeof(LocalTable) ? " - Local" : " - Master")
                                                                      : ""),
                                        Fields = GetFieldNames(ModelType)
                                    };
                                }
				}
					}
				}
				return _TableMapping;
			}
		}

		public string GetFriendlyName(string TableName, string FieldName)
		{
			string Output = "[";
			if (TableMapping.ContainsKey(TableName) && TableMapping[TableName].FriendlyName != null)
			{
				Output += TableMapping[TableName].FriendlyName;
			}
			else
			{
				Output += TableName;
			}
			Output += ".";
            Output += GetFriendlyNameFieldOnly(TableName, FieldName) ?? "";
			Output += "]";
			return Output;
		}

        public string GetFriendlyNameFieldOnly(string TableName, string FieldName)
        {
            string Output = null;
            try
            {
                var tableExists = TableMapping.Any(x => x.Key == TableName);
                var fieldExists = false;
                if (tableExists)
                {
                    fieldExists = TableMapping[TableName].Fields.Any(x => x.Key == FieldName);
                }
                if (tableExists && fieldExists
                    && TableMapping[TableName] != null && TableMapping[TableName].FriendlyName != null
                    && TableMapping[TableName].Fields != null && TableMapping[TableName].Fields[FieldName] != null)
                {
                    DisplayNameAttribute DisplayName = TableMapping[TableName].Fields[FieldName].GetCustomAttribute<DisplayNameAttribute>();
                    Output = DisplayName != null ? DisplayName.DisplayName : TableMapping[TableName].Fields[FieldName].Name;
                }
                else
                {
                    Output = FieldName;
                }
            }
            catch (Exception ex)
            {
                Output = FieldName; // something happened, just return the field name
            }

            return Output;
        }

        private Dictionary<string, PropertyInfo> GetFieldNames(Type TableType)
        {
            var Properties = TableType.GetProperties();
            var Output = new Dictionary<string, PropertyInfo>();

            foreach (var Property in Properties)
            {
                IColumnSpecifier ColumnSpecifier = GetColumnSpecifier(Property);
                if (ColumnSpecifier == null)
                {
                    continue;
                }

                //DisplayNameAttribute DisplayName = Property.GetCustomAttribute<DisplayNameAttribute>();
                // DisplayName != null ? DisplayName.DisplayName : Property.Name;
                Output[ColumnSpecifier.ColumnName ?? Property.Name] = Property;
            }
            return Output;
        }

		private List<Type> FindAllModelTypes()
		{
			var DerivedType = typeof(Model);
			var CurrentAssembly = Assembly.GetAssembly(typeof(Model));
			return CurrentAssembly
				.GetTypes()
				.Where(t =>
					t != DerivedType &&
					DerivedType.IsAssignableFrom(t) &&
					!t.ContainsGenericParameters
					).ToList();

		}

		private IColumnSpecifier GetColumnSpecifier(PropertyInfo Property)
		{
            if (Attribute.IsDefined(Property, typeof(HideInCustomReports)))
            {
                return null;
            }

			if (Attribute.IsDefined(Property, typeof(MasterProperty)))
			{
				return Property.GetCustomAttribute<MasterProperty>();
			}
			else if (Attribute.IsDefined(Property, typeof(LocalProperty)))
			{
				return Property.GetCustomAttribute<LocalProperty>();
			}
			else
			{
				return null;
			}
		}

		private ITableSpecifier[] GetTableSpecifier(Type ModelType)
		{
			if (Attribute.IsDefined(ModelType, typeof(LocalTable)) && Attribute.IsDefined(ModelType, typeof(MasterTable)))
			{
				return new ITableSpecifier[] { ModelType.GetCustomAttribute<MasterTable>(), ModelType.GetCustomAttribute<LocalTable>() };
			}
			if (Attribute.IsDefined(ModelType, typeof(MasterTable)))
			{
				return new ITableSpecifier[] { ModelType.GetCustomAttribute<MasterTable>() };
			}
			else if (Attribute.IsDefined(ModelType, typeof(LocalTable)))
			{
				return new ITableSpecifier[] { ModelType.GetCustomAttribute<LocalTable>() };
			}
			else
			{
				return null;
			}
		}
	}
}
