using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Shipwright;

static class SemanticHelper
{
	/// <summary>
	/// Returns the namespace appended to the class's metadata name.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	/// <remarks>This is mostly used for generating file names.</remarks>
	[return: NotNullIfNotNull("symbol")]
	public static string? FullMetadataName(this INamedTypeSymbol? symbol)
	{
		if (symbol == null)
			return null;

		var prefix = FullNamespace(symbol);

		if (prefix != "")
			return prefix + "." + symbol.MetadataName;
		else
			return symbol.MetadataName;
	}

	/// <summary>
	/// This will return a full name if the type symbol is a IArrayTypeSymbol or INamedTypeSymbol. 
	/// Otherwise it will return the symbol's name without a namespace.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	[return: NotNullIfNotNull("symbol")]
	public static string? TryFullName(this ITypeSymbol? symbol)
	{
		if (symbol == null)
			return null;

		if (symbol is IArrayTypeSymbol ats)
			return $"{ats.ElementType.TryFullName()}[{(new string(',', ats.Rank - 1))}]";

		if (symbol is INamedTypeSymbol nts)
			return FullName(nts);

		return symbol.Name;
	}

	/// <summary>
	/// This will attempt to get the full name of the array, including its namespace and rank.
	/// If the ElementType is not a INamedTypeSymbol, then the full name may not be available.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	[return: NotNullIfNotNull("symbol")]
	public static string? TryFullName(this IArrayTypeSymbol? symbol)
	{
		if (symbol == null)
			return null;

		return $"{symbol.ElementType.TryFullName()}[{(new string(',', symbol.Rank - 1))}]";
	}

	/// <summary>
	/// This will return the XML Docs the for symbol.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <remarks>This can only return a value if the class is in the current project. XML Docs from libraries are not avaialble.</remarks>
	public static string? GetXmlDocs(this ISymbol symbol)
	{
		var xml = symbol.GetDocumentationCommentXml();
		if (xml == null)
			return null;

		var lines = xml.Split(new[] { "\r\n" }, StringSplitOptions.None);
		return string.Join("\r\n", lines.Skip(1).Take(lines.Length - 3).Select(l => @"/// " + l.Trim()));
	}

	/// <summary>
	/// Returns the full name of a method, including any type parameters. It does not inlcude parameters or the return type.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static string FullName(this IMethodSymbol symbol)
	{
		var suffix = "";
		if (symbol.Arity > 0)
		{
			suffix = CollectTypeArguments(symbol.TypeArguments);
		}

		return symbol.Name + suffix;
	}

	/// <summary>
	/// This is used to append a `?` to type name.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	static string NullableToken(this ITypeSymbol symbol)
	{
		if (symbol.IsValueType || symbol.NullableAnnotation != NullableAnnotation.Annotated)
			return "";
		return "?";
	}

	/// <summary>
	/// This will return a full name of a type, including the namespace and type arguments.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	[return: NotNullIfNotNull("symbol")]
	public static string? FullName(this INamedTypeSymbol? symbol)
	{
		if (symbol == null)
			return null;

		var prefix = FullNamespace(symbol);
		var suffix = "";
		if (symbol.Arity > 0)
		{
			suffix = CollectTypeArguments(symbol.TypeArguments);
		}

		if (prefix != "")
			return prefix + "." + symbol.Name + suffix + symbol.NullableToken();
		else
			return symbol.Name + suffix + symbol.NullableToken();
	}

	static string CollectTypeArguments(IReadOnlyList<ITypeSymbol> typeArguments)
	{
		var output = new List<string>();
		for (var i = 0; i < typeArguments.Count; i++)
		{
			switch (typeArguments[i])
			{
				case INamedTypeSymbol nts:
					output.Add(FullName(nts));
					break;
				case ITypeParameterSymbol tps:
					output.Add(tps.Name + tps.NullableToken());
					break;
				default:
					throw new NotSupportedException($"Cannot generate type name from type argument {typeArguments[i].GetType().FullName}");
			}
		}


		return "<" + string.Join(", ", typeArguments) + ">";
	}


	/// <summary>
	/// Returns the full namespace for a sumbol.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
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


	/// <summary>
	/// Returns true if the class has a parameterless constructor.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static bool HasDefaultConstructor(this INamedTypeSymbol symbol)
	{
		return symbol.Constructors.Any(c => c.Parameters.Count() == 0);
	}

