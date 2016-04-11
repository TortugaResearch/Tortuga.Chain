namespace Tortuga.Chain.AuditRules
{
    /// <summary>
    /// Base class for all rules.
    /// </summary>
    abstract public class Rule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="appliesWhen">The rule applies when.</param>
        protected Rule(OperationType appliesWhen)
        {
            AppliesWhen = appliesWhen;
        }


        /// <summary>
        /// Indicates when the rule is applicable.
        /// </summary>
        /// <value>
        /// The rule applies when.
        /// </value>
        public OperationType AppliesWhen { get; }

    }




}