using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Materializers
{
	internal class OrdinalMappedProperty<TTarget>
		where TTarget : class
	{
		readonly MappedProperty<TTarget> m_MappedProperty;

		public OrdinalMappedProperty(MappedProperty<TTarget> mappedProperty, int ordinal)
		{
			m_MappedProperty = mappedProperty;
			Ordinal = ordinal;
		}

		public string MappedColumnName => m_MappedProperty.MappedColumnName;

		public int Ordinal { get; }

		public PropertyMetadata PropertyMetadata => m_MappedProperty.PropertyMetadata;

		public void InvokeSet(TTarget target, object value) => m_MappedProperty.InvokeSet(target, value);
	}
}
