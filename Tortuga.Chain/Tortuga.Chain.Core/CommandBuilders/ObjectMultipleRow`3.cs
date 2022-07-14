using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Materializers;

namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// Extension for using object returning materializers.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TParameter">The type of the parameter.</typeparam>
/// <typeparam name="TObject">The type of the returned object.</typeparam>
[SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
public struct ObjectMultipleRow<TCommand, TParameter, TObject>
		where TCommand : DbCommand
		where TParameter : DbParameter
		where TObject : class
{
	readonly MultipleRowDbCommandBuilder<TCommand, TParameter> m_CommandBuilder;

	/// <summary>
	/// Initializes a new instance of the <see cref="ObjectMultipleRow{TCommand, TParameter, TObject}" /> struct.
	/// </summary>
	/// <param name="commandBuilder">The command builder.</param>
	public ObjectMultipleRow(MultipleRowDbCommandBuilder<TCommand, TParameter> commandBuilder)
	{
		m_CommandBuilder = commandBuilder;
	}

	/// <summary>
	/// Materializes the result as a list of objects.
	/// </summary>
	/// <param name="collectionOptions">The collection options.</param>
	/// <returns>ILink&lt;List&lt;TObject&gt;&gt;.</returns>
	public ILink<List<TObject>> ToCollection(CollectionOptions collectionOptions = CollectionOptions.None)
	{
		return new CollectionMaterializer<TCommand, TParameter, TObject, List<TObject>>(m_CommandBuilder, collectionOptions);
	}

	/// <summary>
	/// Materializes the result as a list of objects.
	/// </summary>
	/// <typeparam name="TCollection">The type of the collection.</typeparam>
	/// <param name="collectionOptions">The collection options.</param>
	/// <returns>ILink&lt;TCollection&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	public ILink<TCollection> ToCollection<TCollection>(CollectionOptions collectionOptions = CollectionOptions.None)
		where TCollection : ICollection<TObject>, new()
	{
		return new CollectionMaterializer<TCommand, TParameter, TObject, TCollection>(m_CommandBuilder, collectionOptions);
	}

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public ILink<TObject> ToObject(RowOptions rowOptions = RowOptions.None)
	{
		return new ObjectMaterializer<TCommand, TParameter, TObject>(m_CommandBuilder, rowOptions);
	}

	/// <summary>
	/// Materializes the result as an instance of the indicated type
	/// </summary>
	/// <param name="rowOptions">The row options.</param>
	/// <returns></returns>
	public ILink<TObject?> ToObjectOrNull(RowOptions rowOptions = RowOptions.None)
	{
		return new ObjectOrNullMaterializer<TCommand, TParameter, TObject>(m_CommandBuilder, rowOptions);
	}
}
