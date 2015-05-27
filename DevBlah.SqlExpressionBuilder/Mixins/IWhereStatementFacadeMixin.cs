using DevBlah.SqlExpressionBuilder.Interfaces;
using DevBlah.SqlExpressionBuilder.Statements;

namespace DevBlah.SqlExpressionBuilder.Mixins
{
    public interface IWhereStatementFacadeMixin<TFluent> : IWhereStatementFacade<TFluent>
    {
        WhereSet WhereSet { get; }
    }
}
