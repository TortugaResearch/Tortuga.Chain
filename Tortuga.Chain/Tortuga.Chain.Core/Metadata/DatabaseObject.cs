namespace Tortuga.Chain.Metadata;

/// <summary>
/// Class DatabaseObject.
/// </summary>
public abstract class DatabaseObject
{
	/// <summary>Initializes a new instance of the <see cref="DatabaseObject"/> class.</summary>
	protected DatabaseObject(string name)
	{
		if (string.IsNullOrEmpty(name))
			throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));

		Name = name;
	}

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>
	/// The name.
	/// </value>
	public string Name { get; }
}
