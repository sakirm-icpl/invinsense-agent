using System.Collections.Generic;

namespace Common.Models
{
    public interface IModel
    {
    }

    public class ResultModel : IModel
    {
        public ResultModel(bool isSuccess)
        {
            Success = isSuccess;
        }

        public bool Success { get; set; }
    }

    public class ModelList<T> : List<T>, IModel
    {
        public ModelList()
        {
        }

        public ModelList(IEnumerable<T> data) : base(data)
        {
        }
    }

    public interface IException
    {
    }
}