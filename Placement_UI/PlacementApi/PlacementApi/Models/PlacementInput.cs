using System.ComponentModel.DataAnnotations;

namespace PlacementAPI.Models
{
    public class PlacementInput
    {
        [Required]
        [Range(0, 10)]
        public float CGPA { get; set; }
        [Required]
        [Range(0, 200)]
        public float IQ { get; set; }
    }
}