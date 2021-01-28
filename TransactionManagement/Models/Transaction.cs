using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TransactionManagement.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        [Required]
        public bool Type { get; set; }
        [Required]
        public string ShortNote { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }

    }
}
