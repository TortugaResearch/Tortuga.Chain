#if !NETCOREAPP3_1_OR_GREATER

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
	internal sealed class NotNullIfNotNullAttribute : Attribute
	{
		public NotNullIfNotNullAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}

		public string ParameterName { get; }
	}
}

#endif
