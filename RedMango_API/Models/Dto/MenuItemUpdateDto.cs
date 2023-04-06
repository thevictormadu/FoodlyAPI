using System.ComponentModel.DataAnnotations;

namespace RedMango_API.Models.Dto
{
    public class MenuItemUpdateDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string SpecialTag { get; set; }
        public string Category { get; set; }
        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        public FormFile File { get; set; }
    }
}
