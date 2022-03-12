using System;

namespace Tortuga.Shipwright;

/// <summary>
/// Class UseTraitAttribute.
/// Implements the <see cref="Attribute" />
/// </summary>
/// <seealso cref="Attribute" />
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class UseTraitAttribute : Attribute
{
	public UseTraitAttribute(Type traitType) { TraitType = traitType; }

	public Type TraitType { get; }
}

