namespace Tortuga.Chain.AuditRules
{

    public abstract class ValidationRule : Rule
    {
        protected ValidationRule(OperationType appliesWhen) : base(appliesWhen)
        {
        }

        public abstract void CheckValue(object argumentValue);

    }
}
