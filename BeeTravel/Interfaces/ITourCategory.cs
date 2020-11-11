using BeeTravel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Interfaces
{
    interface ITourCategory
    {
        IEnumerable<Category> AllCategories { get; }
    }
}
