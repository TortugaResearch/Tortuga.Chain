using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Shipwright;
static class SemanticHelper
{

	[return: NotNullIfNotNull("symbol")]
	public static string? FullMetadataName(this INamedTypeSymbol? symbol)
	{
		if (symbol == null)
			return null;

		var prefix = FullNamespace(symbol);
		//var suffix = "";
		//if (symbol.Arity > 0)
		//{
		//	suffix = "<" + string.Join(", ", symbol.TypeArguments.Select(targ =>
		//	{
		//		switch (targ)
		//		{
		//			case INamedTypeSymbol nts: return FullName(nts);
		//			case ITypeParameterSymbol tps: return tps.Name;
		//		}
		//		throw new NotSupportedException($"Cannot generate type name from type argument {targ.GetType().FullName} in type {symbol.Name}");
		//	})) + ">";
		//}

		if (prefix != "")
			return prefix + "." + symbol.MetadataName;
		else
			return symbol.MetadataName;
	}

	[return: NotNullIfNotNull("symbol")]
	public static string? TryFullName(this ITypeSymbol? symbol)
	{
		if (symbol == null)
			return null;
		if (symbol is not INamedTypeSymbol nts)
			return symbol.Name;
		return FullName(nts);
	}

	public static string? GetXmlDocs(this ISymbol symbol)
	{
		var xml = symbol.GetDocumentationCommentXml();
		if (xml == null)
			return null;

		var lines = xml.Split(new[] { "\r\n" }, StringSplitOptions.None);
		return string.Join("\r\n", lines.Skip(1).Take(lines.Length - 3).Select(l => @"/// " + l.Trim()));
	}

	public static string FullName(this IMethodSymbol symbol)
	{
		var suffix = "";
		if (symbol.Arity > 0)
		{
			suffix = CollectTypeArguments(symbol.TypeArguments, symbol.TypeArgumentNullableAnnotations);
		}

		return symbol.Name + suffix;
	}

	static string NullableToken(this ITypeSymbol symbol)
	{
		if (symbol.IsValueType || symbol.NullableAnnotation != NullableAnnotation.Annotated)
			return "";
		return "?";
	}

	[return: NotNullIfNotNull("symbol")]
	public static string? FullName(this INamedTypeSymbol? symbol, NullableAnnotation nullable = NullableAnnotation.None)
	{
		if (symbol == null)
			return null;

		var prefix = FullNamespace(symbol);
		var suffix = "";
		if (symbol.Arity > 0)
		{
			suffix = CollectTypeArguments(symbol.TypeArguments, symbol.TypeArgumentNullableAnnotations);
		}

		if (prefix != "")
			return prefix + "." + symbol.Name + suffix + symbol.NullableToken();
		else
			return symbol.Name + suffix + symbol.NullableToken();
	}

	static string CollectTypeArguments(IReadOnlyList<ITypeSymbol> typeArguments, IReadOnlyList<NullableAnnotation> typeArgumentNullableAnnotations)
	{
		var output = new List<string>();
		for (var i = 0; i < typeArguments.Count; i++)
		{
			switch (typeArguments[i])
			{
				case INamedTypeSymbol nts:
					output.Add(FullName(nts, typeArgumentNullableAnnotations[i]));
					break;
				case ITypeParameterSymbol tps:
					output.Add(tps.Name + tps.NullableToken()); //(typeArgumentNullableAnnotations[i] == NullableAnnotation.Annotated ? "?" : ""));
					break;
				default:
					throw new NotSupportedException($"Cannot generate type name from type argument {typeArguments[i].GetType().FullName}");
			}
		}


		return "<" + string.Join(", ", typeArguments) + ">";
	}


	public static string FullNamespace(this ISymbol symbol)
	{
		var parts = new Stack<string>();
		INamespaceSymbol? iterator = (symbol as INamespaceSymbol) ?? symbol.ContainingNamespace;
		while (iterator != null)
		{
			if (!string.IsNullOrEmpty(iterator.Name))
				parts.Push(iterator.Name);
			iterator = iterator.ContainingNamespace;
		}
		return string.Join(".", parts);
	}


	public static bool HasDefaultConstructor(this INamedTypeSymbol symbol)
	{
		return symbol.Constructors.Any(c => c.Parameters.Count() == 0);
	}

	public static IEnumerable<IPropertySymbol> ReadWriteScalarProperties(this INamedTypeSymbol symbol)
	{
		return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanRead() && p.CanWrite() && !p.HasParameters());
	}

	public static IEnumerable<IPropertySymbol> ReadableScalarProperties(this INamedTypeSymbol symbol)
	{
		return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanRead() && !p.HasParameters());
	}

	public static IEnumerable<IPropertySymbol> WritableScalarProperties(this INamedTypeSymbol symbol)
	{
		return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanWrite() && !p.HasParameters());
	}

	public static bool CanRead(this IPropertySymbol symbol) => symbol.GetMethod != null;
	public static bool CanWrite(this IPropertySymbol symbol) => symbol.SetMethod != null;
	public static bool HasParameters(this IPropertySymbol symbol) => symbol.Parameters.Any();

	public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, string fullName)
	{
		return symbol.GetAttributes().Where(att => att.AttributeClass.FullName() == fullName);
	}
	public static bool HasAttribute(this ISymbol symbol, string fullName)
	{
		return symbol.GetAttributes().Any(att => att.AttributeClass.FullName() == fullName);
	}
	public static IEnumerable<AttributeData> GetAttributes<TAttribute>(this ISymbol symbol)
	{
		var fullName = typeof(TAttribute).FullName;
		return symbol.GetAttributes().Where(att => att.AttributeClass.FullName() == fullName);
	}
	public static AttributeData? GetAttribute<TAttribute>(this ISymbol symbol)
	{
		var fullName = typeof(TAttribute).FullName;
		return symbol.GetAttributes().SingleOrDefault(att => att.AttributeClass.FullName() == fullName);
	}

	public static bool HasAttribute<TAttribute>(this ISymbol symbol)
	{
		var fullName = typeof(TAttribute).FullName;
		return symbol.GetAttributes().Any(att => att.AttributeClass.FullName() == fullName);
	}

	public static string? TypeConstraintString(this IMethodSymbol symbol)
	{
		if (!symbol.IsGenericMethod)
			return null;

		return string.Join("\r\n", symbol.TypeParameters.Select(tp => TypeConstraintString(tp)).Where(tp => tp != null));
	}

	public static string? TypeConstraintString(this ITypeParameterSymbol symbol)
	{
		var factors = new List<string>();
		if (symbol.HasValueTypeConstraint)
			factors.Add("struct");
		else if (symbol.HasReferenceTypeConstraint)
			factors.Add("class");
		else if (symbol.HasNotNullConstraint)
			factors.Add("notnull");
		else if (symbol.HasUnmanagedTypeConstraint)
			factors.Add("unmanaged");

		if (symbol.HasConstructorConstraint)
			factors.Add("new()");

		foreach (var item in symbol.ConstraintTypes)
		{
			factors.Add(item.TryFullName());
		}

		if (factors.Count == 0)
			return null;
		return "where " + symbol.Name + " : " + string.Join(", ", factors);

	}
}

