﻿#if !NETCOREAPP3_1_OR_GREATER

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
sealed class NotNullWhenAttribute : Attribute
{
	public NotNullWhenAttribute(bool returnValue)
	{
		ReturnValue = returnValue;
	}

	public bool ReturnValue { get; }
}

#endif
