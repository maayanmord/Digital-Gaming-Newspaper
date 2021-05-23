using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DGN.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Category name")]
        [Required]
        public string CategoryName { get; set; }

        [DisplayName("Articles related to this category")]
        public IList<Article> Articles { get; set; } = new List<Article>();
    }
}
