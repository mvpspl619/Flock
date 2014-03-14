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
    public class HashTagRepository : SqlRepository<HashTag>, IHashTagRepository
    {
        private readonly FlockContext _context;
        public HashTagRepository(FlockContext context)
            : base(context)
        {
            _context = context;
        }

        public void SaveHashTag(HashTag hashTag)
        {
            base.Add(hashTag);
        }

        public bool CheckHashTag(HashTag hashTag)
        {
            IQueryable<HashTag> hashTags = base.GetAll();
            var hashTagList = hashTags.ToList();
            foreach (var hashtag in hashTagList)
            {
                if (hashtag.Name == hashTag.Name)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetHashTagId(HashTag hashTag)
        {
            IQueryable<HashTag> hashTags = base.GetAll();
            var hashTagList = hashTags.ToList();
            foreach (var hashtag in hashTagList)
            {
                if (hashtag.Name == hashTag.Name)
                {
                    return hashtag.Id;
                }
            }
            return 0;
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
