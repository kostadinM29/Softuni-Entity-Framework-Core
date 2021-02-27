using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_SalesDatabase.Data.Models
{
   public class Customer
    {
        public Customer()
        {
            Sales = new HashSet<Sale>();
        }
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(80)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "char(20)")] // possible error
        public string CreditCardNumber { get; set; }


        public virtual ICollection<Sale> Sales { get; set; }
    }
}
