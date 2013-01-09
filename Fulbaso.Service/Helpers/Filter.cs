using Fulbaso.Common;
using Fulbaso.Contract;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using ServiceTag = Fulbaso.Contract.Service;

namespace Fulbaso.Service
{
    public class Filter
    {
        public Filter(decimal? latitude, decimal? longitude, int[] players, int[] floorTypes, string[] locations, byte[] tags, bool isIndoor, bool isLighted, DateTime? date = null, int? hour = null)
        {
            this.Players = players;
            this.FloorTypes = floorTypes;
            this.Locations = locations.Where(l => !string.IsNullOrEmpty(l)).ToArray();
            this.Tags = tags;
            this.IsIndoor = isIndoor;
            this.IsLighted = isLighted;

            this._dateParsed = date != null;
            this._hourParsed = hour != null;
            this.Date = date ?? DateTime.Today;
            this.Hour = hour ?? 0;

            this.Latitude = latitude;
            this.Longitude = Longitude;
        }

        private const char separator = '-';
        private bool _dateParsed;
        private bool _hourParsed;

        public string Query { get; private set; }

        public int[] Players { get; private set; }

        public int[] FloorTypes { get; private set; }

        public string[] Locations { get; private set; }

        public byte[] Tags { get; set; }

        public bool IsIndoor { get; private set; }

        public bool IsLighted { get; private set; }

        public DateTime Date { get; private set; }

        public int Hour { get; private set; }

        public decimal? Latitude { get; private set; }

        public decimal? Longitude { get; private set; }

        public bool IsAdvanced
        {
            get
            {
                return this.Players.Any() || this.FloorTypes.Any() || this.Locations.Any() || this.Tags.Any() || this.IsIndoor || this.IsLighted;
            }
        }

        public bool IsSchedule
        {
            get
            {
                return this._hourParsed && this._dateParsed;
            }
        }

        public bool HasQuery
        {
            get
            {
                return this.Query != null;
            }
        }

        public string Reference
        {
            get
            {
                var list = new List<string>();

                if (this.Players.Count() > 1 || this.FloorTypes.Count() > 1 || this.Locations.Count() > 1 || this.Tags.Count() > 1)
                {
                    return null;
                }

                if (this.Players.Count() == 1)
                {
                    list.Add(string.Format("para {0} jugadores", this.Players.Single()));
                }

                if (this.FloorTypes.Count() == 1)
                {
                    //var floorType = ContainerUtil.GetApplicationContainer().Resolve<IFloorTypeLogic>().Get(this.FloorTypes.Single());
                    //list.Add(string.Format("con {0}", floorType.Description.ToLower()));
                }

                if (this.Locations.Count() == 1)
                {
                    list.Add(string.Format("en {0}", this.Locations.Single()));
                }

                if (this.Tags.Count() == 1)
                {
                    list.Add(string.Format("con {0}", ((ServiceTag)this.Tags.Single()).GetDescription().ToLower()));
                }

                if (this.IsIndoor)
                {
                    list.Add("techadas");
                }

                if (this.IsLighted)
                {
                    list.Add("iluminadas");
                }

                return list.Count() == 1 ? list.Single() : null;
            }
        }

        public override string ToString()
        {
            var query = !string.IsNullOrEmpty(this.Query) ? ("para \"" + this.Query + "\"") : "";

            if (this.IsAdvanced)
            {
                if (this.Players.Any())
                {
                    query += "para " + FilterUtil.GetJoin(this.Players.Select(p => p.ToString())) + " jugadores, ";
                }

                if (this.Locations.Any())
                {
                    query += "en " + FilterUtil.GetJoin(this.Locations) + ", ";
                }

                if (this.FloorTypes.Any())
                {
                    query += "con " + FilterUtil.GetFloors(this.FloorTypes) + ", ";
                }

                if (this.Tags.Any())
                {
                    query += "con " + FilterUtil.GetTags(this.Tags) + ", ";
                }

                if (this.IsIndoor)
                {
                    query += "techada, ";
                }

                if (this.IsLighted)
                {
                    query += "con luz, ";
                }

                query = query.Length > 0 ? query.Substring(0, query.Length - 2) : "";
            }

            return query == "" ? null : query;
        }
    }
}