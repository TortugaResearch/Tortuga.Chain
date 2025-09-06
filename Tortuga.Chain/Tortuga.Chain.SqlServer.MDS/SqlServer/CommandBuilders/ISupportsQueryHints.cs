namespace Tortuga.Chain.SqlServer.CommandBuilders;

interface ISupportsQueryHints
{
	void AddQueryHint(string hint);
}