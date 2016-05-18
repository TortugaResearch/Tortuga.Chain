

namespace Tortuga.Chain.Access
{

    /// <summary>
    /// 
    /// </summary>
    internal enum AccessCommandExecutionMode
    {
        /// <summary>
        /// Pass the prepared command to the materializer for execution.
        /// </summary>
        Materializer = 0,

        /// <summary>
        /// Run the prepared command directly, bypassing the materializer.
        /// </summary>
        /// <remarks>This is usually used when chaining commands.</remarks>
        NonQuery = 1,

        /// <summary>
        /// Reads a value using ExecuteScalar and passes the result to the next execution token
        /// </summary>
        ExecuteScalarAndForward = 2


    }


}
