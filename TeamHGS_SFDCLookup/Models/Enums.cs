using System.ComponentModel.DataAnnotations;

namespace TeamHGS_SFDCLookup.Models
{
    public class Enums
    {
        public enum OriginatingBusinessUnits
        {
            [Display(Name = "All")]
            All = 1,
            [Display(Name = "NACET")]
            NACET,
            [Display(Name = "UK")]
            UK,
            [Display(Name = "Healthcare")]
            Healthcare,
            [Display(Name = "Colibrium")]
            Colibrium,
            [Display(Name="India Domestic")]
            IndiaDomestic,
            [Display(Name="Latin America")]
            LatinAmerica,
            [Display(Name="Middle East")]
            MiddleEast,
            APAC,
            [Display(Name="Axis Point Health")]
            AxisPointHealth,
            [Display(Name="Element")]
            Element
        }
    }
}
