using System;
using System.Collections.Generic;
using System.Text;

namespace TeisterMask.Data.Models.Enums
{
    public enum ExecutionType
    {
        //enumeration of type ExecutionType, with possible values (ProductBacklog, SprintBacklog, InProgress, Finished) (required)
        ProductBacklog = 0,
        SprintBacklog = 1,
        InProgress = 2,
        Finished = 3
    }
}
