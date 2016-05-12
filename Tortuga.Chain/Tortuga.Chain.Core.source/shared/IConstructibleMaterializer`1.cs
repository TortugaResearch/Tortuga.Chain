using System;
using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain
{
    /// <summary>
    /// This interface denotes an materializer that allows overriding the constructor logic.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <seealso cref="ILink{TResult}" />
    public interface IConstructibleMaterializer<TResult> : ILink<TResult>
    {
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <param name="constructorSignature">The constructor signature.</param>
        ILink<TResult> WithConstructor(params Type[] constructorSignature);


        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3, T4>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
        /// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3, T4, T5>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
        /// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
        /// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
        /// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
        /// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
        /// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
        /// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
        /// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
        /// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
        /// <typeparam name="T8">The type of the 8th constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7, T8>();
        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
        /// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
        /// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
        /// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
        /// <typeparam name="T8">The type of the 8th constructor parameter.</typeparam>
        /// <typeparam name="T9">The type of the 9th constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>();


        /// <summary>
        /// Appends the indicated constructor onto the materializer.
        /// </summary>
        /// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
        /// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
        /// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
        /// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
        /// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
        /// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
        /// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
        /// <typeparam name="T8">The type of the 8th constructor parameter.</typeparam>
        /// <typeparam name="T9">The type of the 9th constructor parameter.</typeparam>
        /// <typeparam name="T10">The type of the 10th constructor parameter.</typeparam>
        /// <returns>ILink&lt;TResult&gt;.</returns>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        ILink<TResult> WithConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();
    }
}
