using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Tortuga.Anchor.Metadata;

namespace Tortuga.Chain.Appenders
{
    internal sealed class ExpressionJoinAppender<T1, T2> : Appender<Tuple<List<T1>, List<T2>>, List<T1>>
    {
        readonly Func<T1, T2, bool> m_JoinExpression;
        readonly JoinOptions m_JoinOptions;
        readonly Func<T1, ICollection<T2>> m_TargetCollectionExpression;

        public ExpressionJoinAppender(ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, T2, bool> joinExpression, Func<T1, ICollection<T2>> targetCollectionExpression, JoinOptions joinOptions) : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException(nameof(previousLink), $"{nameof(previousLink)} is null.");

            m_TargetCollectionExpression = targetCollectionExpression ?? throw new ArgumentNullException(nameof(targetCollectionExpression), $"{nameof(targetCollectionExpression)} is null.");
            m_JoinOptions = joinOptions;
            m_JoinExpression = joinExpression ?? throw new ArgumentNullException(nameof(joinExpression), $"{nameof(joinExpression)} is null.");
        }

        public ExpressionJoinAppender(ILink<Tuple<List<T1>, List<T2>>> previousLink, Func<T1, T2, bool> joinExpression, string targetCollectionName, JoinOptions joinOptions) : base(previousLink)
        {
            if (previousLink == null)
                throw new ArgumentNullException(nameof(previousLink), $"{nameof(previousLink)} is null.");

            if (string.IsNullOrEmpty(targetCollectionName))
                throw new ArgumentException($"{nameof(targetCollectionName)} is null or empty.", nameof(targetCollectionName));

            var targetPropertyStub = MetadataCache.GetMetadata(typeof(T1)).Properties[targetCollectionName]; //don't inline this variable.
            m_TargetCollectionExpression = (p) => (ICollection<T2>)targetPropertyStub.InvokeGet(p);

            m_JoinOptions = joinOptions;
            m_JoinExpression = joinExpression ?? throw new ArgumentNullException(nameof(joinExpression), $"{nameof(joinExpression)} is null.");
        }

        public override List<T1> Execute(object state = null)
        {
            var result = PreviousLink.Execute(state);
            Match(result);
            return result.Item1;
        }

        public override async Task<List<T1>> ExecuteAsync(CancellationToken cancellationToken, object state = null)
        {
            var result = await PreviousLink.ExecuteAsync(state).ConfigureAwait(false);
            Match(result);
            return result.Item1;
        }

        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ChildObject")]
        void Match(Tuple<List<T1>, List<T2>> result)
        {
            foreach (var child in result.Item2)
            {
                var matched = false;
                foreach (var parent in result.Item1)
                {
                    if (m_JoinExpression(parent, child))
                    {
                        matched = true;
                        m_TargetCollectionExpression(parent).Add(child);
                        if (!m_JoinOptions.HasFlag(JoinOptions.MultipleParents))
                            break;
                    }
                }
                if (!matched && !m_JoinOptions.HasFlag(JoinOptions.IgnoreUnmatchedChildren))
                {
                    var ex = new UnexpectedDataException("Found child object that couldn't be matched to a parent. See Exception.Data[\"ChildObject\"] for details.");
                    ex.Data["ChildObject"] = child;
                    throw ex;
                }
            }
        }
    }
}