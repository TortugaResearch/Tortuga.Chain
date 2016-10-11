using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.AuditRules
{
    /// <summary>
    /// An immutable collection of rules.
    /// </summary>
    /// <seealso cref="IReadOnlyList{Rule}" />
    public class AuditRuleCollection : IReadOnlyList<AuditRule>
    {
        readonly ImmutableArray<AuditRule> m_List;
        readonly ImmutableArray<ValidationRule> m_Validation;

        /// <summary>
        /// Returns an empty RulesCollection.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly AuditRuleCollection Empty = new AuditRuleCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRuleCollection"/> class.
        /// </summary>
        private AuditRuleCollection()
        {
            m_List = ImmutableArray.Create<AuditRule>();
            m_Validation = ImmutableArray.Create<ValidationRule>();

            SoftDeleteForSelect = ImmutableArray.Create<SoftDeleteRule>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRuleCollection"/> class.
        /// </summary>
        /// <param name="rules">The list of rules used to build this collection.</param>
        public AuditRuleCollection(IEnumerable<AuditRule> rules)
        {
            m_List = ImmutableArray.CreateRange(rules);
            m_Validation = ImmutableArray.CreateRange(m_List.OfType<ValidationRule>());

            SoftDeleteForSelect = this.Where(r => r.AppliesWhen.HasFlag(OperationTypes.Select)).OfType<SoftDeleteRule>().ToImmutableArray();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditRuleCollection"/> class.
        /// </summary>
        /// <param name="baseRules">The list of rules to be added upon.</param>
        /// <param name="additionalRules">The additional rules.</param>
        public AuditRuleCollection(AuditRuleCollection baseRules, IEnumerable<AuditRule> additionalRules)
        {
            if (baseRules == null)
                throw new ArgumentNullException(nameof(baseRules), $"{nameof(baseRules)} is null.");
            if (additionalRules == null)
                throw new ArgumentNullException(nameof(additionalRules), $"{nameof(additionalRules)} is null.");

            m_List = baseRules.m_List.AddRange(additionalRules);
            m_Validation = ImmutableArray.CreateRange(m_List.OfType<ValidationRule>());

            SoftDeleteForSelect = this.Where(r => r.AppliesWhen.HasFlag(OperationTypes.Select)).OfType<SoftDeleteRule>().ToImmutableArray();
        }

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count
        {
            get { return m_List.Length; }
        }

        /// <summary>
        /// Gets the soft delete rules for select.
        /// </summary>
        /// <value>
        /// The soft delete for select.
        /// </value>
        internal ImmutableArray<SoftDeleteRule> SoftDeleteForSelect { get; }

        /// <summary>
        /// Gets the <see cref="AuditRule"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="AuditRule"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public AuditRule this[int index]
        {
            get { return m_List[index]; }
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<AuditRule> GetEnumerator()
        {
            return ((IEnumerable<AuditRule>)m_List).GetEnumerator();
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
        public bool UseSoftDelete(TableOrViewMetadata table)
        {
            return m_List.Where(r => r.AppliesWhen.HasFlag(OperationTypes.Delete)).OfType<SoftDeleteRule>().Any(r => table.Columns.Any(c => c.SqlName.Equals(r.ColumnName, StringComparison.OrdinalIgnoreCase)));
        }

        internal void CheckValidation(object argumentValue)
        {
            for (int i = 0; i < m_Validation.Length; i++)
                m_Validation[i].CheckValue(argumentValue);
        }

        internal IEnumerable<ColumnRule> GetRulesForColumn(string sqlName, string clrName, OperationTypes appliesWhen)
        {
            return m_List.OfType<ColumnRule>().Where(c => (c.AppliesWhen & appliesWhen) > 0 && c.ColumnName.Equals(sqlName, StringComparison.OrdinalIgnoreCase) || c.ColumnName.Equals(clrName, StringComparison.OrdinalIgnoreCase));
        }

        internal IEnumerable<RestrictColumn> GetRestrictionsForColumn(string objectName, string sqlName, string clrName)
        {
            return m_List.OfType<RestrictColumn>().Where(c =>
                (c.ObjectName == null || c.ObjectName.Equals(objectName, StringComparison.OrdinalIgnoreCase))
                &&
                (c.ColumnName.Equals(sqlName, StringComparison.OrdinalIgnoreCase) || c.ColumnName.Equals(clrName, StringComparison.OrdinalIgnoreCase)));
        }

    }
}


