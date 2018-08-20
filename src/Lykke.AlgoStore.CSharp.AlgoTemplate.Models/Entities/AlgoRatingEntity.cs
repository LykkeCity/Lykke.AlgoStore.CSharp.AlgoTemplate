using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoRatingEntity : TableEntity
    {
        public double Rating { get; set; }
    }
}
