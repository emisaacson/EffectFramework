using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using PhoneNumbers;
using HRMS.Core.Models.Employee;
using System.Reflection;

namespace HRMS.Core.Attributes
{
    public class CustomPhoneNumberValidate :  ValidationAttribute
    {
        private string phoneCountryCodePropertyName;

        /// <summary>
        ///  Sets to a phone number property in a model
        /// </summary>
        /// <param name="phoneCountryCodePropertyName"> name of property which is responsible for country code in a model</param>
        public CustomPhoneNumberValidate(string phoneCountryCodePropertyName)
        {
            this.phoneCountryCodePropertyName = phoneCountryCodePropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var countryCodeProperty = validationContext.ObjectType.GetProperty(phoneCountryCodePropertyName);
            bool isRequired = validationContext.ObjectType.GetCustomAttributes(true)
                                                           .OfType<RequiredAttribute>()
                                                           .FirstOrDefault() != null;
            if (countryCodeProperty == null)
            {
                return new ValidationResult(string.Format(
                    CultureInfo.CurrentCulture,
                    "Model missing country"
                ));
            }

            // if contact model, then need to check type property
            if(validationContext.ObjectType == typeof(ContactModel))
            {
                var typePropery = validationContext.ObjectType.GetProperty("Type");
                int? typeValue = (int?)typePropery.GetValue(validationContext.ObjectInstance);
                if(typeValue.HasValue)
                {
                    if(typeValue!=3 && typeValue!=7 && typeValue!=9)
                    {
                        // no need to validate other type of contact data
                        return null;
                    }
                    
                }
            }
            string phoneNumber = (string)value;
            string countryCode =(string) countryCodeProperty.GetValue(validationContext.ObjectInstance, null);
            if ((countryCode == null || String.IsNullOrEmpty(countryCode)) )
            {
                if (!String.IsNullOrEmpty(phoneNumber))
                {
                    return new ValidationResult(string.Format(
                       CultureInfo.CurrentCulture,
                       "Please select country"
                   ));
                }
            }
            
            if(String.IsNullOrEmpty(phoneNumber))
            {
                return null;
            }

            PhoneNumberUtil PNUtil = PhoneNumberUtil.GetInstance();
            PhoneNumbers.PhoneNumber ProcessedNumber;
            try
            {
                ProcessedNumber = PNUtil.Parse(phoneNumber, countryCode);
            }
            catch (NumberParseException e)
            {
                return new ValidationResult(string.Format(
                   CultureInfo.CurrentCulture,
                   "This phone number did not validate "
               ));
            }

            if(!PNUtil.IsValidNumber(ProcessedNumber))
            {
                return new ValidationResult(string.Format(
                   CultureInfo.CurrentCulture,
                   "This phone number did not validate "
               ));
            }
            if(PNUtil.GetRegionCodeForNumber(ProcessedNumber)!=countryCode)
            {
                return new ValidationResult(string.Format(
                   CultureInfo.CurrentCulture,
                   "This is not a valid phone number for the selected country."
               ));
            }
            
            return null;
        }
     
    }
}