using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tortuga.Shipwright;
class SyntaxReceiver : ISyntaxContextReceiver
{
	public List<string> Log { get; } = new();
	public WorkItemCollection WorkItems { get; } = new();


	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{

		try
		{
			// any field with at least one attribute is a candidate for property generation
			if (context.Node is ClassDeclarationSyntax classDeclarationSyntax)
			{
				var modifiedClass = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node)!;

				//We may see the same class multiple times if it is a partial class
				if (WorkItems.Contains(modifiedClass))
					return;

				WorkItems.Add(new(modifiedClass));

				//Log.Add($"Found a class named {modifiedClass.FullName()}");
				//var attributes = modifiedClass.GetAttributes();
				//Log.Add($"    Found {attributes.Length} attributes");
				//foreach (AttributeData att in attributes)
				//{
				//	Log.Add($"   Attribute: {att.AttributeClass!.Name} Full Name: {att.AttributeClass.FullNamespace()}");
				//	foreach (var arg in att.ConstructorArguments)
				//	{
				//		Log.Add($"    ....Argument: Type='{arg.Type}' Value_Type='{arg.Value?.GetType().FullName}' Value='{arg.Value}'");

				//		if (arg.Value is INamedTypeSymbol namedArgType)
				//		{
				//			Log.Add($"    ........Found a INamedTypeSymbol named '{namedArgType}'");
				//			var members = namedArgType.GetMembers();
				//			foreach (var member in members)
				//			{
				//				if (member is IPropertySymbol property)
				//					Log.Add($"    ...........Property: {property.Name} CanRead:{property.GetMethod != null} CanWrite:{property.SetMethod != null}");
				//			}
				//		}
				//	}
				//}

				var useTraitAttributes = modifiedClass.GetAttributes<Tortuga.Shipwright.UseTraitAttribute>().ToList();
				if (useTraitAttributes.Any())
				{
					Log.Add($"Container class: {modifiedClass.FullName()}");
					foreach (var useTraitAttribute in useTraitAttributes)
					{
						var traitClass = (INamedTypeSymbol?)useTraitAttribute.ConstructorArguments[0].Value;

						if (traitClass != null)
						{
							if (WorkItems[modifiedClass].TraitClasses.Add(traitClass))
								Log.Add($"\tAdding trait: {traitClass.FullName()}");
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.Add("Error parsing syntax: " + ex.ToString());
		}
	}
}
