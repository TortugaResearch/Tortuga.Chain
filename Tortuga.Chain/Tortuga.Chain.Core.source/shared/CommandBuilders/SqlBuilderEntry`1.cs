using System;
using System.Diagnostics.CodeAnalysis;
using Tortuga.Chain.Metadata;

namespace Tortuga.Chain.CommandBuilders
{
    /// <typeparam name="TDbType">The type of the database type.</typeparam>
    /// <remarks>This is a struct because we want fast allocations and copies. Try to keep it at 16 bytes or less.</remarks>
    /// <remarks>For internal use only.</remarks>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct SqlBuilderEntry<TDbType>
        where TDbType : struct
    {
        private Flags m_Flags;

        /// <summary>
        /// Gets or sets the immutable column metadata.
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public ISqlBuilderEntryDetails<TDbType> Details { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a formal parameter for a stored procedure or table value function.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [formal parameter]; otherwise, <c>false</c>.
        /// </value>
        public bool IsFormalParameter
        {
            get { return (m_Flags & Flags.IsFormalParameter) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.IsFormalParameter;
                else
                    m_Flags = m_Flags & ~Flags.IsFormalParameter;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in insert operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if insert; otherwise, <c>false</c>.
        /// </value>
        public bool UseForInsert
        {
            get { return (m_Flags & Flags.UseForInsert) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.UseForInsert;
                else
                    m_Flags = m_Flags & ~Flags.UseForInsert;
            }
        }

        public bool UseClrNameAsAlias
        {
            get { return (m_Flags & Flags.UseClrNameAsAlias) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.UseClrNameAsAlias;
                else
                    m_Flags = m_Flags & ~Flags.UseClrNameAsAlias;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column should be treated as primary key.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is primary key; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// This can be overridden. For example, if the parameter object defines its own alternate keys.
        /// </remarks>
        public bool IsKey
        {
            get { return (m_Flags & Flags.IsKey) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.IsKey;
                else
                    m_Flags = m_Flags & ~Flags.IsKey;
            }
        }


        /// <summary>
        /// Gets or sets the value to be used when constructing parameters.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        /// <remarks>A null means this parameter's value was not set. A DBNull.Value means it is passed to the database as a null.</remarks>
        public object ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in read operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if read; otherwise, <c>false</c>.
        /// </value>
        public bool UseForRead
        {
            get { return (m_Flags & Flags.UseForRead) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.UseForRead;
                else
                    m_Flags = m_Flags & ~Flags.UseForRead;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in update operations.
        /// </summary>
        /// <value>
        ///   <c>true</c> if update; otherwise, <c>false</c>.
        /// </value>
        public bool UseForUpdate
        {
            get { return (m_Flags & Flags.UseForUpdate) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.UseForUpdate;
                else
                    m_Flags = m_Flags & ~Flags.UseForUpdate;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}"/> participates in parameter generation.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use parameter]; otherwise, <c>false</c>.
        /// </value>
        public bool UseParameter
        {
            get { return (m_Flags & Flags.UseParameter) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.UseParameter;
                else
                    m_Flags = m_Flags & ~Flags.UseParameter;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SqlBuilderEntry{TDbType}" /> participates in the second pass of parameter generation.
        /// </summary>
        /// <value><c>true</c> if [use parameter2]; otherwise, <c>false</c>.</value>
        /// <remarks>This is needed when referencing anonymous parameters.</remarks>
        public bool UseParameter2
        {
            get { return (m_Flags & Flags.UseParameter2) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.UseParameter2;
                else
                    m_Flags = m_Flags & ~Flags.UseParameter2;
            }
        }

        /// <summary>
        /// When non-null, this indicates that we want to use a table value parameter instead of a normal parameter.
        /// </summary>
        public ISqlBuilderEntryDetails<TDbType> ParameterColumn { get; set; }

        /// <summary>
        /// Gets a value indicating whether restricted from reading.
        /// </summary>
        public bool RestrictedRead
        {
            get { return (m_Flags & Flags.RestrictedRead) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.RestrictedRead;
                else
                    m_Flags = m_Flags & ~Flags.RestrictedRead;
            }
        }

        /// <summary>
        /// Gets a value indicating whether restricted from inserting.
        /// </summary>
        public bool RestrictedInsert
        {
            get { return (m_Flags & Flags.RestrictedInsert) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.RestrictedInsert;
                else
                    m_Flags = m_Flags & ~Flags.RestrictedInsert;
            }
        }

        /// <summary>
        /// Gets a value indicating whether restricted from updating.
        /// </summary>
        public bool RestrictedUpdate
        {
            get { return (m_Flags & Flags.RestrictedUpdate) > 0; }
            internal set
            {
                if (value)
                    m_Flags = m_Flags | Flags.RestrictedUpdate;
                else
                    m_Flags = m_Flags & ~Flags.RestrictedUpdate;
            }
        }

        [Flags]
        internal enum Flags : ushort //this is internal, so we can make it larger if needed
        {
            IsFormalParameter = 1,
            UseForInsert = 2,
            IsKey = 4,
            UseForRead = 8,
            UseForUpdate = 16,
            UseParameter = 32,
            RestrictedRead = 64,
            RestrictedInsert = 128,
            RestrictedUpdate = 256,

            /// <summary>
            /// This allows the parameter to be used a second time. It is needed when using anonymous parameters.
            /// </summary>
            UseParameter2 = 512,

            /// <summary>
            /// The use Clr name as alias when reading. Used for object materialization purposes.
            /// </summary>
            UseClrNameAsAlias = 1024
        }

    }
}
