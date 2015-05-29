namespace EffectFramework.Core.Models.Fields
{
    /// <summary>
    /// A simple interface that all fields must implement.
    /// </summary>
    public interface IField
    {
        object Value { get; set; }
        int? FieldID { get; }
        FieldType Type { get; }
    }
}
