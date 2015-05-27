
using System.Collections.Generic;

namespace DevBlah.SqlExpressionBuilder.Statements.Where
{
    public interface IWhereSubSet
    {
        ConnectOperations Operation { get; set; }

        IList<IWhereSubSet> Parent { get; }
    }
}
