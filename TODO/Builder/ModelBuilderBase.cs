using HRMS.Modules.DBModel;
using HRMS.Modules.DBModel.Local;
using HRMS.Core.Attributes;
using HRMS.Core.Helpers;
using HRMS.Core.Security;
using HRMS.Core.Extensions;
using HRMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.ComponentModel;
using HRMS.Core.Models.Employee;

namespace HRMS.Core.Builders
{
    public class ModelBuilderBase
    {
        protected virtual Model Record { get; set; }

        protected string CurrentDatabase;
        protected UserContext CurrentUser;

        protected HrmsMasterDataClassesDataContext MasterDB;
        protected LocalDataClassesDataContext LocalDB;

        protected EmployeeMaster EmployeeMaster;
        protected EmpGeneral EmployeeLocal;

        protected string CipherKey;

        public ModelBuilderBase(LocalDataClassesDataContext Local, HrmsMasterDataClassesDataContext Master)
        {
            this.MasterDB = Master;
            this.LocalDB = Local;
            this.CurrentDatabase = DBHelper.GetCurrentDatabase(true);
            this.CurrentUser = DBHelper.GetCurrentUserContext();
        }
        public ModelBuilderBase(LocalDataClassesDataContext Local, HrmsMasterDataClassesDataContext Master, string CipherKey)
            : this(Local, Master)
        {
            this.CipherKey = CipherKey;
        }


        public void SetModelProperty<SourceT, AttributeT>(PropertyInfo Property, SourceT Source) where AttributeT : Attribute, IColumnSpecifier
        {
            SetModelPropertyOn<SourceT, AttributeT>(Property, Source, Record);
        }

        public void SetModelPropertyOn<SourceT, AttributeT>(PropertyInfo Property, SourceT Source, Model Target) where AttributeT : Attribute, IColumnSpecifier
        {
            // If there's a permission on this field, check to make sure the current user has it.
            if (!UserHasPermission(Property, PermissionLevel.READ))
            {
                return;
            }

            // IF not for this entity, skip
            if (Attribute.IsDefined(Property, typeof(EntityRestriction)))
            {
                var CurrentDatabase = DBHelper.GetCurrentDatabase();
                var EntityAttribute = Property.GetCustomAttribute<EntityRestriction>(false);

                if (!EntityAttribute.Entities.Select(x => x.ToLower()).Contains(CurrentDatabase.ToLower()))
                {
                    return;
                }
            }

            var SpecifierAttribute = (AttributeT)Property.GetCustomAttribute<AttributeT>(false);
            var ColumnName = SpecifierAttribute.ColumnName ?? Property.Name;

            if (Attribute.IsDefined(Property, typeof(Encrypted)))
            {

                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(Target.GetType())[Property.Name];

                // Fetch the ReadOnlyAttribute from the descriptor. 
                Encrypted attrib = (Encrypted)descriptor.Attributes[typeof(Encrypted)];

                // Get the internal isReadOnly field from the ReadOnlyAttribute using reflection. 
                PropertyInfo isVisible = attrib.GetType().GetProperty("IsVisible");

                // Using Reflection, set the internal isReadOnly field. 
                isVisible.SetValue(attrib, !String.IsNullOrEmpty(CipherKey));
            }

            if (typeof(SourceT).GetProperty(ColumnName).PropertyType == typeof(string) &&
                typeof(SourceT).GetProperty(ColumnName).GetValue(Source) == null)
            {
                Target.GetType().GetProperty(Property.Name).SetValue(Target, "");
            }
            else
            {
                var Value = typeof(SourceT).GetProperty(ColumnName).GetValue(Source);

                if (Attribute.IsDefined(Property, typeof(PercentageField)) && Value != null)
                {
                    Value = (decimal)Value * 100.0M;
                }

                if (Attribute.IsDefined(Property, typeof(Encrypted)))
                {

                    // if we don't have CipherKey - we can't decrypt, and encrypted data will be hidden and rewrited again
                    if (String.IsNullOrEmpty(CipherKey))
                        return;
                    if (Value is String)
                    {
                        Value = CryptoHelper.Decrypt((string)Value, CipherKey);
                    }
                    else if (Value is Decimal)
                    {
                      //  Value = Decimal.Parse(Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);   //Decimal.Parse(Crypto.DecryptStringAES(Value.ToString(), CipherKey), System.Globalization.CultureInfo.InvariantCulture); // Teemporary all data stored in non encripted format //TODO in future
                       // return;
                        //Decimal? dValue = (Decimal?)Value;
                        //if (dValue.HasValue)
                        //{
                        //    //todo
                        //}
                    }


                    //Value = CryptoHelper.Decrypt((string)Value);
                }

                if (Target.GetType().GetProperty(Property.Name).PropertyType == typeof(decimal))
                {
                    Target.GetType().GetProperty(Property.Name).SetValue(Target, Decimal.Parse(Value.ToString(), System.Globalization.CultureInfo.InvariantCulture));
                }
                else
                {

                    Target.GetType().GetProperty(Property.Name).SetValue(Target, Value);
                }
            }
        }

