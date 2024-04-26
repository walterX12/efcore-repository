using System;
using System.Collections.Generic;
using System.Text;

namespace Repository
{
    /// <summary>
    /// Class with identifier of the type <see cref="T"/>
    /// Typical is int or string
    /// </summary>
    /// <typeparam name="T">Datatype of the identyfier</typeparam>
    public  interface IIdentifiable<T>
    {
        T Id { get; }
    }
}
