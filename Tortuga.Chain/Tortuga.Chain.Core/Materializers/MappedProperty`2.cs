using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Materializers;

/// <summary>
/// The purpose of this class
/// </summary>
/// <typeparam name="TTarget">The type of the target.</typeparam>
/// <typeparam name="TProperty">The type of the property.</typeparam>
internal class MappedProperty<TTarget, TProperty> : MappedProperty<TTarget>
	where TTarget : class
{
	readonly Action<TTarget, TProperty> m_DelegateSetter;

	public MappedProperty(string mappedColumnName, PropertyMetadata propertyMetadata) : base(mappedColumnName, propertyMetadata)
	{
		m_DelegateSetter = PropertyMetadata.CreateDelegateSetter<TTarget, TProperty>();
	}

#nullable disable

	public override void InvokeSet(TTarget target, object value) => m_DelegateSetter(target, (TProperty)value);

#nullable restore
}
