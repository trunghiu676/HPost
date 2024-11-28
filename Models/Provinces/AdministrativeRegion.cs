using System.ComponentModel.DataAnnotations;

namespace App.Models.Provinces
{
    public class AdministrativeRegion
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string NameEn { get; set; }

        [MaxLength(255)]
        public string? CodeName { get; set; }

        [MaxLength(255)]
        public string? CodeNameEn { get; set; }

        public ICollection<Province> Provinces { get; set; }
    }


}