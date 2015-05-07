using System;

namespace EffectFramework.Core.Models.Fields
{
    public class FieldType
    {
        public int Value { get; private set; }
        public DataType DataType { get; private set; }
        public string Name { get; private set; }

        private FieldType(string Name, int Value, DataType DataType)
        {
            this.Name = Name;
            this.Value = Value;
            this.DataType = DataType;
        }

#region Field Types
        public static readonly FieldType Job_Title = new FieldType(Strings.Job_Title, 1, DataType.Text);
        public static readonly FieldType Job_Start_Date = new FieldType(Strings.Job_Start_Date, 2, DataType.Date);
        public static readonly FieldType Hire_Date = new FieldType(Strings.Hire_Date, 3, DataType.Date);
#endregion

        public static implicit operator int (FieldType dt)
        {
            return dt.Value;
        }

        public static explicit operator FieldType(int i)
        {
            switch (i)
            {
                case 1:
                    return Job_Title;
                case 2:
                    return Job_Start_Date;
                case 3:
                    return Hire_Date;
                default:
                    throw new InvalidCastException(string.Format("Cannot convert the int value {0} to an EntityFields instance.", i));
            }
        }
    }
}
