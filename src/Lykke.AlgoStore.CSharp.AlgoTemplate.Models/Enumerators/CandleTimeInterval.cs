using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators
{
    public enum CandleTimeInterval
    {
        Unspecified = 0,

        [Display(Name = "Second")]
        Sec = 1,

        [Display(Name = "Minute")]
        Minute = 60,

        [Display(Name = "5 Minutes")]
        Min5 = 300,

        [Display(Name = "15 Minutes")]
        Min15 = 900,

        [Display(Name = "30 Minutes")]
        Min30 = 1800,

        [Display(Name = "Hour")]
        Hour = 3600,

        [Display(Name = "4 Hours")]
        Hour4 = 14400,

        [Display(Name = "6 Hours")]
        Hour6 = 21600,

        [Display(Name = "12 Hours")]
        Hour12 = 43200,

        [Display(Name = "Day")]
        Day = 86400,

        [Display(Name = "Week")]
        Week = 604800,

        [Display(Name = "Month")]
        Month = 3000000,
    }
}
