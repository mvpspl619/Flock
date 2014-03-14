using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Flock.DTO;
using Flock.DataAccess.EntityFramework;
using Flock.Models;


namespace Flock.Facade.Interfaces
{
    public interface IQuackFacade
    {
        int SaveQuack(Quack quack);
        void GetQuack(int id);
        QuacksList GetAllQuacks(int quackCount);
        IList<QuackDto> GetAllQuacksWithHashtag(string hashTag);
        IList<QuackDto> GetQuacksByLastNameAndFirstName(string lastName, string firstName);
        IList<QuackDto> GetAllQuacksbyHashtag(string hashtag);
        void ActivateQuack(int id);
        void DeleteQuack(int id);
        QuackDto ReloadQuack(int id);
        void LikeOrUnlikeQuack(int quackId, int userId, Boolean isLike);
        IList<QuackDto> GetQuacksInfo(int conversationId);
    }
}