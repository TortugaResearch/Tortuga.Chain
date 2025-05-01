namespace Tortuga.Chain;

/// <summary>
/// This exception indicates that unexpected data was returned from the database.
/// </summary>
/// <remarks>This can occur when more rows or columns than expected were returned.</remarks>
[Serializable]
public class UnexpectedDataException : DataException
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UnexpectedDataException"/> class.
	/// </summary>
	public UnexpectedDataException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnexpectedDataException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public UnexpectedDataException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UnexpectedDataException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException"/> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
	public UnexpectedDataException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
