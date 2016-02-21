using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Formatters;

namespace Tortuga.Chain
{
    /// <summary>
    /// This is the base class from which all other command builders are created.
    /// </summary>
    /// <typeparam name="TCommandType">The type of the command used.</typeparam>
    /// <typeparam name="TParameterType">The type of the t parameter type.</typeparam>
    public abstract class DbCommandBuilder<TCommandType, TParameterType>
        where TCommandType : DbCommand
        where TParameterType : DbParameter
    {
        private readonly DataSource<TCommandType, TParameterType> m_DataSource;

        /// <summary>
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        protected DbCommandBuilder(DataSource<TCommandType, TParameterType> dataSource)
        {
            m_DataSource = dataSource;
        }

        /// <summary>
        /// Prepares the command for execution by generating any necessary SQL.
        /// </summary>
        /// <param name="formatter">The formatter.</param>
        /// <returns>ExecutionToken&lt;TCommandType&gt;.</returns>
        public abstract ExecutionToken<TCommandType, TParameterType> Prepare(Formatter<TCommandType, TParameterType> formatter);

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>The data source.</value>
        public DataSource<TCommandType, TParameterType> DataSource
        {
            get { return m_DataSource; }
        }


        /// <summary>
        /// Indicates the results should be formatted as a nullable Boolean.
        /// </summary>
        public BooleanOrNullResult<TCommandType, TParameterType> AsBooleanOrNull() { return new BooleanOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Boolean.
        /// </summary>
        public BooleanResult<TCommandType, TParameterType> AsBoolean() { return new BooleanResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a DataSet.
        /// </summary>
        /// <param name="tableNames">The table names.</param>
        public DataSetResult<TCommandType, TParameterType> AsDataSet(params string[] tableNames) { return new DataSetResult<TCommandType, TParameterType>(this, tableNames); }

        /// <summary>
        /// Indicates the results should be formatted as a DataSet.
        /// </summary>
        public DataTableResult<TCommandType, TParameterType> AsDataTableResult() { return new DataTableResult<TCommandType, TParameterType>(this); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable DateTimeOffset.
        /// </summary>
        public DateTimeOffsetOrNullResult<TCommandType, TParameterType> AsDateTimeOffsetOrNull() { return new DateTimeOffsetOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a DateTimeOffset.
        /// </summary>
        public DateTimeOffsetResult<TCommandType, TParameterType> AsDateTimeOffset() { return new DateTimeOffsetResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable DateTime.
        /// </summary>
        public DateTimeOrNullResult<TCommandType, TParameterType> AsDateTimeOrNull() { return new DateTimeOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a DateTime.
        /// </summary>
        public DateTimeResult<TCommandType, TParameterType> AsDateTime() { return new DateTimeResult<TCommandType, TParameterType>(this); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable Double.
        /// </summary>
        public DoubleOrNullResult<TCommandType, TParameterType> AsDoubleOrNull() { return new DoubleOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Double.
        /// </summary>
        public DoubleResult<TCommandType, TParameterType> AsDouble() { return new DoubleResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DoubleListResult<TCommandType, TParameterType> AsDoubleList(ListOptions listOptions = ListOptions.None) { return new DoubleListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of booleans.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public BooleanListResult<TCommandType, TParameterType> AsBooleanList(ListOptions listOptions = ListOptions.None) { return new BooleanListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of TimeSpan.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public TimeSpanListResult<TCommandType, TParameterType> AsTimeSpanList(ListOptions listOptions = ListOptions.None) { return new TimeSpanListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public SingleListResult<TCommandType, TParameterType> AsSingleList(ListOptions listOptions = ListOptions.None) { return new SingleListResult<TCommandType, TParameterType>(this, listOptions); }


        /// <summary>
        /// Indicates the results should be formatted as a list of numbers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DecimalListResult<TCommandType, TParameterType> AsDecimalList(ListOptions listOptions = ListOptions.None) { return new DecimalListResult<TCommandType, TParameterType>(this, listOptions); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable Int16.
        /// </summary>
        public Int16OrNullResult<TCommandType, TParameterType> AsInt16OrNull() { return new Int16OrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int16.
        /// </summary>
        public Int16Result<TCommandType, TParameterType> AsInt16() { return new Int16Result<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public Int16ListResult<TCommandType, TParameterType> AsInt16List(ListOptions listOptions = ListOptions.None) { return new Int16ListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int32.
        /// </summary>
        public Int32OrNullResult<TCommandType, TParameterType> AsInt32OrNull() { return new Int32OrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int32.
        /// </summary>
        public Int32Result<TCommandType, TParameterType> AsInt32() { return new Int32Result<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public Int32ListResult<TCommandType, TParameterType> AsInt32List(ListOptions listOptions = ListOptions.None) { return new Int32ListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a nullable Int64.
        /// </summary>
        public Int64OrNullResult<TCommandType, TParameterType> AsInt64OrNull() { return new Int64OrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Int64.
        /// </summary>
        public Int64Result<TCommandType, TParameterType> AsInt64() { return new Int64Result<TCommandType, TParameterType>(this); }


        /// <summary>
        /// Indicates the results should be formatted as a list of integers.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public Int64ListResult<TCommandType, TParameterType> AsInt64List(ListOptions listOptions = ListOptions.None) { return new Int64ListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Formats the result as an instance of the indicated type
        /// </summary>
        /// <typeparam name="TObject">The type of the object returned.</typeparam>
        /// <param name="rowOptions">The row options.</param>
        /// <returns></returns>
        public ObjectResult<TCommandType, TParameterType, TObject> AsObject<TObject>(RowOptions rowOptions = RowOptions.None)
            where TObject : class, new()
        {
            return new ObjectResult<TCommandType, TParameterType, TObject>(this, rowOptions);
        }

        /// <summary>
        /// Formats the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <returns></returns>
        public CollectionResult<TCommandType, TParameterType, TObject, List<TObject>> AsCollection<TObject>()
            where TObject : class, new()
        {
            return new CollectionResult<TCommandType, TParameterType, TObject, List<TObject>>(this);
        }

        /// <summary>
        /// Formats the result as a list of objects.
        /// </summary>
        /// <typeparam name="TObject">The type of the model.</typeparam>
        /// <typeparam name="TCollection">The type of the collection.</typeparam>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public CollectionResult<TCommandType, TParameterType, TObject, TCollection> AsCollection<TObject, TCollection>()
            where TObject : class, new()
            where TCollection : ICollection<TObject>, new()
        {
            return new CollectionResult<TCommandType, TParameterType, TObject, TCollection>(this);
        }

        /// <summary>
        /// Indicates this operation has no result set.
        /// </summary>
        /// <returns></returns>
        public NonQueryResult<TCommandType, TParameterType> AsNonQuery() { return new NonQueryResult<TCommandType, TParameterType>(this); }



        /// <summary>
        /// Indicates the results should be formatted as a Row.
        /// </summary>
        public RowResult<TCommandType, TParameterType> AsRow(RowOptions rowOptions = RowOptions.None) { return new RowResult<TCommandType, TParameterType>(this, rowOptions); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable Single.
        /// </summary>
        public SingleOrNullResult<TCommandType, TParameterType> AsSingleOrNull() { return new SingleOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Single.
        /// </summary>
        public SingleResult<TCommandType, TParameterType> AsSingle() { return new SingleResult<TCommandType, TParameterType>(this); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable string.
        /// </summary>
        /// <returns></returns>
        public StringResult<TCommandType, TParameterType> AsString() { return new StringResult<TCommandType, TParameterType>(this); }


        /// <summary>
        /// Indicates the results should be formatted as a list of strings.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public StringListResult<TCommandType, TParameterType> AsStringList(ListOptions listOptions = ListOptions.None) { return new StringListResult<TCommandType, TParameterType>(this, listOptions); }


        /// <summary>
        /// Indicates the results should be formatted as a Table.
        /// </summary>
        public TableResult<TCommandType, TParameterType> AsTable() { return new TableResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a set of tables.
        /// </summary>
        public TableSetResult<TCommandType, TParameterType> AsTableSet(params string[] tableNames) { return new TableSetResult<TCommandType, TParameterType>(this, tableNames); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable TimeSpan.
        /// </summary>
        public TimeSpanOrNullResult<TCommandType, TParameterType> AsTimeSpanOrNull() { return new TimeSpanOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a TimeSpan.
        /// </summary>
        public TimeSpanResult<TCommandType, TParameterType> AsTimeSpan() { return new TimeSpanResult<TCommandType, TParameterType>(this); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable Guid.
        /// </summary>
        public GuidOrNullResult<TCommandType, TParameterType> AsGuidOrNull() { return new GuidOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Guid.
        /// </summary>
        public GuidResult<TCommandType, TParameterType> AsGuid() { return new GuidResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a list of Guids.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public GuidListResult<TCommandType, TParameterType> AsGuidList(ListOptions listOptions = ListOptions.None) { return new GuidListResult<TCommandType, TParameterType>(this, listOptions); }


        /// <summary>
        /// Indicates the results should be formatted as a byte array.
        /// </summary>
        public ByteArrayResult<TCommandType, TParameterType> AsByteArray() { return new ByteArrayResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a list of DateTime.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DateTimeListResult<TCommandType, TParameterType> AsDateTimeList(ListOptions listOptions = ListOptions.None) { return new DateTimeListResult<TCommandType, TParameterType>(this, listOptions); }

        /// <summary>
        /// Indicates the results should be formatted as a list of DateTimeOffset.
        /// </summary>
        /// <param name="listOptions">The list options.</param>
        /// <returns></returns>
        public DateTimeOffsetListResult<TCommandType, TParameterType> AsDateTimeOffsetList(ListOptions listOptions = ListOptions.None) { return new DateTimeOffsetListResult<TCommandType, TParameterType>(this, listOptions); }


        /// <summary>
        /// Indicates the results should be formatted as a nullable Decimal.
        /// </summary>
        public DecimalOrNullResult<TCommandType, TParameterType> AsDecimalOrNull() { return new DecimalOrNullResult<TCommandType, TParameterType>(this); }

        /// <summary>
        /// Indicates the results should be formatted as a Decimal.
        /// </summary>
        public DecimalResult<TCommandType, TParameterType> AsDecimal() { return new DecimalResult<TCommandType, TParameterType>(this); }


    }
}