using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Materializers
{
	internal class MappedProperty<TTarget> where TTarget : class
	{
		public MappedProperty(string mappedColumnName, PropertyMetadata propertyMetadata)
		{
			MappedColumnName = mappedColumnName;
			PropertyMetadata = propertyMetadata;
		}

		public string MappedColumnName { get; }

		public PropertyMetadata PropertyMetadata { get; }

		public virtual void InvokeSet(TTarget target, object? value)
		{
			PropertyMetadata.InvokeSet(target, value);
		}
	}
}
