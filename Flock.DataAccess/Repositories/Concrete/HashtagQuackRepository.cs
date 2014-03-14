using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flock.DataAccess.Base;
using Flock.DataAccess.EntityFramework;
using Flock.DataAccess.Repositories.Interfaces;

namespace Flock.DataAccess.Repositories.Concrete
{
    public class HashtagQuackRepository : SqlRepository<HashtagQuack>, IHashtagQuackRepository
    {
        private readonly FlockContext _context;
        public HashtagQuackRepository(FlockContext context)
            : base(context)
        {
            _context = context;
        }

        public void SaveHashtagQuack(HashtagQuack hashtagQuack)
        {
            base.Add(hashtagQuack);
        }

        public bool CheckQuackIdExists(int quackId)
        {
            var hashtagQuacks = _context.HashtagQuacks
                .Where(q => q.QuackId == quackId);
            var hashtagQuackList = hashtagQuacks.ToList();
            if (hashtagQuackList.Count != 0) 
            {
                return true;
            }
            return false;
        }

        public void DeleteHashtagQuacks(int quackId)
        { 
            var hashtagQuacks = _context.HashtagQuacks
                .Where(q => q.QuackId == quackId);
            var hashtagQuackList = hashtagQuacks.ToList();
            for (int i = 0; i < hashtagQuackList.Count; i++)
            {
                DeleteSingleHashtagQuack(hashtagQuackList[i].Id);
            }
        }

        public void DeleteSingleHashtagQuack(int hashtagQuackId) 
        {
            var hashtagQuack = base.GetById(hashtagQuackId);
            hashtagQuack.Active = false;
            base.Update(hashtagQuack);
        }

        public void ActivateHashtagQuacks(int quackId)
        {
            var hashtagQuacks = _context.HashtagQuacks
                .Where(q => q.QuackId == quackId);
            var hashtagQuackList = hashtagQuacks.ToList();
            for (int i = 0; i < hashtagQuackList.Count; i++)
            {
                ActivateSingleHashtagQuack(hashtagQuackList[i].Id);
            }
        }

        public void ActivateSingleHashtagQuack(int hashtagQuackId)
        {
            var hashtagQuack = base.GetById(hashtagQuackId);
            hashtagQuack.Active = true;
            base.Update(hashtagQuack);
        }

        public List<int> GetQuackIdsByHashtagId(int hashtagId)
        {
            var hashtagQuacks = _context.HashtagQuacks
                .Where(h => h.HashTagId == hashtagId).ToList();
            List<int> quackIdList = new List<int>();
            for (int i = 0; i < hashtagQuacks.Count; i++)
            { 
                quackIdList.Add(hashtagQuacks[i].QuackId);
            }
            return quackIdList;
        }

        //IList<Quack> IHashTagRepository.GetAllQuacks(string hashTag)
        //{
        //    var quacks = _context.Quacks
        //        .Where(q=>q.HashTags.Count)
        //        .Include("QuackContent")
        //        .Include("User")
        //        .Include("QuackType");
        //    //TODO: Paging
        //    return quacks.Where(quack => quack.Active && quack.QuackTypeID == 1)
        //        .OrderByDescending(quack => quack.LastModifiedDate)
        //        .Take(200)
        //        .ToList();
        //}
    }
}
