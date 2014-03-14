using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flock.DataAccess.EntityFramework;

namespace Flock.DataAccess.Repositories.Interfaces
{
    public interface IHashTagRepository
    {
        void SaveHashTag(HashTag hashTag);
        bool CheckHashTag(HashTag hashTag);
        int GetHashTagId(HashTag hashTag);
        //IList<Quack> GetAllQuacks(string hashTag);
    }
}