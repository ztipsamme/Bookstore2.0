using System;

namespace Bookstore2._0.Flows;

public class FlowBase
{
    internal readonly DbService _dbService;

    public FlowBase(DbService dbService)
    {
        _dbService = dbService;
    }
}
