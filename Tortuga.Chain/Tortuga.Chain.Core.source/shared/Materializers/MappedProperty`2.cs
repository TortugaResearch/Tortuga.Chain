using System;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Materializers
{
    /// <summary>
    /// The purpose of this class 
    /// </summary>
    /// <typeparam name="TTarget">The type of the target.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    internal class MappedProperty<TTarget, TProperty> : MappedProperty<TTarget>
    {


        public MappedProperty(string mappedColumnName, PropertyMetadata propertyMetadata) : base(mappedColumnName, propertyMetadata)
        {
#if !NETSTANDARD1_3
            //TODO: Change Anchor so that it can build these strongly typed delegates
            var propertyInfo = typeof(TTarget).GetProperty(propertyMetadata.Name);
            var reflectionSetter = propertyInfo.GetSetMethod();

            m_DelegateSetter = (Action<TTarget, TProperty>)Delegate.CreateDelegate(typeof(Action<TTarget, TProperty>), reflectionSetter);
#endif
        }

#if !NETSTANDARD1_3
        Action<TTarget, TProperty> m_DelegateSetter;

        public override void InvokeSet(TTarget target, object value) => m_DelegateSetter(target, (TProperty)value);
#endif
    }
}
