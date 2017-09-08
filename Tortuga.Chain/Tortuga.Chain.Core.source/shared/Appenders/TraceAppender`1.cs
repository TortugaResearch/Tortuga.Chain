using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using Tortuga.Chain.Core;

namespace Tortuga.Chain.Appenders
{
    /// <summary>
    /// Class TraceAppender.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    internal sealed class TraceAppender<TResult> : Appender<TResult>
    {
        readonly TextWriter m_Stream;


        /// <summary>
        /// Initializes a new instance of the <see cref="TraceAppender{TResult}"/> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        public TraceAppender(ILink<TResult> previousLink)
            : base(previousLink)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TraceAppender{TResult}"/> class.
        /// </summary>
        /// <param name="previousLink">The previous link.</param>
        /// <param name="stream">The optional stream to direct the output. If null, Debug is used..</param>
        public TraceAppender(ILink<TResult> previousLink, TextWriter stream) : this(previousLink)
        {
            m_Stream = stream;
        }

        /// <summary>
        /// Override this if you want to examine or modify the DBCommand before it is executed.
        /// </summary>
        /// <param name="e">The <see cref="CommandBuiltEventArgs" /> instance containing the event data.</param>
        protected override void OnCommandBuilt(CommandBuiltEventArgs e)
        {
            Write(m_Stream, e);
        }



        internal static void Write(TextWriter stream, CommandBuiltEventArgs e)
        {
            if (stream == null)
            {

                Debug.WriteLine("Command Text: " + e.Command.CommandText);
#if !WINDOWS_UWP && !NETSTANDARD1_3
                Debug.Indent();
#endif
                foreach (DbParameter parameter in e.Command.Parameters)
                {
                    var valueText = (parameter.Value == null || parameter.Value == DBNull.Value) ? "<NULL>" : parameter.Value.ToString();
                    Debug.WriteLine($"Parameter: {parameter.ParameterName} = {valueText}");
                }
#if !WINDOWS_UWP && !NETSTANDARD1_3
                Debug.Unindent();
#endif
            }
            else
            {
                stream.WriteLine("Command Text: " + e.Command.CommandText);

                foreach (DbParameter parameter in e.Command.Parameters)
                {
                    var valueText = (parameter.Value == null || parameter.Value == DBNull.Value) ? "<NULL>" : parameter.Value.ToString();
                    stream.WriteLine($"    Parameter: {parameter.ParameterName} = {valueText}");
                }

            }

        }
    }
}
