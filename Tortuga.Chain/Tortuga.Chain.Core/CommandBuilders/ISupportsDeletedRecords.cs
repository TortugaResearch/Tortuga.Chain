namespace Tortuga.Chain.CommandBuilders;

/// <summary>
/// Ignore the audit rule that normally hides deleted records.
/// </summary>
public interface ISupportsDeletedRecords
{
	/// <summary>
	/// Ignore the audit rule that normally hides deleted records.
	/// </summary>
	void InlcudeDeletedRecords();
}
