using System.Data.SqlTypes;
using Blockify.Domain.Entities;

namespace Blockify.Application.Exceptions;

public class FailedJsonSerializationException<T> : Exception
{
    public string OriginalJson { get; }
    public string ModelStructureJson { get; } = JsonMapper<T>.ToJson();

    public FailedJsonSerializationException(string message)
        : this(message, string.Empty, null) { }

    public FailedJsonSerializationException(string message, string json)
        : this(message, json, null) { }

    public FailedJsonSerializationException(string message, string json, Exception? innerException)
        : base(message, innerException)
    {
        OriginalJson = json;
    }
}