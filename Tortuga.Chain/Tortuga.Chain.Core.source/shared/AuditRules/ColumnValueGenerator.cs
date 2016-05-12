namespace Tortuga.Chain.AuditRules
{
    /// <summary>
    /// Rule Value Generator
    /// </summary>
    /// <param name="argumentValue">The argument value.</param>
    /// <param name="userValue">The user value.</param>
    /// <param name="currentValue">The current value. Used when the rule is conditionally applied.</param>
    /// <returns></returns>
    public delegate object ColumnValueGenerator(object argumentValue, object userValue, object currentValue);
}