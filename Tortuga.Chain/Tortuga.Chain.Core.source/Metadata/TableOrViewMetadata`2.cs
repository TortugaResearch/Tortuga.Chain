using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Metadata
{

    /// <summary>
    /// Metadata for a database table or view.
    /// </summary>
    /// <typeparam name="TName">The type used to represent database object names.</typeparam>
    /// <typeparam name="TDbType">The variant of DbType used by this data source.</typeparam>
    public sealed class TableOrViewMetadata<TName, TDbType> : ITableOrViewMetadata
        where TDbType : struct
    {
        private readonly ConcurrentDictionary<Tuple<Type, GetPropertiesFilter>, Lazy<ImmutableArray<ColumnPropertyMap<TDbType>>>> m_PropertyMap = new ConcurrentDictionary<Tuple<Type, GetPropertiesFilter>, Lazy<ImmutableArray<ColumnPropertyMap<TDbType>>>>();

        /// <summary>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isTable">if set to <c>true</c> [is table].</param>
        /// <param name="columns">The columns.</param>
        public TableOrViewMetadata(TName name, bool isTable, IList<ColumnMetadata<TDbType>> columns)
        {
            IsTable = isTable;
            Name = name;
            Columns = new ReadOnlyCollection<ColumnMetadata<TDbType>>(columns);
        }


        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>
        /// The columns.
        /// </value>
        public ReadOnlyCollection<ColumnMetadata<TDbType>> Columns { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is table or a view.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is a table; otherwise, <c>false</c>.
        /// </value>
        public bool IsTable { get; }

        IReadOnlyList<IColumnMetadata> ITableOrViewMetadata.Columns
        {
            get { return Columns; }
        }

        string ITableOrViewMetadata.Name
        {
            get { return Name.ToString(); }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public TName Name { get; }
        /// <summary>
        /// Creates the SQL builder.
        /// </summary>
        /// <returns></returns>
        public SqlBuilder<TDbType> CreateSqlBuilder(bool strictMode)
        {
            return new SqlBuilder<TDbType>(Name.ToString(), Columns, strictMode);
        }

        /// <summary>
        /// Gets the keys for the given dictionary that map to columns on this table or view.
        /// </summary>
        /// <param name="argument">The parameter dictionary to examine.</param>
        /// <param name="filter">The filter.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public ImmutableArray<ColumnMetadata<TDbType>> GetKeysFor(IReadOnlyDictionary<string, object> argument, GetKeysFilter filter)
        {
            //Filtered versions rely on the unfiltered version.
            IEnumerable<ColumnMetadata<TDbType>> result = Columns.Where(c => argument.ContainsKey(c.ClrName));

            var filterText = "";

            var checkCount = 0;
            if (filter.HasFlag(GetKeysFilter.PrimaryKey))
                checkCount += 1;
            if (filter.HasFlag(GetKeysFilter.NonPrimaryKey))
                checkCount += 1;
            if (checkCount > 1)
                throw new ArgumentException("Cannot specify more than one of (PrimaryKey, NonPrimaryKey)", nameof(filter));

            if (filter.HasFlag(GetKeysFilter.PrimaryKey))
            {
                filterText = "primary key";
                result = result.Where(c => c.IsPrimaryKey).ToList();

                if (filter.HasFlag(GetKeysFilter.ThrowOnMissingProperties))
                {
                    var missingProperties = Columns.Where(c => c.IsPrimaryKey && !result.Any(r => r == c)).ToList();
                    if (missingProperties.Count > 0)
                        throw new MappingException($"The parameter dictionary is missing a property mapped to the primary key column(s): " + string.Join(", ", missingProperties.Select(c => c.SqlName)) + " on table " + Name);
                }
            }
            else if (filter.HasFlag(GetKeysFilter.NonPrimaryKey))
            {
                filterText = "non-primary key";

                if (filter.HasFlag(GetKeysFilter.ThrowOnMissingColumns))
                {
                    var missingColumns = argument.Keys.Where(p => !result.Any(r => r.ClrName == p)).ToList();
                    if (missingColumns.Count > 0)
                        throw new MappingException($"The table {Name} is missing a column mapped to the properties: " + string.Join(", ", missingColumns + $". Remove the unused keys or disable strict mode."));
                }
                result = result.Where(c => !c.IsPrimaryKey).ToList();

            }

            if (filter.HasFlag(GetKeysFilter.MutableColumns))
            {
                filterText = "updateable " + filterText;
                result = result.Where(c => !c.IsComputed && !c.IsIdentity);
            }

            if (filter.HasFlag(GetKeysFilter.ThrowOnNoMatch) && !result.Any())
            {
                throw new MappingException($"None of the properties for the parameter dictionary match the {filterText} columns for {Name}");
            }

            return result.ToImmutableArray();
        }

        /// <summary>
        /// Gets the properties for the given type which map to columns on this table or view.
        /// </summary>
        /// <param name="type">The type to examine.</param>
        /// <param name="filter">The filter.</param>
        /// <remarks>This will cache and rethrow exceptions thrown by GetPropertiesForImplementation</remarks>
        public ImmutableArray<ColumnPropertyMap<TDbType>> GetPropertiesFor(Type type, GetPropertiesFilter filter)
        {
            var result = m_PropertyMap.GetOrAdd(Tuple.Create(type, filter), key =>
            {
                return new Lazy<ImmutableArray<ColumnPropertyMap<TDbType>>>(() => GetPropertiesForImplementation(key.Item1, key.Item2), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
            });

            return result.Value;
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "NotMapped")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private ImmutableArray<ColumnPropertyMap<TDbType>> GetPropertiesForImplementation(Type type, GetPropertiesFilter filter)
        {
            //The none option is used as a basis for all of the filtered versions
            if (filter == GetPropertiesFilter.None)
            {
                return (from column in Columns
                        join property in MetadataCache.GetMetadata(type).Properties
                        on column.ClrName equals property.MappedColumnName
                        select new ColumnPropertyMap<TDbType>(column, property)).ToImmutableArray();
            }

            //Filtered versions rely on the unfiltered version.
            IEnumerable<ColumnPropertyMap<TDbType>> result = GetPropertiesFor(type, GetPropertiesFilter.None);

            var filterText = "";


            var checkCount = 0;
            if (filter.HasFlag(GetPropertiesFilter.PrimaryKey))
                checkCount += 1;
            if (filter.HasFlag(GetPropertiesFilter.NonPrimaryKey))
                checkCount += 1;
            if (filter.HasFlag(GetPropertiesFilter.ObjectDefinedKey))
                checkCount += 1;
            if (filter.HasFlag(GetPropertiesFilter.ObjectDefinedNonKey))
                checkCount += 1;
            if (checkCount > 1)
                throw new ArgumentException("Cannot specify more than one of (PrimaryKey, NonPrimaryKey, ObjectDefinedKey, ObjectDefinedNonKey)", nameof(filter));

            if (filter.HasFlag(GetPropertiesFilter.PrimaryKey))
            {
                filterText = "primary key";
                result = result.Where(c => c.Column.IsPrimaryKey).ToList();

                if (filter.HasFlag(GetPropertiesFilter.ThrowOnMissingProperties))
                {
                    var missingProperties = Columns.Where(c => c.IsPrimaryKey && !result.Any(r => r.Column == c)).ToList();
                    if (missingProperties.Count > 0)
                        throw new MappingException($"The type {type.Name} is missing a property mapped to the primary key column(s): " + string.Join(", ", missingProperties.Select(c => c.SqlName)) + " on table " + Name);
                }
            }
            else if (filter.HasFlag(GetPropertiesFilter.NonPrimaryKey))
            {
                filterText = "non-primary key";

                if (filter.HasFlag(GetPropertiesFilter.ThrowOnMissingColumns))
                {
                    var missingColumns = MetadataCache.GetMetadata(type).Properties.Where(p => p.MappedColumnName != null && !result.Any(r => r.Property == p)).ToList();
                    if (missingColumns.Count > 0)
                        throw new MappingException($"The table {Name} is missing a column mapped to the properties: " + string.Join(", ", missingColumns.Select(p => p.MappedColumnName)) + $" on type {type.Name}. Use the Column and/or NotMapped attributes to correct this or disable strict mode.");
                }

                result = result.Where(c => !c.Column.IsPrimaryKey).ToList();
            }
            else if (filter.HasFlag(GetPropertiesFilter.ObjectDefinedKey))
            {
                filterText = "object defined key";
                result = result.Where(c => c.Property.IsKey).ToList();

                if (filter.HasFlag(GetPropertiesFilter.ThrowOnMissingColumns))
                {
                    var missingColumns = MetadataCache.GetMetadata(type).Properties.Where(p => p.IsKey && !result.Any(r => r.Property == p)).ToList();
                    if (missingColumns.Count > 0)
                        throw new MappingException($"The table {Name} is missing a column mapped to the key properties: " + string.Join(", ", missingColumns.Select(p => p.MappedColumnName)) + " on type " + type.Name);

                }
            }
            else if (filter.HasFlag(GetPropertiesFilter.ObjectDefinedNonKey))
            {
                filterText = "object defined non-key";
                result = result.Where(c => !c.Property.IsKey);
            }

            if (filter.HasFlag(GetPropertiesFilter.MutableColumns))
            {
                filterText = "updateable " + filterText;
                result = result.Where(c => !c.Column.IsComputed && !c.Column.IsIdentity);
            }

            if (filter.HasFlag(GetPropertiesFilter.ForInsert))
            {
                filterText = filterText + " for insert";
                result = result.Where(c => !c.Property.IgnoreOnInsert);
            }

            if (filter.HasFlag(GetPropertiesFilter.ForUpdate))
            {
                filterText = filterText + " for update";
                result = result.Where(c => !c.Property.IgnoreOnUpdate);
            }

            if (filter.HasFlag(GetPropertiesFilter.ThrowOnNoMatch) && !result.Any())
            {
                throw new MappingException($"None of the properties for {type.Name} match the {filterText} columns for {Name}");
            }

            return result.ToImmutableArray();
        }
    }

}
