using System.Text;

namespace Tortuga.Shipwright;

class CodeWriter
{
	public CodeWriter()
	{
		m_ScopeTracker = new(this); //We only need one. It can be reused.
	}
	StringBuilder Content { get; } = new();
	int IndentLevel { get; set; }
	ScopeTracker m_ScopeTracker { get; } //We only need one. It can be reused.
	public void Append(string line) => Content.Append(line);

	public void AppendLine(string? line) => Content.Append(new string('\t', IndentLevel)).AppendLine(line);
	public void AppendLine() => Content.AppendLine();
	public IDisposable BeginScope(string line)
	{
		AppendLine(line);
		return BeginScope();
	}
	public IDisposable BeginScope()
	{
		Content.Append(new string('\t', IndentLevel)).AppendLine("{");
		IndentLevel += 1;
		return m_ScopeTracker;
	}

	public void EndLine() => Content.AppendLine();

	public void EndScope()
	{
		IndentLevel -= 1;
		Content.Append(new string('\t', IndentLevel)).AppendLine("}");
	}

	public void StartLine() => Content.Append(new string('\t', IndentLevel));
	public override string ToString() => Content.ToString();

	string EscapeString(string text) => text.Replace("\"", "\"\"");

	/// <summary>
	/// This is used to close code blocks and remove indents.
	class ScopeTracker : IDisposable
	{
		public ScopeTracker(CodeWriter parent)
		{
			Parent = parent;
		}
		public CodeWriter Parent { get; }

		public void Dispose()
		{
			Parent.EndScope();
		}
	}

	/// <summary>
	/// Appends the multiple lines, ensuring the correct indentation is applied to each line.
	/// </summary>
	public void AppendMultipleLines(string? content)
	{
		if (content == null)
			return;

		var lines = content.Split(new[] { "\r\n" }, StringSplitOptions.None);
		foreach (var line in lines)
			AppendLine(line);
	}
}
