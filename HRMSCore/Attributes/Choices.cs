using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;

namespace HRMS.Core.Attributes
{
    /*
     * Attribute to decorate Model properties signifying two things:
     * 
     *   1) The field should be rendered on forms as a dropdown list
     *   2) Specifies, optionally, the items to display in the dropdown.
     *   
     * The choices can be specified in several ways:
     *   1) As a parameter list:
     *      [Choices("Red", "Blue", "Green")]
     *      
     *   2) As a parameter list specifying key/value pairs, delimited by a pipe |
     *      [Choices("R|Red", "B|Blue", "G|Green")]
     *      
     *   3) As a static string[] on some class, specified by a type and a field name
     *   
     *      public class Colors {
     *          public static string[] Items = new string[] { "Red", "Gree,", "Blue" };
     *      }
     *      
     *      ...snip...
     *      [Choices(typeof(Colors), "Items")]
     * 
     */
    [AttributeUsage(AttributeTargets.Property)]
    public class 
        Choices : Attribute
    {

        public KeyValuePair<string, string>[] Items { get; set; }
        private bool _NeedToEvaluateAtRunTime = false;
        private Type _ItemsType;
        private string _FieldOrProperty;

        public Choices()
        {
            this.Items = new KeyValuePair<string, string>[] { };
        }

        public Choices(params string[] Items)
        {
            this.Items = ParseOutStringChoices(Items);
        }

        // Take the array from a class
        public Choices(Type ItemsType, string FieldOrProperty)
        {

            _NeedToEvaluateAtRunTime = true;
            _ItemsType = ItemsType;
            _FieldOrProperty = FieldOrProperty;

        }

        // Generate key/value pairs if the delimiter exists
        private KeyValuePair<string, string>[] ParseOutStringChoices(string[] StringChoices)
        {
            return StringChoices.Select(Item =>
            {
                string Key, Value;
                if (Item.Contains('|'))
                {
                    var ItemSplit = Item.Split('|');
                    Key = ItemSplit[0];
                    Value = ItemSplit[1];
                }
                else
                {
                    Key = Value = Item;
                }
                return new KeyValuePair<string, string>(Key, Value);
            }).ToArray();
        }

        // Easy way to provide the items to the view.
        public KeyValuePair<string, string>[] GetOptions()
        {
            if (_NeedToEvaluateAtRunTime)
            {
                var FieldInfo = _ItemsType.GetField(_FieldOrProperty, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                if (FieldInfo != null)
                {
                    string[] StringChoices = (string[])FieldInfo.GetValue(null);

                    return ParseOutStringChoices(StringChoices);
                }
                else
                {
                    var PropInfo = _ItemsType.GetProperty(_FieldOrProperty, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

                    return (KeyValuePair<string, string>[])PropInfo.GetValue(null);
                }
            }
            else
            {
                return Items;
            }
        }
    }
}