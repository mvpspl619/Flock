using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flock.DataAccess.Base;
using Flock.DataAccess.EntityFramework;
using Flock.DataAccess.Repositories.Interfaces;

namespace Flock.DataAccess.Repositories.Concrete
{
    public class QuackLikeRepository:  SqlRepository<QuackLike>, IQuackLikeRepository
    {
        public QuackLikeRepository(FlockContext context) : base(context)
        {
        }

        public void DeleteQuackLike(int quackId)
        {
            var allQuackLikes = base.GetAll().ToArray();
            var quackLikesToDelete = allQuackLikes.Where(q => q.QuackId == quackId);
            foreach (var singleQuackLike in quackLikesToDelete)
            {
                singleQuackLike.Active = false;
                base.Update(singleQuackLike);
            }
        }

        public void ActivateQuackLike(int quackId)
        {
            var allQuackLikes = base.GetAll().ToArray();
            var quackLikesToDelete = allQuackLikes.Where(q => q.QuackId == quackId);
            foreach (var singleQuackLike in quackLikesToDelete)
            {
                singleQuackLike.Active = true;
                base.Update(singleQuackLike);
            }
        }

        public void UpdateQuackLike(QuackLike quackLike)
        {
            var allQuackLikes = base.GetAll().ToArray();
            var quackToUpdate = allQuackLikes.FirstOrDefault(q => q.QuackId == quackLike.QuackId && q.UserId == quackLike.UserId);
            
            if(quackToUpdate!=null )
            {
                var id = quackToUpdate.Id;  
                var getQuack = base.GetById(id);
                getQuack.QuackId = quackLike.QuackId;
                getQuack.UserId = quackLike.UserId;
                getQuack.Active = quackLike.Active;

                base.Update(getQuack);
            }
            else
            {
                base.Add(quackLike);    
            }
            
        }

        public List<QuackLike> GetUserLikesInfo(int quackId)
        {
            var userLikesInfo = base.GetAll();
            return userLikesInfo.Where(x => x.QuackId == quackId && x.Active ).ToList();
        }
    }
}
