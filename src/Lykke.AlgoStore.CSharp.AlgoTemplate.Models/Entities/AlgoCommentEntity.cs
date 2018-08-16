using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities
{
    public class AlgoCommentEntity: TableEntity
    {
        public string AuthorId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public string Content { get; set; }
    }
}
