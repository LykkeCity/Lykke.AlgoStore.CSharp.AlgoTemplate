﻿using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class AlgoCommentData
    {
        public string CommentId { get; set; }
        public string AlgoId { get; set; }
        public string Author { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? EditedOn { get; set; }
        public string Content { get; set; }
    }
}
