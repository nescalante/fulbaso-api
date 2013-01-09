using System;
using System.Collections.Generic;
using System.Linq;
using Facebook;
using Fulbaso.Contract;

namespace Fulbaso.Facebook.Logic
{
    public class PhotoLogic : FacebookLogic, IPhotoLogic
    {
        public Photo Get(long id)
        {
            var photo = this.Client.Get(id.ToString()) as dynamic;

            return new Photo
            {
                Id = Convert.ToInt64(photo.id),
                Name = photo.name,
                Link = photo.link,
                Picture = photo.picture,
                Source = photo.source,
                Position = Convert.ToInt32(photo.position),
                Height = Convert.ToInt32(photo.height),
                Width = Convert.ToInt32(photo.width),
                Likes = photo.likes != null ? (photo.likes).Count : 0,
            };
        }

        public IEnumerable<Photo> GetFromAlbum(long id)
        {
            var photos = (this.Client.Get(id + "/photos?limit=5000") as dynamic)["data"];

            return (photos as List<object>).Cast<dynamic>().Select(photo => new Photo
            {
                Id = Convert.ToInt64(photo.id),
                Name = photo.name,
                Link = photo.link,
                Picture = photo.picture,
                Source = photo.source,
                Position = Convert.ToInt32(photo.position),
                Height = Convert.ToInt32(photo.height),
                Width = Convert.ToInt32(photo.width),
                Likes = photo.likes != null ? (photo.likes).Count : 0,
            });
        }
    }
}
