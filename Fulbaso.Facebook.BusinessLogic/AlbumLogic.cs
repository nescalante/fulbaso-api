using System;
using System.Collections.Generic;
using System.Linq;
using Facebook;
using Fulbaso.Contract;

namespace Fulbaso.Facebook.Logic
{
    public class AlbumLogic : FacebookLogic, IAlbumLogic
    {
        public Album Get(long id)
        {
            var album = this.Client.Get(id.ToString()) as dynamic;

            return new Album
            {
                Id = Convert.ToInt64(album.id),
                Name = album.name,
                Description = album.description,
                Type = album.type == "wall" ? AlbumType.Wall :
                    album.type == "friends_walls" ? AlbumType.FriendsWalls :
                    album.type == "profile" ? AlbumType.Profile :
                    album.type == "mobile" ? AlbumType.Mobile :
                    album.name == "Cover Photos" ? AlbumType.Cover :
                    AlbumType.Normal,
                Count = (album as IDictionary<string, object>).Keys.Contains("count") ? Convert.ToInt32(album.count) : 0,
                Cover = Convert.ToInt64(album.cover_photo),
            };
        }

        public IEnumerable<Album> Get(string page)
        {
            var data = (this.Client.Get(page + "/albums") as dynamic)["data"];

            return (data as List<object>).Cast<dynamic>().Select(album => new Album
            {
                Id = Convert.ToInt64(album.id),
                Name = album.name,
                Description = album.description,
                Type = album.type == "wall" ? AlbumType.Wall :
                    album.type == "friends_walls" ? AlbumType.FriendsWalls :
                    album.type == "profile" ? AlbumType.Profile :
                    album.type == "mobile" ? AlbumType.Mobile :
                    album.name == "Cover Photos" ? AlbumType.Cover :
                    AlbumType.Normal,
                Count = (album as IDictionary<string, object>).Keys.Contains("count") ? Convert.ToInt32(album.count) : 0,
                Cover = Convert.ToInt64(album.cover_photo),
            });
        }
    }
}
