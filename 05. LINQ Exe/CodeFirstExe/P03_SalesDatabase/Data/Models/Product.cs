using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P03_SalesDatabase.Data.Models
{
    public class Product
    {
        public Product()
        {
            Description = "No description"; // doesn't set default value so I used Fluent API
            Sales = new HashSet<Sale>();
        }

        [Key]
        public int ProductId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; } // pretty sure this is not decimal but word doc says so 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string Description { get; set; }

        public virtual ICollection<Sale> Sales { get; set; }



    }
}
