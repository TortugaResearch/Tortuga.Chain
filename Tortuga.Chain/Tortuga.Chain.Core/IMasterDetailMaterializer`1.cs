using System.Diagnostics.CodeAnalysis;

namespace Tortuga.Chain;

/// <summary>
/// This interface denotes an materializer that allows overriding the constructor logic. This
/// includes the ability to limit the list of columns being populated.
/// </summary>
/// <typeparam name="TResult">The type of the result. This may be a TMaster or List&lt;TMaster&gt;"</typeparam>
/// <seealso cref="IMasterDetailMaterializer{TResult}"/>
public interface IMasterDetailMaterializer<TResult> : ILink<TResult>
{
	/// <summary>
	/// Excludes the properties from the list of what will be populated in the object.
	/// </summary>
	/// <param name="propertiesToOmit">The properties to omit.</param>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	IMasterDetailMaterializer<TResult> ExceptProperties(params string[] propertiesToOmit);

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3, T4>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3, T4, T5>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3, T4, T5, T6>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
	/// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
	/// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
	/// <typeparam name="T8">The type of the 8th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
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
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
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
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithDetailConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();

	/// <summary>
	/// Appends the indicated constructor onto the detail object materializer.
	/// </summary>
	/// <param name="constructorSignature">The constructor signature.</param>
	IMasterDetailMaterializer<TResult> WithDetailConstructor(params Type[] constructorSignature);

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <param name="constructorSignature">The constructor signature.</param>
	IMasterDetailMaterializer<TResult> WithMasterConstructor(params Type[] constructorSignature);

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3, T4>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3, T4, T5>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3, T4, T5, T6>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
	/// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
	/// </summary>
	/// <typeparam name="T1">The type of the 1st constructor parameter.</typeparam>
	/// <typeparam name="T2">The type of the 2nd constructor parameter.</typeparam>
	/// <typeparam name="T3">The type of the 3rd constructor parameter.</typeparam>
	/// <typeparam name="T4">The type of the 4th constructor parameter.</typeparam>
	/// <typeparam name="T5">The type of the 5th constructor parameter.</typeparam>
	/// <typeparam name="T6">The type of the 6th constructor parameter.</typeparam>
	/// <typeparam name="T7">The type of the 7th constructor parameter.</typeparam>
	/// <typeparam name="T8">The type of the 8th constructor parameter.</typeparam>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
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
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9>();

	/// <summary>
	/// Appends the indicated constructor onto the master object materializer.
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
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
	IMasterDetailMaterializer<TResult> WithMasterConstructor<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>();

	/// <summary>
	/// Limits the list of properties to populate to just the indicated list.
	/// </summary>
	/// <param name="propertiesToPopulate">The properties of the object to populate.</param>
	/// <returns>IMasterDetailConstructibleMaterializer&lt;TResult&gt;.</returns>
	IMasterDetailMaterializer<TResult> WithProperties(params string[] propertiesToPopulate);
}