        public void WriteModelPropertyToDB<TargetT, AttributeT>(PropertyInfo Property, TargetT Target) where AttributeT : Attribute, IColumnSpecifier
        {
            WriteModelPropertyToDBOn<TargetT, AttributeT>(Property, Record, Target);
        }

        public void WriteModelPropertyToDBOn<TargetT, AttributeT>(PropertyInfo Property, Model Source, TargetT Target) where AttributeT : Attribute, IColumnSpecifier
        {
          

            // +IF not for this entity, skip
            if (Attribute.IsDefined(Property, typeof(EntityRestriction)))
            {
                var CurrentDatabase = DBHelper.GetCurrentDatabase();
                var EntityAttribute = Property.GetCustomAttribute<EntityRestriction>(false);

                if (!EntityAttribute.Entities.Select(x => x.ToLower()).Contains(CurrentDatabase.ToLower()))
                {
                    return;
                }
            }

            // +If there's a permission on this field, check to make sure the current user has it.
            if (!UserHasPermission(Property, PermissionLevel.CREATE))
            {
                return;
            }

            var SpecifierAttribute = (AttributeT)Property.GetCustomAttribute<AttributeT>(false);
            var ColumnName = SpecifierAttribute.ColumnName ?? Property.Name;

            // format telephone number string before writing
            if (Attribute.IsDefined(Property, typeof(CustomPhoneNumberValidate)))
            {
                var countryCodeProperty = Source.GetType().GetProperties().Where(x => x.GetCustomAttributes(false)
                                                                                .OfType<CountryCode>()
                                                                                .FirstOrDefault() != null).FirstOrDefault();
                if (countryCodeProperty != null)
                {
                    string countryCodeValue = (string)countryCodeProperty.GetValue(Source);
                    if (!String.IsNullOrEmpty(countryCodeValue))
                    {
                        string Value = (string)Source.GetType().GetProperty(Property.Name).GetValue(Source);
                        if (Value != null)
                        {
                            Value = TelephoneNumberHelper.Format(Value, countryCodeValue);
                            typeof(TargetT).GetProperty(ColumnName).SetValue(Target, Value);
                            return;
                        }
                    }
                    //return;
                }
            }

            bool NullIsOK = true;
            if (Attribute.IsDefined(Target.GetType().GetProperty(ColumnName), typeof(global::System.Data.Linq.Mapping.ColumnAttribute)))
            {
                var ColumnAttribute = (global::System.Data.Linq.Mapping.ColumnAttribute)(Target.GetType().GetProperty(ColumnName).GetCustomAttributes(typeof(global::System.Data.Linq.Mapping.ColumnAttribute), false))[0];
                if (!ColumnAttribute.CanBeNull)
                {
                    NullIsOK = false;
                }
            }
            if (Property.PropertyType == typeof(string) &&
                (string)Property.GetValue(Source) == "" && NullIsOK)
            {
                typeof(TargetT).GetProperty(ColumnName).SetValue(Target, null);
            }
            else
            {
                var Value = Source.GetType().GetProperty(Property.Name).GetValue(Source);

                if (Attribute.IsDefined(Property, typeof(PercentageField)) && Value != null)
                {
                    Value = (decimal)Value / 100.0M;
                }

                if (Attribute.IsDefined(Property, typeof(Encrypted)))
                {
                    // if we don't have CipherKey -> this filed already encrypted or empty
                    if (String.IsNullOrEmpty(CipherKey))
                        return;
                    if (Property.PropertyType == typeof(string))
                    {
                        Value = Crypto.EncryptStringAES((string)Value, CipherKey);  // Teemporary all data stored in non encripted format 
                    }
                    else if (Property.PropertyType == typeof(decimal))
                    {
                        Value = Value.ToString(); //Crypto.EncryptStringAES(Value.ToString(), CipherKey); // Teemporary all data stored in non encripted format //TODO in future
                }
                }
                // temporary... when field should stores in db encrypted but in model it is decimal (price, amount)
                if (Target.GetType().GetProperty(ColumnName).PropertyType == typeof(string) && Property.PropertyType == typeof(decimal)) //&&
                {
                    //Type underLyingType = Nullable.GetUnderlyingType(Property.PropertyType);
                    //if (underLyingType == typeof(decimal))
                    //{
                    typeof(TargetT).GetProperty(ColumnName).SetValue(Target, Value.ToString());
                    //}
                }
                else
                {
                    typeof(TargetT).GetProperty(ColumnName).SetValue(Target, Value);
                }
            }
        }

        public bool UserHasPermission(PropertyInfo Property, PermissionLevel Level)
        {

            if ((Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(ModelCollection<>)) || Property.PropertyType == typeof(JobAssignmentsCollection)
                || Property.PropertyType == typeof(ContactsCollection) || Property.PropertyType == typeof(AddressCollection))
            {
                var props = Property.PropertyType.GetProperties().First(x => x.Name == "Items").PropertyType.GetGenericArguments()[0].GetProperties();
                return props.Any(x => DBHelper.GetCurrentUserContext().HasPermissionTo(x, Level));
            }

            return DBHelper.GetCurrentUserContext().HasPermissionTo(Property, Level);
        }
    }
}
