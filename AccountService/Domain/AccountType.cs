using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace AccountService.Domain;

/// <summary>
/// An enumeration of possible account types
/// </summary>
[DataContract]
[JsonConverter(typeof(JsonStringEnumConverter))]

public enum AccountType : ushort
{
    /// <summary>
    /// Account allows to withdraw and deposit money at any time
    /// </summary>
    [EnumMember(Value = "Checking")]
    Checking = 1,
    /// <summary>
    /// Account allows to accumulate money
    /// </summary>
    [EnumMember(Value = "Deposit")]
    // Resharper disable once. Value is used by requests and domain.
    [UsedImplicitly]
    Deposit = 2,
    /// <summary>
    /// Account registers credit of money, which must be returned later
    /// </summary>
    [EnumMember(Value = "Credit")]
    // Resharper disable once. Value is used by requests and domain.
    [UsedImplicitly]
    Credit = 3
}