using AstraID.Domain.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AstraID.Domain.Entities;

/// <summary>
/// Represents a persisted ASP.NET Core data protection key.
/// </summary>
[Table("DataProtectionKeys", Schema = "auth")]
public sealed class DataProtectionKey : Entity<int>
{
    /// <summary>
    /// Friendly display name for the key.
    /// </summary>
    [MaxLength(200)]
    public string FriendlyName { get; private set; } = string.Empty;

    /// <summary>
    /// Key material stored as XML.
    /// </summary>
    public string Xml { get; private set; } = string.Empty;

    private DataProtectionKey()
    {
    }

    /// <summary>
    /// Initializes a new key instance.
    /// </summary>
    public DataProtectionKey(int id, string friendlyName, string xml) : base(id)
    {
        if (string.IsNullOrWhiteSpace(friendlyName))
            throw new ArgumentException("Friendly name cannot be empty.", nameof(friendlyName));
        if (string.IsNullOrWhiteSpace(xml))
            throw new ArgumentException("XML cannot be empty.", nameof(xml));

        FriendlyName = friendlyName;
        Xml = xml;
    }
}
