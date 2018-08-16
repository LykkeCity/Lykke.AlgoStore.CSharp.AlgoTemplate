using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public interface IAlgoCommentsRepository
    {
        Task<List<AlgoCommentData>> GetCommentsForAlgoAsync(string algoId);
        Task<AlgoCommentData> GetCommentByIdAsync(string algoId, string commentId);
        Task<List<AlgoCommentData>> GetAllAsync();
        Task<AlgoCommentData> SaveCommentAsync(AlgoCommentData data);
        Task DeleteCommentsAsync(string algoId);
        Task DeleteCommentAsync(string algoId, string commentId);
    }
}
