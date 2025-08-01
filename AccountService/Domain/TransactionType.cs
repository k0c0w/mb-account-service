using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace AccountService.Domain;

/// <summary>
/// An enumeration of possible transaction types
/// </summary>
[DataContract]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType: ushort
{
    /// <summary>
    /// Debit transaction, account will receive money
    /// </summary>
    [EnumMember(Value="Debit")]
    Debit = 1,
    
    /// <summary>
    /// Credit transaction, account will send money
    /// </summary>
    [EnumMember(Value = "Credit")]
    Credit = 2
}