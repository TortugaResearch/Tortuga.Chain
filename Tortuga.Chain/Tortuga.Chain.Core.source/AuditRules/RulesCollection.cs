using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.AuditRules
{
    /// <summary>
    /// An immutable collection of rules.
    /// </summary>
    /// <seealso cref="IReadOnlyList{Rule}" />
    public class RulesCollection : IReadOnlyList<Rule>
    {
        private readonly ImmutableArray<Rule> m_List;

        /// <summary>
        /// Returns an empty RulesCollection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly RulesCollection Empty = new RulesCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesCollection"/> class.
        /// </summary>
        private RulesCollection()
        {
            m_List = ImmutableArray.Create<Rule>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesCollection"/> class.
        /// </summary>
        /// <param name="rules">The list of rules used to build this colleciton.</param>
        public RulesCollection(IEnumerable<Rule> rules)
        {
            m_List = ImmutableArray.CreateRange(rules);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RulesCollection"/> class.
        /// </summary>
        /// <param name="baseRules">The list of rules to be added upon.</param>
        /// <param name="additionalRules">The additional rules.</param>
        public RulesCollection(RulesCollection baseRules, IEnumerable<Rule> additionalRules)
        {
            if (baseRules == null)
                throw new ArgumentNullException(nameof(baseRules), $"{nameof(baseRules)} is null.");
            if (additionalRules == null)
                throw new ArgumentNullException(nameof(additionalRules), $"{nameof(additionalRules)} is null.");

            m_List = baseRules.m_List.AddRange(additionalRules);
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return m_List.Length; }
        }

        /// <summary>
        /// Gets the <see cref="Rule"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="Rule"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public Rule this[int index]
        {
            get { return m_List[index]; }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Rule> GetEnumerator()
        {
            return ((IEnumerable<Rule>)m_List).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_List).GetEnumerator();
        }

        /// <summary>
        /// Indicates whether or not a soft delete should be performed.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public bool UseSoftDelete(ITableOrViewMetadata table)
        {
            return m_List.Where(r => r.AppliesWhen.HasFlag(OperationTypes.Delete)).OfType<SoftDeleteRule>().Any(r => table.Columns.Any(c => c.SqlName.Equals(r.ColumnName, StringComparison.OrdinalIgnoreCase)));
        }

        internal void CheckValidation(object argumentValue)
        {
            foreach (var item in m_List.OfType<ValidationRule>())
                item.CheckValue(argumentValue);
        }

        internal IEnumerable<ColumnRule> GetRulesForColumn(string sqlName, string clrName, OperationTypes appliesWhen)
        {
            return m_List.OfType<ColumnRule>().Where(c => (c.AppliesWhen & appliesWhen) > 0 && c.ColumnName.Equals(sqlName, StringComparison.OrdinalIgnoreCase) || c.ColumnName.Equals(clrName, StringComparison.OrdinalIgnoreCase));
        }
    }
}


