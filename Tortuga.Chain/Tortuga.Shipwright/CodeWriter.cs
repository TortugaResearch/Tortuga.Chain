using System.Text;

namespace Tortuga.Shipwright;

/// <summary>
/// This class is used to collect the generated source code prior to writing it to a file.
/// </summary>
class CodeWriter
{
	readonly StringBuilder m_Content = new();

	/// <summary>
	/// We only need one. It can be reused.
	/// </summary>
	readonly ScopeTracker m_ScopeTracker;

	/// <summary>
	/// Indent level is used to prepend tabs to each line.
	/// </summary>
	int m_IndentLevel;

	public CodeWriter()
	{
		m_ScopeTracker = new(this); //We only need one. It can be reused.
	}
	/// <summary>
	/// Append text to the current line. This will not create a new line. This will not honor the indentation level.
	/// </summary>
	/// <param name="text"></param>
	/// <remarks>Call StartLine before using this function.</remarks>
	public void Append(string text) => m_Content.Append(text);

	/// <summary>
	/// Indents the current line, appends the provided text, and creates a new line.
	/// </summary>
	/// <param name="text"></param>
	public void AppendLine(string text) => m_Content.Append(new string('\t', m_IndentLevel)).AppendLine(text);

	/// <summary>
	/// Append a new line.
	/// </summary>
	public void AppendLine() => m_Content.AppendLine();

	/// <summary>
	/// Appends the multiple lines, ensuring the correct indentation is applied to each line.
	/// </summary>
	public void AppendMultipleLines(string? text)
	{
		if (text == null)
			return;

		var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
		foreach (var line in lines)
			AppendLine(line);
	}

	/// <summary>
	/// Appends the provided text and a new line. The appends an opening brace and increased the indentation level. 
	/// </summary>
	/// <param name="text">Text to append before starting the block.</param>
	/// <returns>An IDisposable marker that can be used to close the block.</returns>
	public IDisposable BeginScope(string text)
	{
		AppendLine(text);

		m_Content.Append(new string('\t', m_IndentLevel)).AppendLine("{");
		m_IndentLevel += 1;
		return m_ScopeTracker;
	}

	public void EndScope()
	{
		m_IndentLevel -= 1;
		m_Content.Append(new string('\t', m_IndentLevel)).AppendLine("}");
	}

	/// <summary>
	/// Indents the current line, optioanlly adding the provided text.
	/// </summary>
	/// <param name="text">Text to append.</param>
	/// <remarks>This is normally used in conjuction with Append.</remarks>
	public void StartLine(string? text = null) => m_Content.Append(new string('\t', m_IndentLevel)).Append(text);


	/// <summary>Returns a string that represents the current object.</summary>
	/// <returns>A string that represents the current object.</returns>
	public override string ToString() => m_Content.ToString();


	/// <summary>
	/// Each time Dispose is called it will close the current code block and reduce the indent level by 1.
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
}
