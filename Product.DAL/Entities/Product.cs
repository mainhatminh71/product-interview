using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Product.DAL.Entities
{
    [Table("Items")]
    public class Item
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemId { get; set; }
        [Required]
        [MaxLength(100)]
        public String ItemName { get; set; }
        [Required]
        [MaxLength(100)]
        public double Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
