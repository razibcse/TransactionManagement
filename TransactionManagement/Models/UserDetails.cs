using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionManagement.Models
{
    public class UserDetails
    {
        public int Id { get; set; }
        public double TotalIncome { get; set; }
        public double TotalExpence { get; set; }
        public double TotalBalance { get; set; }
        public string OwnerId { get; set; }

    }
}

