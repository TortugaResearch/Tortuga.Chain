﻿namespace Tortuga.Chain;

/// <summary>
/// This exception occurs when there is an internal error in the Chain library. This is a bug in Chain itself or one of its extensions.
/// </summary>
[Serializable]
public class ChainInternalException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ChainInternalException"/> class.
	/// </summary>
	public ChainInternalException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ChainInternalException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public ChainInternalException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ChainInternalException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception. If the <paramref name="innerException" /> parameter is not a null reference (Nothing in Visual Basic), the current exception is raised in a catch block that handles the inner exception.</param>
	public ChainInternalException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
