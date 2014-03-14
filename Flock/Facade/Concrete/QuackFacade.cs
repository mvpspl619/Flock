using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Web;
using Flock.DTO;
using Flock.DataAccess.Base;
using Flock.DataAccess.EntityFramework;
using Flock.DataAccess.Repositories.Interfaces;
using Flock.Facade.Interfaces;
using System;
using Flock.Models;


namespace Flock.Facade.Concrete
{
    public class QuackFacade : IQuackFacade
    {
        private readonly IQuackRepository _quackRepository;
        private readonly IQuackTypeRepository _quackTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IQuackLikeRepository _quackLikeRepository;
        private readonly IHashTagRepository _hashTagRepository;
        private readonly IHashtagQuackRepository _hashtagQuackRepository;
        private readonly IUserFacade _userFacade;
        private readonly IImageFacade _imageFacade;

        public QuackFacade(IQuackRepository quackRepository, IQuackTypeRepository quackTypeRepository, IUserRepository userRepository, IQuackLikeRepository quackLikeRepository, IHashTagRepository hashTagRepository,
            IHashtagQuackRepository hashtagQuackRepository, IUserFacade userFacade, IImageFacade imageFacade)
        {
            _quackRepository = quackRepository;
            _quackTypeRepository = quackTypeRepository;
            _userRepository = userRepository;
            _quackLikeRepository = quackLikeRepository;
            _hashTagRepository = hashTagRepository;
            _hashtagQuackRepository = hashtagQuackRepository;
            _userFacade = userFacade;
            _imageFacade = imageFacade;
        }

        public static Image ResizeImage(Image imgToResize, int width, int height)
        {
            return (Image)(new Bitmap(imgToResize, new Size(width, height)));
        }

        public byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return ms.ToArray();
        }


        public int SaveQuack(Quack quack)
        {
            //Remove this in production
            //int id = quack.QuackContent.ID;
            //quack.QuackContent.ID = id + 1;
            //quack.ID = id + 1;
            quack.CreatedDate = DateTime.Now;
            quack.LastModifiedDate = DateTime.Now;
            quack.Active = true;
            var quackType = _quackTypeRepository.GetQuackByQuackType(quack.QuackTypeID);

            quack.QuackType = quackType;
            var user = _userRepository.GetUserById(quack.UserID);
            quack.User = user;

            if (!String.IsNullOrEmpty(quack.QuackContent.ImageUrl))
            {
                var img = quack.QuackContent.ImageUrl;
                quack.QuackContent.ImageUrl = "Y";
                var currentImage = img.Substring(img.IndexOf(',') + 1);
                var data = Convert.FromBase64String(currentImage);

                //Reduce image here and feed it down below
                Image imgPhoto = Image.FromStream(new MemoryStream(data));
                double ar = (Convert.ToDouble(imgPhoto.Width)) / (Convert.ToDouble(imgPhoto.Height));
                imgPhoto = ResizeImage(imgPhoto, 538, (Convert.ToInt32(538 / ar)));
                data = ImageToByteArray(imgPhoto);
                quack.QuackContent.Image = data;
            }

            quack.QuackContent.CreatedDate = DateTime.Now;

            //look for hashtags
            var regex = new Regex(@"(?<=#)\w+");
            var matches = regex.Matches(quack.QuackContent.MessageText);

            _quackRepository.SaveQuack(quack);
            int quackId =(int) _quackRepository.GetQuackID(quack);
            foreach (Match m in matches)
            {
                HashTag hashtag = new HashTag();
                hashtag.Name = m.ToString();
                int hashtagId = 0;
                //check for hashtag
                if (!_hashTagRepository.CheckHashTag(hashtag))
                {
                    _hashTagRepository.SaveHashTag(hashtag);
                    hashtagId = hashtag.Id;
                }
                else
                {
                    //get the hashtag id and append
                    hashtagId = _hashTagRepository.GetHashTagId(hashtag);
                }
                HashtagQuack hashtagQuack = new HashtagQuack();
                hashtagQuack.HashTagId = hashtagId;
                hashtagQuack.QuackId = quack.ID;
                _hashtagQuackRepository.SaveHashtagQuack(hashtagQuack);
            }
            if (quack.ConversationID != 0)
            {
                _quackRepository.UpdateQuack(quack.ConversationID);
            }
            return quack.ID;
        }

