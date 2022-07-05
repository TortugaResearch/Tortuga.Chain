namespace Tortuga.Chain.Metadata;

/// <summary>
/// Language options needed for code generation scenarios.
/// </summary>
[Flags]
public enum NameGenerationOptions
{
	/// <summary>
	/// No options
	/// </summary>
	None = 0,

	/// <summary>
	/// Use C# type names
	/// </summary>
	CSharp = 1,

	/// <summary>
	/// Use Visual Basic type names
	/// </summary>
	VisualBasic = 2,

	/// <summary>
	/// Use F# type names
	/// </summary>
	FSharp = 4,

	/*

        /// <summary>
        /// Use short names instead of fully qualified names. When possible, language specific names will be used (e.g. int vs System.Int32).
        /// </summary>
        ShortNames = 8,

        */

	/// <summary>
	/// If the column's nullability is unknown, assume that it is nullable.
	/// </summary>
	AssumeNullable = 16,

	/// <summary>
	/// Use the nullable reference types option from C# 8.
	/// </summary>
	NullableReferenceTypes = 32,

	/// <summary>
	/// Treat the type as nullable even if the column/parameter isn't nullable.
	/// </summary>
	/// <remarks>This is for database generated values such as identity and created date columns</remarks>
	ForceNullable = 64,

	/// <summary>
	/// Treat the type as non-nullable even if the column/parameter isn't nullable.
	/// </summary>
	/// <remarks>This is needed for legacy serializers that don't support nullable primitives.</remarks>
	ForceNonNullable = 128
}
