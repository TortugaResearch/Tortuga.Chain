#if !NETCOREAPP3_1_OR_GREATER

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
sealed class MaybeNullWhenAttribute : Attribute
{
	public MaybeNullWhenAttribute(bool returnValue)
	{
		ReturnValue = returnValue;
	}

	public bool ReturnValue { get; }
}

#endif
