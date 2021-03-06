﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Fulbaso.Contract;
using Fulbaso.EntityFramework;

namespace Fulbaso.EntityFramework.Logic
{
    public class CourtConfigurationLogic : ICourtConfigurationLogic
    {
        private ObjectContextEntities _context;

        public CourtConfigurationLogic(ObjectContextEntities context)
        {
            _context = context;
        }

        public void Add(CourtConfiguration courtConfiguration)
        {
            var courtConfigurationEntity = new CourtConfigurationEntity
            {
                CourtId = courtConfiguration.Court.Id,
                StartHour = courtConfiguration.StartHour,
                EndHour = courtConfiguration.EndHour,
                Days = CourtConfigurationLogic.GetDays(courtConfiguration.Days),
                Price = courtConfiguration.Price,
                Order = courtConfiguration.Order,
                StartDate = courtConfiguration.StartDate,
                EndDate = courtConfiguration.EndDate,
            };

            _context.CourtConfigurations.AddObject(courtConfigurationEntity);
            _context.SaveChanges();
            
            courtConfiguration.Id = courtConfigurationEntity.Id;
        }

        public void SetOrder(IEnumerable<int> idsInOrder)
        {
            for (int i = 0; i < idsInOrder.Count(); i++)
            {
                var Id = idsInOrder.ToList()[i];
                _context.CourtConfigurations.First(c => c.Id == Id).Order = Convert.ToByte(i + 1);
            }

            _context.SaveChanges();
        }

        public void Update(CourtConfiguration courtConfiguration)
        {
            var courtConfigurationEntity = _context.CourtConfigurations.First(cc => cc.Id == courtConfiguration.Id);

            courtConfigurationEntity = new CourtConfigurationEntity
            {
                Id = courtConfiguration.Id,
                CourtId = courtConfiguration.Court.Id,
                StartHour = courtConfiguration.StartHour,
                EndHour = courtConfiguration.EndHour,
                Days = CourtConfigurationLogic.GetDays(courtConfiguration.Days),
                Price = courtConfiguration.Price,
                Order = courtConfiguration.Order,
                StartDate = courtConfiguration.StartDate,
                EndDate = courtConfiguration.EndDate,
            };

            _context.SaveChanges();
        }

        public void Delete(int courtConfigurationId)
        {
            this.Delete(CourtConfiguration.Create<CourtConfiguration>(courtConfigurationId));
        }

        private void Delete(CourtConfiguration courtConfiguration)
        {
            _context.CourtConfigurations.DeleteObject(new CourtConfigurationEntity { Id = courtConfiguration.Id });
            _context.SaveChanges();
        }

        public CourtConfiguration Get(int courtConfigurationId)
        {
            return this.Get(r => r.Id == courtConfigurationId).SingleOrDefault();
        }

        public decimal GetPrice(int courtId, DateTime time)
        {
            return (from p in _context.CourtConfigurations.Where(p => p.CourtId == courtId).ToList()
                    where (p.StartHour == null || p.StartHour <= time.Hour) &&
                          (p.EndHour == null || p.EndHour >= time.Hour) &&
                          (p.Days == null || CourtConfigurationLogic.GetDays(p.Days).Contains(time.DayOfWeek)) &&
                          (p.StartDate == null || p.StartDate <= time) &&
                          (p.EndDate == null || p.EndDate <= time)
                    orderby p.Order
                    select p.Price).FirstOrDefault();
        }

        public IEnumerable<CourtConfiguration> GetByCourt(int courtId)
        {
            return this.Get(r => r.CourtId == courtId);
        }

        internal IEnumerable<CourtConfiguration> Get(Expression<Func<CourtConfigurationEntity, bool>> predicate)
        {
            var query = _context.CourtConfigurations.Where(predicate);
            return CourtConfigurationLogic.Get(query);
        }

        internal static IEnumerable<CourtConfiguration> Get(IQueryable<CourtConfigurationEntity> query)
        {
            return CourtConfigurationLogic.Get(query.ToList());
        }

        internal static IEnumerable<CourtConfiguration> Get(List<CourtConfigurationEntity> list)
        {
            return (from r in list
                    select new CourtConfiguration
                    {
                        Id = r.Id,
                        Court = Court.Create<Court>(r.CourtId),
                        StartHour = r.StartHour,
                        EndHour = r.EndHour,
                        Days = CourtConfigurationLogic.GetDays(r.Days),
                        Price = r.Price,
                        Order = r.Order,
                        StartDate = r.StartDate,
                        EndDate = r.EndDate,
                    }).ToList();
        }

        private static byte GetDays(IEnumerable<DayOfWeek> days)
        {
            var boolArray = new BitArray(new[] 
            {
                days.Contains(DayOfWeek.Monday),
                days.Contains(DayOfWeek.Tuesday),
                days.Contains(DayOfWeek.Wednesday),
                days.Contains(DayOfWeek.Thursday),
                days.Contains(DayOfWeek.Friday),
                days.Contains(DayOfWeek.Saturday),
                days.Contains(DayOfWeek.Sunday),
            });
            byte[] bytes = new byte[1];
            boolArray.CopyTo(bytes, 0);

            return bytes[0];
        }

        private static IEnumerable<DayOfWeek> GetDays(byte? days)
        {
            if (days == null) return new List<DayOfWeek>();

            var bits = new BitArray(System.BitConverter.GetBytes((byte)days));

            var list = new List<DayOfWeek>();
            if (bits[0]) list.Add(DayOfWeek.Monday);
            if (bits[1]) list.Add(DayOfWeek.Tuesday);
            if (bits[2]) list.Add(DayOfWeek.Wednesday);
            if (bits[3]) list.Add(DayOfWeek.Thursday);
            if (bits[4]) list.Add(DayOfWeek.Friday);
            if (bits[5]) list.Add(DayOfWeek.Saturday);
            if (bits[6]) list.Add(DayOfWeek.Sunday);

            return list;
        }

        public int GetPlaceId(int id)
        {
            return _context.CourtConfigurations.Where(cc => cc.Id == id).Select(cc => cc.Court.PlaceId).ToList().First();
        }
    }
}
