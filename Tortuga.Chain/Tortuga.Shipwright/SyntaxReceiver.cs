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

				var useTraitAttributes = modifiedClass.GetAttributes<UseTraitAttribute>().ToList();
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
