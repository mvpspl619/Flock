using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flock.DataAccess.EntityFramework;

namespace Flock.DataAccess.Repositories.Interfaces
{
    public interface IHashtagQuackRepository
    {
        void SaveHashtagQuack(HashtagQuack hashtagQuack);
        bool CheckQuackIdExists(int quackId);
        void DeleteHashtagQuacks(int quackId);
        void ActivateHashtagQuacks(int quackId);
        void DeleteSingleHashtagQuack(int hashtagQuackId);
        List<int> GetQuackIdsByHashtagId(int hashtagId);
        //IList<Quack> GetAllQuacks(string hashTag);
    }
}