namespace Tortuga.Chain.SqlServer.CommandBuilders;

interface ISupportsApproximateCount
{
	ILink<long> AsCountApproximate(string columnName);

	ILink<long> AsCountApproximate();
}
