namespace Tortuga.Chain.SqlServer.CommandBuilders;

interface ISupportsTableHints
{
	void AddTableHint(string hint);
}

interface ISupportsQueryHints
{
	void AddQueryHint(string hint);
}