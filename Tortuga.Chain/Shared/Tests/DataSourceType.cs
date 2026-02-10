namespace Tests;

public enum DataSourceType
{
	Normal,
	Transactional,
	Open,
	Strict,
	SequentialAccess
#if COMMON_DB_DATA_SOURCE
	,CommonDBDataSource
#endif
}

