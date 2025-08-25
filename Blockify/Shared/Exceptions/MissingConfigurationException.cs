namespace Blockify.Shared.Exceptions;

public class MissingConfigurationException : Exception
{
    public MissingConfigurationException()
        : base("There is a configuration field missing") { }

    public MissingConfigurationException(string fieldName)
        : base($"{fieldName} is missing on the configuration") { }

    public MissingConfigurationException(string fieldName, Exception? innerException)
        : base($"{fieldName} is missing on the configuration", innerException) { }
}
