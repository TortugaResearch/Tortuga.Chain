namespace Tortuga.Chain.Materializers;

partial class MasterDetailCollectionMaterializer<TCommand, TParameter, TMaster, TDetail>

{
	class DateTimeEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (DateTime)x! == (DateTime)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class DateTimeOffsetEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (DateTimeOffset)x! == (DateTimeOffset)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class GuidEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (Guid)x! == (Guid)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class Int16EqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (short)x! == (short)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class Int32EqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (int)x! == (int)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class Int64EqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (long)x! == (long)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}

	class StringEqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => string.Equals((string)x!, (string)y!, StringComparison.OrdinalIgnoreCase);

		public int GetHashCode(object obj) => ((string)obj).GetHashCode(StringComparison.OrdinalIgnoreCase);
	}

	class UInt64EqualityComparer : IEqualityComparer<object>
	{
		public new bool Equals(object? x, object? y) => (ulong)x! == (ulong)y!;

		public int GetHashCode(object obj) => obj.GetHashCode();
	}
}
