using System;
using System.Collections.Generic;
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
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <typeparam name="TObject">The type of the t object.</typeparam>
    /// <seealso cref="Materializer{TCommand, TParameter, TResult}" />
    /// <seealso cref="IConstructibleMaterializer{TResult}" />
    public abstract class ConstructibleMaterializer<TCommand, TParameter, TResult, TObject> : Materializer<TCommand, TParameter, TResult>, IConstructibleMaterializer<TResult>
        where TCommand : DbCommand
        where TParameter : DbParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructibleMaterializer{TCommand, TParameter, TResult, TObject}" /> class.
        /// </summary>
        /// <param name="commandBuilder">The associated operation.</param>
        protected ConstructibleMaterializer(DbCommandBuilder<TCommand, TParameter> commandBuilder) : base(commandBuilder)
        {
            ObjectMetadata = MetadataCache.GetMetadata(typeof(TObject));
        }

        /// <summary>
        /// Gets or sets the constructor signature to use when materializing an object.
        /// </summary>
        /// <value>The constructor signature.</value>
        protected IReadOnlyList<Type> ConstructorSignature { get; set; }

        /// <summary>
        /// Gets or sets the TObject metadata.
        /// </summary>
        /// <value>The object metadata.</value>
        protected ClassMetadata ObjectMetadata { get; set; }
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <param name="constructorSignature">The constructor signature.</param>
        /// <returns>ILink&lt;TResult&gt;.</returns>

        public ILink<TResult> WithConstructor(params Type[] constructorSignature)
        {
            ConstructorSignature = constructorSignature;
            return this;
        }

        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the t1.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>

        public ILink<TResult> WithConstructor<T1>()
        {
            ConstructorSignature = new[] { typeof(T1) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) };
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
            ConstructorSignature = new[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10) };
            return this;
        }
    }
}
