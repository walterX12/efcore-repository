using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Repository.DataCleaning
{
    /// <summary>
    /// Entity with timestamp
    /// </summary>
    public interface ITimestamped
    {
        DateTime Timestamp { get; }
    }
}
