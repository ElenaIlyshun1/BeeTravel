using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models
{
    public class SearchUser
    {
        public string currentFilter { get; set; }  
        public string searchString { get; set; }  
        public string sortOrder { get; set; }    
     
    }
}
