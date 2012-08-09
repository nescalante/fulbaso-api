using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Levex.CM.Contract;
using System.Linq.Expressions;
using Levex.CM.DataAccess;

namespace Levex.CM.BusinessLogic
{
    public class CommentBusiness : Repository
    {
        public List<CommentDto> GetByPlace(int placeID)
        {
            return this.Get(c => c.PlaceID == placeID);
        }

        internal List<CommentDto> Get(Expression<Func<Comment, bool>> predicate)
        {
            var query = this.Context.Comments.Where(predicate);
            return CommentBusiness.Get(query);
        }

        internal static List<CommentDto> Get(IQueryable<Comment> query)
        {
            var list = query.Include(p => p.Place).Include(p => p.User).ToList();

            return CommentBusiness.GetList(list.Where(i => i.ReplyCommentID == null).ToList(), list.Where(i => i.ReplyCommentID != null).ToList());
        }

        internal static List<CommentDto> GetList(List<Comment> list, List<Comment> source)
        {
            var dtoList = new List<CommentDto>();
            
            foreach (var i in list)
            {
                var item = new CommentDto
                {
                    ID = i.CommentID,
                    Comment = i.Comment1,
                    Date = i.Date,
                    Parent = i.ReplyCommentID == null ? null : new CommentDto { ID = i.ReplyCommentID.Value },
                    Place = i.Place.ToEntity<PlaceDto>(),
                    User = i.User.ToEntity<UserDto>(),
                };

                item.Replys = CommentBusiness.GetList(source.Where(l => l.ReplyCommentID == i.CommentID).ToList(), source.Where(l => l.ReplyCommentID != i.CommentID).ToList());
                dtoList.Add(item);
            }

            return dtoList;
        }

        public void Add(int placeID, int userID, string comment)
        {
            var commentEntity = new Comment
            {
                UserID = userID,
                Date = DateTime.Now,
                Comment1 = comment,
                PlaceID = placeID,
            };

            this.Context.Comments.AddObject(commentEntity);
            this.Context.SaveChanges();
        }

        public void Add(int placeID, string user, string comment)
        {
            var users = this.Context.Users.Where(u => u.UserName.ToLower() == user.ToLower()).Select(u => u.UserID).ToList();

            if (!users.Any()) return;
            else
                Add(placeID, users.First(), comment);
        }

        public void Delete(int commentID)
        {
            this.Context.Comments.DeleteObject(this.Context.Comments.Single(c => c.CommentID == commentID));
            this.Context.SaveChanges();
        }
    }
}
