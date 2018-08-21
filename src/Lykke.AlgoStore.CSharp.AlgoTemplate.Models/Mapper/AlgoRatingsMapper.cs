using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper
{
    public static class AlgoRatingsMapper
    {
        public static AlgoRatingData ToModel(this AlgoRatingEntity entity)
        {
            var result = new AlgoRatingData();

            if (entity == null)
                return result;

            result.AlgoId = entity.PartitionKey;
            result.ClientId = entity.RowKey;
            result.Rating = entity.Rating;

            return result;
        }

        public static List<AlgoRatingData> ToModel(this ICollection<AlgoRatingEntity> entities)
        {
            var result = new List<AlgoRatingData>();

            if (entities == null || entities.Count == 0)
                return result;

            foreach (var item in entities)
            {
                var ratingData = new AlgoRatingData
                {
                    AlgoId = item.PartitionKey,
                    ClientId = item.RowKey,
                    Rating = item.Rating
                };

                result.Add(ratingData);
            }

            return result;
        }

        public static List<AlgoRatingData> ToModelWithPrimaryKeyClientId(this ICollection<AlgoRatingEntity> entities)
        {
            var result = new List<AlgoRatingData>();

            if (entities == null || entities.Count == 0)
                return result;

            foreach (var item in entities)
            {
                var ratingData = new AlgoRatingData
                {
                    ClientId = item.PartitionKey,
                    AlgoId = item.RowKey,
                    Rating = item.Rating
                };

                result.Add(ratingData);
            }

            return result;
        }

        public static AlgoRatingEntity ToEntity(this AlgoRatingData data)
        {
            var result = new AlgoRatingEntity();

            if (data == null)
                return result;

            result.PartitionKey = data.AlgoId;
            result.RowKey = data.ClientId;
            result.Rating = data.Rating;

            return result;

        }

        public static AlgoRatingEntity ToClientIdPartionKeyEntity(this AlgoRatingData data)
        {
            var result = new AlgoRatingEntity();

            if (data == null)
                return result;

            result.PartitionKey = data.ClientId;
            result.RowKey = data.AlgoId;
            result.Rating = data.Rating;

            return result;
        }
    }

}
