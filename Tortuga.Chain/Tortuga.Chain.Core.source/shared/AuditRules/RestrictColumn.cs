namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// This rule is used to prevent users from reading or updating a column
    /// </summary>
    public class RestrictColumn : AuditRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictColumn"/> class.
        /// </summary>
        /// <param name="objectName">Name of the database object this rule applies to.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="appliesWhen">While operations are being restricted.</param>
        /// <param name="exceptWhen">This function will return true if the rule doesn't apply to this user..</param>
        public RestrictColumn(string objectName, string columnName, OperationTypes appliesWhen, ExceptWhenPredicate exceptWhen) : base(appliesWhen)
        {
            ColumnName = columnName;
            ObjectName = objectName;
            ExceptWhen = exceptWhen;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictColumn"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="appliesWhen">While operations are being restricted.</param>
        /// <param name="exceptWhen">This function will return true if the rule doesn't apply to this user..</param>
        public RestrictColumn(string columnName, OperationTypes appliesWhen, ExceptWhenPredicate exceptWhen) : base(appliesWhen)
        {
            ColumnName = columnName;
            ExceptWhen = exceptWhen;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the except when.
        /// </summary>
        /// <value>
        /// The except when.
        /// </value>
        public ExceptWhenPredicate ExceptWhen { get; }

        /// <summary>
        /// Gets the name of the object (e.g. table, view). 
        /// </summary>
        /// <value>
        /// The name of the object.
        /// </value>
        public string ObjectName { get; }
    }



}

