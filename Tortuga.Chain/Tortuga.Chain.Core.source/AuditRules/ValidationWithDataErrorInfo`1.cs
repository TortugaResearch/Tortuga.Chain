using System;
using System.ComponentModel;

namespace Tortuga.Chain.AuditRules
{

    /// <summary>
    /// When this rule is in effect, objects of type T will be checked.
    /// </summary>
    /// <seealso cref="Rule" />
    public class ValidationWithDataErrorInfo<T> : ValidationWithDataErrorInfo
        where T : class, IDataErrorInfo
    {
        private readonly Action<T> m_ValidationMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationWithDataErrorInfo{T}" /> class.
        /// </summary>
        /// <param name="appliesWhen">The rule applies when.</param>
        /// <param name="validationMethod">The method on the object that triggers validation. Usually this will be something like x =&gt; x.Validate().</param>
        public ValidationWithDataErrorInfo(OperationType appliesWhen, Action<T> validationMethod) : base(appliesWhen)
        {
            m_ValidationMethod = validationMethod;
        }

        public override void CheckValue(object argumentValue)
        {
            var validation = argumentValue as T;

            if (validation == null)
                return;

            m_ValidationMethod(validation);
            CheckValueCore(validation);
        }

    }

}
