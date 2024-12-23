using System.ComponentModel.DataAnnotations;

namespace App.Models.Provinces
{
    public class AdministrativeUnit
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(255)]
        public string? FullName { get; set; }

        [MaxLength(255)]
        public string? FullNameEn { get; set; }

        [MaxLength(255)]
        public string? ShortName { get; set; }

        [MaxLength(255)]
        public string? ShortNameEn { get; set; }

        [MaxLength(255)]
        public string? CodeName { get; set; }

        [MaxLength(255)]
        public string? CodeNameEn { get; set; }

        public ICollection<Province> Provinces { get; set; }
        public ICollection<District> Districts { get; set; }
        public ICollection<Ward> Wards { get; set; }
    }


}
