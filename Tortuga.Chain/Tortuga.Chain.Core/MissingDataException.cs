namespace Tortuga.Chain;

/// <summary>
/// This exception indicates that the expected data was not found.
/// </summary>
/// <remarks>This can occur when a null or empty result set is returned from the database.</remarks>
[Serializable]
public class MissingDataException : DataException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MissingDataException"/> class.
	/// </summary>
	public MissingDataException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissingDataException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public MissingDataException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MissingDataException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
	public MissingDataException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
