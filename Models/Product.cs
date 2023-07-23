using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCrudDotNet.Models
{
    [Table("product")]
    public class Product
    {
        public int Id { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
    }
}
