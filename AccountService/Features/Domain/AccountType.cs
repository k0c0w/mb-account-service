using System.Runtime.Serialization;

namespace AccountService.Features.Domain;

[DataContract]
public enum AccountType : ushort
{
    [EnumMember(Value = "checking")]
    Checking = 1,
    [EnumMember(Value = "deposit")]
    Deposit = 2,
    [EnumMember(Value = "credit")]
    Credit = 3,
}