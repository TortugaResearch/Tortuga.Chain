namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// This delegate is used for restricting access to a column
    /// </summary>
    /// <param name="userValue">The user value.</param>
    /// <returns></returns>
    public delegate bool ExceptWhenPredicate(object userValue);

}

