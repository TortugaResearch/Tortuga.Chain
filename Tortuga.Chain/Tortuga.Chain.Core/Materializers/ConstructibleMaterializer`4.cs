using System.Data.Common;
using Tortuga.Anchor.Metadata;
using Tortuga.Chain.CommandBuilders;

namespace Tortuga.Chain.Materializers
{
	/// <summary>
	/// Class ConstructibleMaterializer.
	/// </summary>
	/// <typeparam name="TCommand">The type of the t command.</typeparam>
	/// <typeparam name="TParameter">The type of the t parameter.</typeparam>
	/// <typeparam name="TResult">The type of the result. This may be a collection of TObject.</typeparam>
	/// <typeparam name="TObject">The type of the object that will be returned.</typeparam>
	/// <seealso cref="Materializer{TCommand, TParameter, TResult}"/>
	/// <seealso cref="IConstructibleMaterializer{TResult}"/>
	public abstract class ConstructibleMaterializer<TCommand, TParameter, TResult, TObject> : Materializer<TCommand, TParameter, TResult>, IConstructibleMaterializer<TResult>
		where TCommand : DbCommand
		where TParameter : DbParameter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructibleMaterializer{TCommand,
		/// TParameter, TResult, TObject}"/> class.
		/// </summary>
		/// <param name="commandBuilder">The associated operation.</param>
		protected ConstructibleMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder) : base(commandBuilder)
		{
			ObjectMetadata = MetadataCache.GetMetadata(typeof(TObject));
		}

		/// <summary>
		/// Sets the constructor using a signature.
		/// </summary>
		/// <param name="signature">The constructor signature.</param>
		/// <exception cref="MappingException">Cannot find a matching constructor for the desired type</exception>
		protected void SetConstructor(IReadOnlyList<Type>? signature)
		{
			if (signature == null)
			{
				Constructor = null;
			}
			else
			{
				var constructor = ObjectMetadata.Constructors.Find(signature);
				if (constructor == null)
				{
					var types = string.Join(", ", signature.Select(t => t.Name));
					throw new MappingException($"Cannot find a constructor on {typeof(Type).Name} with the types [{types}]");
				}
				Constructor = constructor;
			}
		}

		/// <summary>
		/// Gets or sets the data reader constructor.
		/// </summary>
		/// <value>The data reader constructor.</value>
		protected ConstructorMetadata? Constructor { get; set; }

		/// <summary>
		/// Columns to ignore when generating the list of desired columns.
		/// </summary>
		/// <value>The excluded columns.</value>
		protected IReadOnlyList<string>? ExcludedColumns { get; private set; }

		/// <summary>
		/// Only include the indicated columns when generating the list of desired columns.
		/// </summary>
		/// <value>The included columns.</value>
		protected IReadOnlyList<string>? IncludedColumns { get; private set; }

		/// <summary>
		/// Gets or sets the TObject metadata.
		/// </summary>
		/// <value>The object metadata.</value>
		protected ClassMetadata ObjectMetadata { get; }

		/// <summary>
		/// Returns the list of columns the result materializer would like to have.
		/// </summary>
		/// <returns>IReadOnlyList&lt;System.String&gt;.</returns>
		/// <exception cref="MappingException">
		/// Cannot find a constructor on {desiredType.Name} with the types [{types}]
		/// </exception>
		/// <remarks>
		/// If AutoSelectDesiredColumns is returned, the command builder is allowed to choose which
		/// columns to return. If NoColumns is returned, the command builder should omit the
		/// SELECT/OUTPUT clause.
		/// </remarks>
		public override IReadOnlyList<string> DesiredColumns()
		{
			if (Constructor == null)
			{
				if (IncludedColumns != null && ExcludedColumns != null)
					throw new InvalidOperationException("Cannot specify both included and excluded columns/properties.");

				if (IncludedColumns != null)
					return IncludedColumns;

				IReadOnlyList<string> result = ObjectMetadata.ColumnsFor;
				if (ExcludedColumns != null)
					result = result.Where(x => !ExcludedColumns.Contains(x)).ToList();
				return result;
			}

			if (IncludedColumns != null)
				throw new NotImplementedException("Cannot specify included columns/properties with constructors. See #295");

			if (ExcludedColumns != null)
				throw new InvalidOperationException("Cannot specify excluded columns/properties with constructors.");

			return Constructor.ParameterNames;
		}

		/// <summary>
		/// Excludes the properties from the list of what will be populated in the object.
		/// </summary>
		/// <param name="propertiesToOmit">The properties to omit.</param>
		public ILink<TResult> ExceptProperties(params string[] propertiesToOmit)
		{
			if (propertiesToOmit == null || propertiesToOmit.Length == 0)
				return this;

			var result = new List<string>(propertiesToOmit.Length);
			var meta = MetadataCache.GetMetadata<TObject>();
			foreach (var propertyName in propertiesToOmit)
			{
				if (meta.Properties.TryGetValue(propertyName, out var property))
				{
					if (property.MappedColumnName != null)
						result.Add(property.MappedColumnName);
				}
			}

			ExcludedColumns = result;
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <param name="constructorSignature">The constructor signature.</param>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor(params Type[] constructorSignature)
		{
			SetConstructor(constructorSignature);
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1>()
		{
			SetConstructor(new[] { typeof(T1) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <typeparam name="T4">The type of the t4.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3, T4>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <typeparam name="T4">The type of the t4.</typeparam>
		/// <typeparam name="T5">The type of the t5.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3, T4, T5>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <typeparam name="T4">The type of the t4.</typeparam>
		/// <typeparam name="T5">The type of the t5.</typeparam>
		/// <typeparam name="T6">The type of the t6.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <typeparam name="T4">The type of the t4.</typeparam>
		/// <typeparam name="T5">The type of the t5.</typeparam>
		/// <typeparam name="T6">The type of the t6.</typeparam>
		/// <typeparam name="T7">The type of the t7.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <typeparam name="T4">The type of the t4.</typeparam>
		/// <typeparam name="T5">The type of the t5.</typeparam>
		/// <typeparam name="T6">The type of the t6.</typeparam>
		/// <typeparam name="T7">The type of the t7.</typeparam>
		/// <typeparam name="T8">The type of the t8.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7, T8>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <typeparam name="T4">The type of the t4.</typeparam>
		/// <typeparam name="T5">The type of the t5.</typeparam>
		/// <typeparam name="T6">The type of the t6.</typeparam>
		/// <typeparam name="T7">The type of the t7.</typeparam>
		/// <typeparam name="T8">The type of the t8.</typeparam>
		/// <typeparam name="T9">The type of the t9.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) });
			return this;
		}

		/// <summary>
		/// Appends the indicated constructor onto the materializer.
		/// </summary>
		/// <typeparam name="T1">The type of the t1.</typeparam>
		/// <typeparam name="T2">The type of the t2.</typeparam>
		/// <typeparam name="T3">The type of the t3.</typeparam>
		/// <typeparam name="T4">The type of the t4.</typeparam>
		/// <typeparam name="T5">The type of the t5.</typeparam>
		/// <typeparam name="T6">The type of the t6.</typeparam>
		/// <typeparam name="T7">The type of the t7.</typeparam>
		/// <typeparam name="T8">The type of the t8.</typeparam>
		/// <typeparam name="T9">The type of the t9.</typeparam>
		/// <typeparam name="T10">The type of the T10.</typeparam>
		/// <returns>ILink&lt;TResult&gt;.</returns>

		public ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
		{
			SetConstructor(new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) });
			return this;
		}

		/// <summary>
		/// Limits the list of properties to populate to just the indicated list.
		/// </summary>
		/// <param name="propertiesToPopulate">The properties of the object to populate.</param>
		/// <returns>ILink&lt;TResult&gt;.</returns>
		public ILink<TResult> WithProperties(params string[] propertiesToPopulate)
		{
			if (propertiesToPopulate == null || propertiesToPopulate.Length == 0)
				return this;

			var result = new List<string>(propertiesToPopulate.Length);
			var meta = MetadataCache.GetMetadata<TObject>();
			foreach (var propertyName in propertiesToPopulate)
			{
				if (meta.Properties.TryGetValue(propertyName, out var property))
				{
					if (property.MappedColumnName != null)
						result.Add(property.MappedColumnName);
				}
			}

			IncludedColumns = result;
			return this;
		}
	}
}
