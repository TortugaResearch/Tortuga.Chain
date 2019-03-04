using System;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Metadata
{
    /// <summary>
    /// Langauge options needed for code generation scenarios.
    /// </summary>
    [Flags]
    public enum NameGenerationOptions
    {
        /// <summary>
        /// No options
        /// </summary>
        None = 0,

        /// <summary>
        /// Use C# type names
        /// </summary>
        CSharp = 1,

        /// <summary>
        /// Use Visual Basic type names
        /// </summary>
        VisualBasic = 2,

        /// <summary>
        /// Use F# type names
        /// </summary>
        FSharp = 3,

        /*

        /// <summary>
        /// Use short names instead of fully qualified names. When possible, language specific names will be used (e.g. int vs System.Int32).
        /// </summary>
        ShortNames = 16,

        */

        /// <summary>
        /// If the column's nullability is unknown, assume that it is nullable.
        /// </summary>
        AssumeNullable = 4,

        /// <summary>
        /// Use the nullable reference types option from C# 8.
        /// </summary>
        NullableReferenceTypes = 8,

        /// <summary>
        /// Treat the type as nullable even if the column/parameter isn't nullable.
        /// </summary>
        /// <remarks>This is for database generated values such as identity and created date columns</remarks>
        ForceNullable = 16,

        /// <summary>
        /// Treat the type as non-nullable even if the column/parameter isn't nullable.
        /// </summary>
        /// <remarks>This is needed for legacy serializers that don't support nullable primitives.</remarks>
        ForceNonNullable = 32
    }

    /// <summary>
    /// Abstract version of ColumnMetadata.
    /// </summary>
    public abstract class ColumnMetadata
    {
        /// <summary>
        /// Gets the name used by CLR objects.
        /// </summary>
        public string ClrName { get; protected set; }

        /// <summary>
        /// Gets the CLR type that matches this column's database type.
        /// </summary>
        /// <value>
        /// The CLR type of the column or NULL if the type is unkonwn.
        /// </value>
        public Type ClrType { get; protected set; }

        /// <summary>
        /// Gets the type used by the database.
        /// </summary>
        public object DbType { get; protected set; }

        /// <summary>
        /// Gets or sets the full name of the type including max length, precision, and/or scale.
        /// </summary>
        /// <value>
        /// The full name of the type.
        /// </value>
        public string FullTypeName { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ColumnMetadata{TDbType}"/> is computed.
        /// </summary>
        /// <value>
        /// <c>true</c> if computed; otherwise, <c>false</c>.
        /// </value>
        public bool IsComputed { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this column is an identity column.
        /// </summary>
        /// <value><c>true</c> if this instance is identity; otherwise, <c>false</c>.</value>
        public bool IsIdentity { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is nullable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this column is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool? IsNullable { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this column is a primary key.
        /// </summary>
        /// <value><c>true</c> if this instance is primary key; otherwise, <c>false</c>.</value>
        public bool IsPrimaryKey { get; protected set; }

        /// <summary>
        /// Gets or sets the maximum length.
        /// </summary>
        /// <value>
        /// The maximum length.
        /// </value>
        public int? MaxLength { get; protected set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public int? Precision { get; protected set; }

        /// <summary>
        /// Gets the name used by SQL Server, quoted.
        /// </summary>
        public string QuotedSqlName { get; protected set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public int? Scale { get; protected set; }

        /// <summary>
        /// Gets the name used by SQL Server.
        /// </summary>
        public string SqlName { get; protected set; }

        /// <summary>
        /// Gets the column, formatted as a SQL variable.
        /// </summary>
        public string SqlVariableName { get; protected set; }

        /// <summary>
        /// Gets the database specific name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        public string TypeName { get; protected set; }

        /// <summary>
        /// Returns the CLR Type name suitable for code generation scenarios.
        /// </summary>
        /// <param name="options">Code generation options.</param>
        /// <returns></returns>
        public string ClrTypeName(NameGenerationOptions options)
        {
            if (options.HasFlag(NameGenerationOptions.ForceNullable) && options.HasFlag(NameGenerationOptions.ForceNonNullable))
                throw new ArgumentException("Cannot specify both ForceNullable and ForceNonNullable", nameof(options));

            var langCount = 0;
            if (options.HasFlag(NameGenerationOptions.CSharp))
                langCount += 1;
            if (options.HasFlag(NameGenerationOptions.VisualBasic))
                langCount += 1;
            if (options.HasFlag(NameGenerationOptions.FSharp))
                langCount += 1;
            if (langCount != 1)
                throw new ArgumentException("Must specify one of CSharp, FSharp, or VisualBasic");

            if (ClrType == null)
                ClrType = typeof(object);

            var metadata = MetadataCache.GetMetadata(ClrType);

            bool effectivelyNullable;

            if (options.HasFlag(NameGenerationOptions.ForceNullable))
                effectivelyNullable = true;
            else if (options.HasFlag(NameGenerationOptions.ForceNonNullable))
                effectivelyNullable = false;
            else if (IsNullable.HasValue)
                effectivelyNullable = IsNullable.Value;
            else
                effectivelyNullable = options.HasFlag(NameGenerationOptions.AssumeNullable);

            string fullTypeName;

            if (options.HasFlag(NameGenerationOptions.CSharp))
                fullTypeName = metadata.CSharpFullName;
            else if (options.HasFlag(NameGenerationOptions.VisualBasic))
                fullTypeName = metadata.VisualBasicFullName;
            else if (options.HasFlag(NameGenerationOptions.FSharp))
                fullTypeName = metadata.FSharpFullName;
            else
                fullTypeName = ClrType.Name;

            //Make the data type nullable if necessary
            if (options.HasFlag(NameGenerationOptions.CSharp) && options.HasFlag(NameGenerationOptions.NullableReferenceTypes))
            {
                //value types are handled upstream
                if (!ClrType.IsValueType && effectivelyNullable)
                    fullTypeName += "?";
            }

            return fullTypeName;
        }
    }
}
