using System.Runtime.Serialization;

namespace AccountService.Domain;

[DataContract]
public enum TransactionType: ushort
{
    [EnumMember(Value="debit")]
    Debit = 1,
    
    [EnumMember(Value = "credit")]
    Credit = 2,
}