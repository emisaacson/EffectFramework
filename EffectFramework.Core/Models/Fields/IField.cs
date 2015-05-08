namespace EffectFramework.Core.Models.Fields
{
    public interface IField
    {
        object Value { get; set; }
        int? FieldID { get; }
        FieldType Type { get; }
    }
}
