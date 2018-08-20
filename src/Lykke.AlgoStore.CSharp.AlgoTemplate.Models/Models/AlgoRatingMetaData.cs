﻿namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class AlgoRatingMetaData : AlgoData
    {
        public string Author { get; set; }
        public double Rating { get; set; }
        public int RatedUsersCount { get; set; }
        public int UsesCount { get; set; }
    }
}
