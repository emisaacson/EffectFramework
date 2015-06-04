namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// An interface that all fields must implement.
    /// </summary>
    public interface IField
    {
        object Value { get; set; }
        object DereferencedValue { get; }
        int? FieldID { get; }
        FieldType Type { get; }
    }
}
