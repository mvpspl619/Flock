using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flock.DataAccess.Base;
using Flock.DataAccess.EntityFramework;
using Flock.DataAccess.Repositories.Interfaces;
using System.Data.Entity;

namespace Flock.DataAccess.Repositories.Concrete
{
    public class QuackRepository : SqlRepository<Quack>, IQuackRepository
    {
        private readonly FlockContext _context;
        public QuackRepository(FlockContext context)
            : base(context)
        {
            _context = context;
        }

        public void SaveQuack(Quack quack)
        {
            base.Add(quack);
        }

        public double GetQuackID(Quack quack)
        {
            //var singleQuack = _context.Quacks
            //    .Where(q=>q.QuackContent == quack.QuackContent)
            //    .Where(q=>q.QuackType == quack.QuackType)
            //    .Where(q=>q.LastModifiedDate == quack.LastModifiedDate)
            //    .Where(q=>q.User == quack.User).ToList();
            return 0;
        }

        public void GetQuack(int id)
        {
            var quack = _context.Quacks
                .Where(q => q.ID == id)
                .Include("QuackContent")
                .Include("User")
                .Include("QuackType").ToList();

        }

        public Quack GetQuackById(int id)
        {
            var quack = _context.Quacks
                .Where(q => q.ID == id)
                .Include("QuackContent")
                .Include("User")
                .Include("QuackType").ToList().FirstOrDefault();
            return quack;
        }

        IList<Quack> IQuackRepository.GetAllQuacksByIdList(List<int> idList)
        {
            IList<Quack> quackList = new List<Quack>();
            foreach (var id in idList)
            {
                Quack singleQuack = GetQuackById(id);
                quackList.Add(singleQuack);
            }
            return quackList.OrderByDescending(q => q.LastModifiedDate).ToList();
        }

        IList<Quack> IQuackRepository.GetAllQuacks()
        {
            var quacks = _context.Quacks
                .Include("QuackContent")
                .Include("User")
                .Include("QuackType");
            //TODO: Paging
            //return quacks.Where(quack => quack.Active && quack.QuackTypeID ==1)
            //    .OrderByDescending(quack => quack.LastModifiedDate )
            //    .Take(200)
            //    .ToList();
            var finalList = quacks.Where(quack => quack.Active && quack.QuackTypeID == 1)
                .OrderByDescending(quack => quack.LastModifiedDate)
                .ToList();
            return finalList;
        }

        public void DeleteQuack(int quackId)
        {
            var quack = base.GetById(quackId);
            quack.Active = false;
            base.Update(quack);
        }

        public void ActivateQuack(int quackId)
        {
            var quack = base.GetById(quackId);
            quack.Active = true;
            base.Update(quack);
        }

        public void UpdateQuack(int quackId)
        {
            var currentQuack = base.GetById(quackId);
            currentQuack.LastModifiedDate = DateTime.Now;
            base.Update(currentQuack);
        }

        public IList<Quack> GetAllReplies(int quackId)
        {
            var quacks = _context.Quacks
                .Include("QuackContent")
                .Include("User")
                .Include("QuackType");
            //TODO: Paging
            return quacks.Where(quack => quack.Active && quack.QuackTypeID == 2 && quack.ConversationID == quackId)
                .OrderBy(quack => quack.CreatedDate)
                .Take(200)
                .ToList();
        }

        public IList<Quack > GetQuacksInfo(int conversationId)
        {
            var quacks = _context.Quacks
                .Include("QuackContent")
                .Include("User")
                .Include("QuackType");
            //TODO: Paging
            return quacks.Where(quack => quack.Active &&  quack.ConversationID == conversationId)
                .OrderBy(quack => quack.CreatedDate)
                .Take(200)
                .ToList();
        }


        public IQueryable<Quack> GetQuacksByLastNameAndFirstName(string lastName, string firstName)
        {
            return from quack in _context.Quacks
                         join quackContent in _context.QuackContents
                         on quack.ID equals quackContent.ID
                         join user in _context.Users
                         on quack.UserID equals user.ID
                         join quackType in _context.QuackTypes
                         on quack.QuackTypeID equals quackType.ID
                         where user.FirstName.Equals(firstName) && user.LastName.Equals(lastName)
                         orderby quack.CreatedDate descending
                         select quack
                         ;
        }
    }
}