        public QuackDto ReloadQuack(int id)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var user = _userFacade.GetUserDetails(userName);
            var qs = _quackRepository.GetQuackById(id);
            var q = new QuackDto();
            q = QuackMapper(qs, user.ID);
            var replies = _quackRepository.GetAllReplies(id);
            var qReplies = (from reply in replies let qreply = new QuackDto() select QuackMapper(reply)).ToList();
            q.QuackReplies = qReplies;
            q.Replies = replies.Count(qq => qq.Active);
            return q;
        }

        public void GetQuack(int id)
        {
            _quackRepository.GetQuack(id);
        }
        public IList<QuackDto> GetQuacksInfo(int conversationId)
        {
            var quacks = _quackRepository.GetQuacksInfo(conversationId);
            var userName = HttpContext.Current.User.Identity.Name;
            var user = _userFacade.GetUserDetails(userName);

            var quacksInfo = (from quack in quacks let q = new QuackDto() select QuackMapper(quack, user.ID)).ToList();


            foreach (var q in quacksInfo)
            {
                q.Replies = quacksInfo.Count - 1;
            }
            return quacksInfo;
        }

        private string VerifyLikeOrUnLike(Quack quack, int userId)
        {
            var check = quack.QuackLikes.FirstOrDefault(q => q.UserId == userId && q.Active && q.QuackId == quack.ID && q.Active);
            return check == null ? "like" : "unlike";
        }

        public QuacksList GetAllQuacks(int quackCount)
        {
            QuacksList returningList = new QuacksList();
            try
            {
                var userName = HttpContext.Current.User.Identity.Name;
                var user = _userFacade.GetUserDetails(userName);

                var quacks = _quackRepository.GetAllQuacks();
                returningList.QuackCount = quacks.Count;
                var quackResults = new List<QuackDto>();

                for (int i = quackCount; i < quackCount + 5; i++)
                {
                    var quack = quacks[i];
                    var q = new QuackDto();
                    q = QuackMapper(quack, user.ID);
                    var replies = _quackRepository.GetAllReplies(quack.ID);
                    var qReplies = (from reply in replies let qreply = new QuackDto() select QuackMapper(reply)).ToList();
                    q.QuackReplies = qReplies;
                    q.Replies = replies.Count(qq => qq.Active);
                    quackResults.Add(q);
                }
                returningList.Quacks = quackResults;
                return returningList;
            }
            catch (Exception)
            {
                var userName = HttpContext.Current.User.Identity.Name;
                var user = _userFacade.GetUserDetails(userName);

                var quacks = _quackRepository.GetAllQuacks();
                var quackResults = new List<QuackDto>();
                returningList.QuackCount = quacks.Count;
                for (int i = quackCount; i < quacks.Count; i++)
                {
                    var quack = quacks[i];
                    var q = new QuackDto();
                    q = QuackMapper(quack, user.ID);
                    var replies = _quackRepository.GetAllReplies(quack.ID);
                    var qReplies = (from reply in replies let qreply = new QuackDto() select QuackMapper(reply)).ToList();
                    q.QuackReplies = qReplies;
                    q.Replies = replies.Count(qq => qq.Active);
                    quackResults.Add(q);
                }
                returningList.Quacks = quackResults;
                return returningList;
            }
        }

        public IList<QuackDto> GetAllQuacksbyHashtag(string hashtag)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var user = _userFacade.GetUserDetails(userName);
            //read the quackids from hashtagQuacks
            var hashtagId = _hashTagRepository.GetHashTagId(new HashTag { Name = hashtag });
            var quackResults = new List<QuackDto>();
            //get every single quack
            List<int> quackIds = _hashtagQuackRepository.GetQuackIdsByHashtagId(hashtagId);

            var quacksList = _quackRepository.GetAllQuacksByIdList(quackIds);
            foreach (var quack in quacksList)
            {
                var q = new QuackDto();
                q = QuackMapper(quack, user.ID);
                var replies = _quackRepository.GetAllReplies(quack.ID);
                var qReplies = (from reply in replies let qreply = new QuackDto() select QuackMapper(reply)).ToList();
                q.QuackReplies = qReplies;
                q.Replies = replies.Count(qq => qq.Active);
                quackResults.Add(q);
            }
            return quackResults;
        }

        public IList<QuackDto> GetAllQuacksWithHashtag(string hashTag)
        {
            var userName = HttpContext.Current.User.Identity.Name;
            var user = _userFacade.GetUserDetails(userName);

            var quacks = _quackRepository.GetAllQuacks();
            var quackResults = new List<QuackDto>();

            foreach (var quack in quacks)
            {
                var q = new QuackDto();
                q = QuackMapper(quack, user.ID);
                var replies = _quackRepository.GetAllReplies(quack.ID);
                var qReplies = (from reply in replies let qreply = new QuackDto() select QuackMapper(reply)).ToList();
                q.QuackReplies = qReplies;
                q.Replies = replies.Count(qq => qq.Active);
                quackResults.Add(q);

            }

            return quackResults;
        }

        private QuackDto QuackMapper(Quack quack, int userId = 0)
        {
            return new QuackDto
                       {
                           Id = quack.ID,
                           Likes = quack.QuackLikes.Count(q => q.Active),
                           Message = quack.QuackContent.MessageText,
                           TimeSpan = GetTimeSpanInformation(quack.LastModifiedDate),
                           UserName = quack.User.UserName,
                           UserImage = quack.User.ProfileImage,
                           UserId = quack.User.ID,
                           LikeOrUnlike = VerifyLikeOrUnLike(quack, userId),
                           IsNew = quack.QuackTypeID == 1 ? true : false,
                           UserNickName = quack.User.UserName.Replace("DS\\", ""),
                           ConversationId = quack.ConversationID,
                           UserDisplayName = quack.User.FirstName + " " + quack.User.LastName,
                           LatestReply = GetRepliesInformation(quack.ID),
                           QuackImage =quack.QuackContent.Image  
                       };
        }

        private QuackDto GetRepliesInformation(int quackId)
        {
            var resultQuack = new QuackDto();

            var commentInfo = "";
            var quacks = _quackRepository.GetAllReplies(quackId);
            if (quacks != null && quacks.Any())
            {
                var latestQuackIndex = quacks.Count() - 1;

                resultQuack.UserDisplayName = quacks[latestQuackIndex].User.FirstName + " " + quacks[latestQuackIndex].User.LastName;
                resultQuack.UserImage = quacks[latestQuackIndex].User.ProfileImage;
                resultQuack.TimeSpan = GetTimeSpanInformation(quacks[latestQuackIndex].LastModifiedDate);
                resultQuack.Id = quacks[latestQuackIndex].ID;
                resultQuack.UserId = quacks[latestQuackIndex].UserID;
                resultQuack.Message = quacks[latestQuackIndex].QuackContent.MessageText;

                switch (quacks.Count())
                {
                    case 1:
                        commentInfo = "";
                        break;
                    case 2:
                        commentInfo = quacks[0].User.FirstName + " also commented on this Quack...";
                        break;
                    case 3:
                        commentInfo = quacks[0].User.FirstName + " and " + quacks[1].User.FirstName +
                                      " also commented on this Quack...";
                        break;
                    case 4:
                        commentInfo = quacks[0].User.FirstName + ", " + quacks[1].User.FirstName + " and " + quacks[2].User.FirstName +
                                      " also commented on this Quack...";
                        break;
                    default:
                        for (var i = 0; i < quacks.Count(); i++)
                        {
                            switch (i)
                            {
                                case 0:
                                    commentInfo = commentInfo + quacks[i].User.FirstName;
                                    break;
                                case 1:
                                    commentInfo = commentInfo + ", " + quacks[i].User.FirstName;
                                    break;
                            }
                        }
                        commentInfo = commentInfo + " and " + (quacks.Count() - 2) + " others also replied to this Quack...";
                        break;
                }
                resultQuack.CommentsInfo = commentInfo;

            }
            return resultQuack;
        }

        private string GetTimeSpanInformation(DateTime? d)
        {
            var result = "";
            if (d != null)
            {
                var date = (DateTime)d;
                TimeSpan timeSpan = DateTime.Now.Subtract(date);


                if (timeSpan.Days > 2)
                {
                    result = date.ToString("MMMM dd, yyyy") + " at " + date.ToString("hh:mm tt");
                }
                else if (timeSpan.Days > 0)
                {
                    result = timeSpan.Days + " days ago";
                }
                else if (timeSpan.Hours > 0)
                {
                    result = timeSpan.Hours + " hours ago";
                }
                else if (timeSpan.Minutes > 0)
                {
                    result = timeSpan.Minutes + " minutes ago";
                }
                else
                {
                    result = "Few seconds ago";
                }

            }
            return result;
        }

        public void DeleteQuack(int id)
        {
            //check if it is in hashtagQuack table
            if (_hashtagQuackRepository.CheckQuackIdExists(id))
            { 
                //delete all the hashtags
                _hashtagQuackRepository.DeleteHashtagQuacks(id);
            }
            //delete likes
            _quackLikeRepository.DeleteQuackLike(id);
            _quackRepository.DeleteQuack(id);

            var conversations = _quackRepository.GetAllReplies(id);
            foreach (var conversation in conversations)
            {
                _quackRepository.DeleteQuack(conversation.ID);
            }
        }

        public void ActivateQuack(int id)
        {
            //activate the hashtags
            if (_hashtagQuackRepository.CheckQuackIdExists(id))
            {
                //delete all the hashtags
                _hashtagQuackRepository.ActivateHashtagQuacks(id);
            }
            _quackLikeRepository.ActivateQuackLike(id);
            _quackRepository.ActivateQuack(id);

            var conversations = _quackRepository.GetAllReplies(id);
            foreach (var conversation in conversations)
            {
                _quackRepository.ActivateQuack(conversation.ID);
            }
        }

        public void UndoDeleteQuack(int id)
        {

        }

        public void LikeOrUnlikeQuack(int quackId, int userId, Boolean isLike)
        {
            var quackLike = new QuackLike { QuackId = quackId, UserId = userId, Active = isLike };
            _quackLikeRepository.UpdateQuackLike(quackLike);

            _quackRepository.UpdateQuack(quackId);
        }

        public IList<QuackDto> GetQuacksByLastNameAndFirstName(string lastName, string firstName)
        {
            var returnQuacks = new List<QuackDto>();
            var quacks = _quackRepository.GetQuacksByLastNameAndFirstName(lastName, firstName);
            quacks.ToList().ForEach(q => returnQuacks.Add(QuackMapper(q)));
            return returnQuacks;
        }
    }
}