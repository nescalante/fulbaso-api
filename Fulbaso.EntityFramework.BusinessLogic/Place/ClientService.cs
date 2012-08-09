using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.BusinessLogic
{
    public class ClientService : IClientService
    {
        public void Add(Client client)
        {
            var entity = new ClientEntity
            {
                PlaceId = client.Place.Id,
                Name = client.Description,
                Phone = client.Phone,
                IsActive = true,
            };

            EntityUtil.Context.Clients.AddObject(entity);
            EntityUtil.Context.SaveChanges();

            client.Id = entity.Id;
        }

        public void Delete(int clientId)
        {
            if (EntityUtil.Context.Clients.Where(c => c.Id == clientId).Select(c => c.Books.Count()).ToList().First() == 0)
            {
                Repository<ClientEntity>.Delete(new ClientEntity { Id = clientId, });
            }
            else
            {
                EntityUtil.Context.Clients.Where(c => c.Id == clientId).ToList().First().IsActive = false;
            }

            EntityUtil.Context.SaveChanges();
        }

        public IEnumerable<Client> GetByPlace(int placeId)
        {
            return (from c in EntityUtil.Context.Clients.Where(c => c.PlaceId == placeId)
                    select new Client
                    {
                        Id = c.Id,
                        Place = new Place { Id = c.PlaceId },
                        Description = c.Name,
                        Phone = c.Phone,
                        IsActive = c.IsActive,
                        Books = c.Books.Count(),
                        LastBook = c.Books.Any() ? c.Books.OrderByDescending(b => b.StartTime).Select(b => b.StartTime).FirstOrDefault() : (DateTime?)null,
                    }).ToList();
        }

        public int GetPlaceId(int clientId)
        {
            return EntityUtil.Context.Clients.Where(c => c.Id == clientId).Select(c => c.PlaceId).First();
        }

        public IEnumerable<Client> GetForAutocomplete(int placeId, string text, int count)
        {
            var term = text.ToUpper();

            return ClientService.Get(
                EntityUtil.Context.Clients.Where(c => c.IsActive && c.PlaceId == placeId && (c.Name.ToUpper().Contains(term) || c.Phone.ToUpper().Contains(term)))
                    .OrderBy(c => c.Name.ToUpper().IndexOf(term) + c.Phone.ToUpper().IndexOf(term))
                    .Take(count));
        }

        internal static IEnumerable<Client> Get(Expression<Func<ClientEntity, bool>> predicate)
        {
            var query = EntityUtil.Context.Clients.Where(predicate);
            return ClientService.Get(query);
        }

        internal static IEnumerable<Client> Get(IQueryable<ClientEntity> query)
        {
            return (from c in query.ToList()
                    select new Client
                    {
                        Id = c.Id,
                        Place = Place.Create<Place>(c.PlaceId),
                        Description = c.Name,
                        Phone = c.Phone,
                        IsActive = c.IsActive,
                    }).ToList();
        }
    }
}
