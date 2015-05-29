using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevBlah.DotNetToolkit.Linq;
using DevBlah.SqlExpressionBuilder.Expressions;

namespace DevBlah.SqlExpressionBuilder.Meta.Conditions
{
    public class ConditionSet : List<ISubCondition>
    {
        public IEnumerable<ParameterExpression> GetParameterExpressions()
        {
            // traverse all subsets including other subsets
            IEnumerable<IEnumerable<ISubCondition>> traversed = this
                .TraverseOfType<ISubCondition, IEnumerable<ISubCondition>>(x => x);

            // flatten the wheresets
            List<ISubCondition> flattened = traversed.SelectMany(x => x).ToList();

            flattened.AddRange(this);

            return flattened
                // only value sets which are including parameters
                .OfType<IParameterizedCondition>()
                .SelectMany(x => x.GetParameterExpressions());
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder("(");

            for (var i = 0; i < Count; ++i)
            {
                if (i > 0)
                {
                    sb.Append(" ").Append(this[i].Operation.ToString().ToUpper()).Append(" ");
                }

                sb.Append(this[i]);
            }

            sb.Append(")");

            return sb.ToString();
        }
    }
}