	/// <summary>
	/// Returns a list of scalar properties with both a getter and setter.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static IEnumerable<IPropertySymbol> ReadWriteScalarProperties(this INamedTypeSymbol symbol)
	{
		return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanRead() && p.CanWrite() && !p.HasParameters());
	}

	/// <summary>
	/// Returns a list of scalar properties with getters.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static IEnumerable<IPropertySymbol> ReadableScalarProperties(this INamedTypeSymbol symbol)
	{
		return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanRead() && !p.HasParameters());
	}

	/// <summary>
	/// Returns a list of scalar properties with setters.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static IEnumerable<IPropertySymbol> WritableScalarProperties(this INamedTypeSymbol symbol)
	{
		return symbol.GetMembers().OfType<IPropertySymbol>().Where(p => p.CanWrite() && !p.HasParameters());
	}

	/// <summary>
	/// Returns true if the property has a getter.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static bool CanRead(this IPropertySymbol symbol) => symbol.GetMethod != null;

	/// <summary>
	/// Returns true if the property has a setter.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static bool CanWrite(this IPropertySymbol symbol) => symbol.SetMethod != null;

	/// <summary>
	/// Returns true if the property has any parameters.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static bool HasParameters(this IPropertySymbol symbol) => symbol.Parameters.Any();

	//public static IEnumerable<AttributeData> GetAttributes(this ISymbol symbol, string fullName)
	//{
	//	return symbol.GetAttributes().Where(att => att.AttributeClass.FullName() == fullName);
	//}
	//public static bool HasAttribute(this ISymbol symbol, string fullName)
	//{
	//	return symbol.GetAttributes().Any(att => att.AttributeClass.FullName() == fullName);
	//}

	/// <summary>
	/// Returns all of the attributes of a given type on the indicated symbol.
	/// </summary>
	/// <typeparam name="TAttribute"></typeparam>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static IEnumerable<AttributeData> GetAttributes<TAttribute>(this ISymbol symbol)
	{
		var fullName = typeof(TAttribute).FullName;
		return symbol.GetAttributes().Where(att => att.AttributeClass.FullName() == fullName);
	}

	/// <summary>
	/// Returns null or the single attribute of the indicated type.
	/// </summary>
	/// <typeparam name="TAttribute"></typeparam>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	/// <remarks>This will throw an error if multiple copies of the same attribute.</remarks>
	public static AttributeData? GetAttribute<TAttribute>(this ISymbol symbol)
	{
		var fullName = typeof(TAttribute).FullName;
		return symbol.GetAttributes().SingleOrDefault(att => att.AttributeClass.FullName() == fullName);
	}

	/// <summary>
	/// Returns true if the symbol has at least one instance of the indicated attribute.
	/// </summary>
	/// <typeparam name="TAttribute"></typeparam>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
	public static bool HasAttribute<TAttribute>(this ISymbol symbol)
	{
		var fullName = typeof(TAttribute).FullName;
		return symbol.GetAttributes().Any(att => att.AttributeClass.FullName() == fullName);
	}

	public static T GetNamedArgument<T>(this AttributeData attribute, string name, T defaultValue)
		where T : notnull
	{
		if (attribute == null)
			throw new ArgumentNullException(nameof(attribute), $"{nameof(attribute)} is null.");

		if (string.IsNullOrEmpty(name))
			throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));

		return (T)(attribute.NamedArguments.SingleOrDefault(x => x.Key == name).Value.Value ?? defaultValue);
	}

	public static T? GetNamedArgumentOrNull<T>(this AttributeData attribute, string name)
	{
		if (attribute == null)
			throw new ArgumentNullException(nameof(attribute), $"{nameof(attribute)} is null.");

		if (string.IsNullOrEmpty(name))
			throw new ArgumentException($"{nameof(name)} is null or empty.", nameof(name));

		return (T?)(attribute.NamedArguments.SingleOrDefault(x => x.Key == name).Value.Value);
	}

	public static string? TypeConstraintString(this IMethodSymbol symbol)
	{
		if (!symbol.IsGenericMethod)
			return null;

		return string.Join("\r\n", symbol.TypeParameters.Select(tp => TypeConstraintString(tp)).Where(tp => tp != null));
	}

	/// <summary>
	/// Returns the constraints for a type parameter, including the `where` keyword.
	/// </summary>
	/// <param name="symbol">The symbol being examined.</param>
	/// <returns></returns>
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
			factors.Add(item.TryFullName());

		if (factors.Count == 0)
			return null;
		return "where " + symbol.Name + " : " + string.Join(", ", factors);

	}
}

