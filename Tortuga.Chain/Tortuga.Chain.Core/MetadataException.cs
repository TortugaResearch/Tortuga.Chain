﻿namespace Tortuga.Chain.SqlServer;

/// <summary>
/// Error loading the schema metadata from the database.
/// </summary>
/// <seealso cref="Exception" />

[Serializable]
public class MetadataException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MetadataException"/> class.
	/// </summary>
	public MetadataException()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MetadataException"/> class.
	/// </summary>
	/// <param name="message">The message that describes the error.</param>
	public MetadataException(string message) : base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MetadataException"/> class.
	/// </summary>
	/// <param name="message">The error message that explains the reason for the exception.</param>
	/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
	public MetadataException(string message, Exception innerException) : base(message, innerException)
	{
	}
}
