using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public interface IAlgo
    {
        string ClientId { get; set; }
        string AlgoId { get; set ;}
        string Name { get; set; }

        string Description { get; set; }
        DateTime DateModified { get; set; }
        DateTime DateCreated { get; set; }
        DateTime? DatePublished { get; set; }
        AlgoVisibility AlgoVisibility { get; set; }
        string AlgoMetaDataInformationJSON { get; set; }
    }
}
