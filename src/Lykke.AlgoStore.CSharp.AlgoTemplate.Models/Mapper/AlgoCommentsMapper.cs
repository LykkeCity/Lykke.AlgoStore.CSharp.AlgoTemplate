﻿using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper
{
    public static class AlgoCommentsMapper
    {
        public static AlgoCommentData ToModel(this AlgoCommentEntity entity)
        {
            var result = new AlgoCommentData
            {
                AlgoId = entity.PartitionKey,
                CommentId = entity.RowKey,
                Author = entity.AuthorId,
                Content = entity.Content,
                CreatedOn = entity.CreatedOn,
                EditedOn = entity.EditedOn
            };

            return result;
        }

        public static List<AlgoCommentData> ToModel(this List<AlgoCommentEntity> entities)
        {
            var result = new List<AlgoCommentData>();

            foreach (var entity in entities)
            {
                var data = new AlgoCommentData
                {
                    AlgoId = entity.PartitionKey,
                    CommentId = entity.RowKey,
                    Author = entity.AuthorId,
                    Content = entity.Content,
                    CreatedOn = entity.CreatedOn,
                    EditedOn = entity.EditedOn
                };

                result.Add(data);
            }            

            return result;
        }

        public static AlgoCommentEntity ToEntity(this AlgoCommentData data)
        {
            var result = new AlgoCommentEntity
            {
                PartitionKey = data.AlgoId,
                RowKey = data.CommentId,
                AuthorId = data.Author,
                Content = data.Content,
                CreatedOn = data.CreatedOn,
                EditedOn = data.EditedOn
            };

            return result;
        }

        public static List<AlgoCommentEntity> ToEntity(this List<AlgoCommentData> entities)
        {
            return entities.Select(e => e.ToEntity()).ToList();
        }
    }
}
