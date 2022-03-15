namespace Tortuga.Shipwright;

[Flags]
public enum Setter
{
	None = 0,
	Protected = 1,
	Internal = 2,
	ProtectedOrInternal = 3,
	Private = 4,
	Init = 8
}
