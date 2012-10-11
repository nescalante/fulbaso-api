using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Fulbaso.Contract;
using System.Collections.Generic;
using Fulbaso.Common;

namespace Fulbaso.Web
{
    public class PlacesFilter
    {
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
                    var floorType = ContainerUtil.GetApplicationContainer().Resolve<IFloorTypeService>().Get(this.FloorTypes.Single());
                    list.Add(string.Format("con {0}", floorType.Description.ToLower()));
                }

                if (this.Locations.Count() == 1)
                {
                    list.Add(string.Format("en {0}", this.Locations.Single()));
                }

                if (this.Tags.Count() == 1)
                {
                    list.Add(string.Format("con {0}", ((Service)this.Tags.Single()).GetDescription().ToLower()));
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

        public RouteValueDictionary Route
        {
            get
            {
                try
                {
                    var routes = new RouteValueDictionary();

                    if (this.IsSchedule)
                    {
                        routes.Add("d", this.Date.ToShortDateString());
                        routes.Add("h", this.Hour);
                    }

                    if (this.IsAdvanced)
                    {
                        routes.Add("j", string.Join(separator.ToString(), this.Players));
                        routes.Add("s", string.Join(separator.ToString(), this.FloorTypes));
                        routes.Add("l", string.Join(separator.ToString(), this.Locations));
                        routes.Add("t", string.Join(separator.ToString(), this.Tags));
                        
                        if (this.IsIndoor)
                        {
                            routes.Add("ind", this.IsIndoor);
                        }
                        if (this.IsLighted)
                        {
                            routes.Add("lig", this.IsLighted);
                        }
                    }

                    if (!string.IsNullOrEmpty(this.Query) || !routes.Any())
                    {
                        routes.Add("q", string.IsNullOrEmpty(this.Query) ? "*" : this.Query);
                    }

                    if (this.Latitude.HasValue && this.Longitude.HasValue)
                    {
                        routes.Add("lat", this.Latitude);
                        routes.Add("lng", this.Longitude);
                    }
                    else
                    {
                        var position = CoreUtil.GetPosition();

                        if (position != null)
                        {
                            routes.Add("lat", position.Latitude);
                            routes.Add("lng", position.Longitude);
                        }
                    }

                    return routes;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Could not parse elements.", ex);
                }
            }
        }

        public PlacesFilter(FormCollection collection)
        {
            DateTime date;
            int hour;

            this.Players = InterfaceUtil.GetInts(collection, "player");
            this.FloorTypes = InterfaceUtil.GetInts(collection, "floor");
            this.Locations = collection.AllKeys.Where(k => k.StartsWith("location"))
                .Select(k => collection[k]).Concat(new[] { collection["searchlocation"] }).Where(l => !string.IsNullOrEmpty(l)).ToArray();
            this.Tags = Enum.GetValues(typeof(Service)).Cast<Service>().Where(s => collection[s.ToString()] != null).Select(s => (byte)s).ToArray();
            this.IsIndoor = Convert.ToBoolean(collection["indoor"].Split(',').First());
            this.IsLighted = Convert.ToBoolean(collection["lighted"].Split(',').First());

            this._dateParsed = DateTime.TryParse(collection["date"], out date);
            this._hourParsed = int.TryParse(collection["hour"], out hour);

            var position = CoreUtil.GetPosition();

            if (position != null)
            {
                this.Latitude = position.Latitude;
                this.Longitude = position.Longitude;
            }
        }

        public PlacesFilter(string query, int[] players, int[] floorTypes, string[] locations, byte[] tags, bool isIndoor, bool isLighted, DateTime? date = null, int? hour = null)
        {
            this.Query = query;
            this.Players = players;
            this.FloorTypes = floorTypes;
            this.Locations = locations;
            this.Tags = tags;
            this.IsIndoor = isIndoor;
            this.IsLighted = isLighted;

            this._dateParsed = date != null;
            this._hourParsed = hour != null;
            this.Date = date ?? DateTime.Today;
            this.Hour = hour ?? 0;

            var position = CoreUtil.GetPosition();

            if (position != null)
            {
                this.Latitude = position.Latitude;
                this.Longitude = position.Longitude;
            }
        }

        private static PlacesFilter Clone(PlacesFilter filter)
        {
            return new PlacesFilter(filter.Query, filter.Players, filter.FloorTypes, filter.Locations, filter.Tags, filter.IsIndoor, filter.IsLighted);
        }

        public PlacesFilter AddPlayerOption(int player)
        {
            if (this.Players.Contains(player)) return this;

            var list = this.Players.ToList();
            list.Add(player);
            this.Players = list.ToArray();

            return this;
        }

        public PlacesFilter AddFloorType(int floorType)
        {
            if (this.FloorTypes.Contains(floorType)) return this;

            var list = this.FloorTypes.ToList();
            list.Add(floorType);
            this.FloorTypes = list.ToArray();

            return this;
        }

        public PlacesFilter AddLocation(string location)
        {
            if (this.Locations.Contains(location)) return this;

            var list = this.Locations.ToList();
            list.Add(location);
            this.Locations = list.ToArray();

            return this;
        }

        public PlacesFilter AddTag(byte tag)
        {
            if (this.Tags.Contains(tag)) return this;

            var list = this.Tags.ToList();
            list.Add(tag);
            this.Tags = list.ToArray();

            return this;
        }

        public PlacesFilter RemovePlayerOption(int player)
        {
            this.Players = this.Players.Where(p => p != player).ToArray();

            return this;
        }

        public PlacesFilter RemoveFloorType(int floorType)
        {
            this.FloorTypes = this.FloorTypes.Where(f => f != floorType).ToArray();

            return this;
        }

        public PlacesFilter RemoveLocation(string location)
        {
            this.Locations = this.Locations.Where(l => l != location).ToArray();

            return this;
        }

        public PlacesFilter RemoveTag(byte tag)
        {
            this.Tags = this.Tags.Where(t => t != tag).ToArray();

            return this;
        }

        public PlacesFilter ChangeIndoor()
        {
            this.IsIndoor = !this.IsIndoor;

            return this;
        }

        public PlacesFilter ChangeLighted()
        {
            this.IsLighted = !this.IsLighted;

            return this;
        }

        public PlacesFilter WithPlayerOption(int player)
        {
            var filter = PlacesFilter.Clone(this);
            filter.AddPlayerOption(player);

            return filter;
        }

        public PlacesFilter WithFloorType(int floorType)
        {
            var filter = PlacesFilter.Clone(this);
            filter.AddFloorType(floorType);

            return filter;
        }

        public PlacesFilter WithLocation(string location)
        {
            var filter = PlacesFilter.Clone(this);
            filter.AddLocation(location);

            return filter;
        }

        public PlacesFilter WithTag(byte tag)
        {
            var filter = PlacesFilter.Clone(this);
            filter.AddTag(tag);

            return filter;
        }

        public PlacesFilter WithoutPlayerOption(int player)
        {
            var filter = PlacesFilter.Clone(this);
            filter.RemovePlayerOption(player);

            return filter;
        }

        public PlacesFilter WithoutFloorType(int floorType)
        {
            var filter = PlacesFilter.Clone(this);
            filter.RemoveFloorType(floorType);

            return filter;
        }

        public PlacesFilter WithoutLocation(string location)
        {
            var filter = PlacesFilter.Clone(this);
            filter.RemoveLocation(location);

            return filter;
        }

        public PlacesFilter WithoutTag(byte tag)
        {
            var filter = PlacesFilter.Clone(this);
            filter.RemoveTag(tag);

            return filter;
        }

        public PlacesFilter WithChangedIndoor()
        {
            var filter = PlacesFilter.Clone(this);
            filter.ChangeIndoor();

            return filter;
        }

        public PlacesFilter WithChangedLighted()
        {
            var filter = PlacesFilter.Clone(this);
            filter.ChangeLighted();

            return filter;
        }

        public PlacesFilter(NameValueCollection collection, bool withSchedule = false)
        {
            try
            {
                var keys = collection.AllKeys;

                // text query parser
                if (keys.Contains("q"))
                {
                    this.Query = collection["q"];
                    this.Query = this.Query == "*" ? string.Empty : this.Query;
                }

                // players parse
                this.Players = keys.Contains("j") ? InterfaceUtil.GetInts(collection["j"], separator) : new int[] { };

                // floor types parse
                this.FloorTypes = keys.Contains("s") ? InterfaceUtil.GetInts(collection["s"], separator) : new int[] { };

                // locations parse
                this.Locations = keys.Contains("l") ? (collection["l"] ?? "").Split(separator).Where(i => !string.IsNullOrEmpty(i)).ToArray() : new string[] { };

                // tags parse
                this.Tags = keys.Contains("t") ? InterfaceUtil.GetBytes(collection["t"], separator) : new byte [] { };

                // indoor parse
                this.IsIndoor = keys.Contains("ind") && Convert.ToBoolean(collection["ind"]);

                // lighted parse
                this.IsLighted = keys.Contains("lig") && Convert.ToBoolean(collection["lig"]);

                // parse schedule
                if (withSchedule)
                {
                    DateTime date = DateTime.Today;
                    int hour = 0;
                    if (keys.Contains("d") && DateTime.TryParse(collection["d"], out date))
                    {
                        this.Date = date;
                    }
                    else
                    {
                        this.Date = DateTime.Today;
                    }

                    this._dateParsed = true;
                    this._hourParsed = true;
                    if (keys.Contains("h") && int.TryParse(collection["h"], out hour))
                    {
                        this.Hour = hour;
                    }
                }
                else
                {
                    this._dateParsed = false;
                    this._hourParsed = false;
                }

                // latitude & longitude parse
                if (keys.Contains("lat") && keys.Contains("lng"))
                {
                    this.Latitude = Convert.ToDecimal(collection["lat"].Replace(".", ","));
                    this.Longitude = Convert.ToDecimal(collection["lng"].Replace(".", ","));

                    CoreUtil.UpdatePosition(this.Latitude.Value, this.Longitude.Value);
                }
                else
                {
                    var position = CoreUtil.GetPosition();

                    if (position != null)
                    {
                        this.Latitude = position.Latitude;
                        this.Longitude = position.Longitude;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Could not parse query values collection.", ex);
            }
        }

        public override string ToString()
        {
            var query = !string.IsNullOrEmpty(this.Query) ? ("para \"" + this.Query + "\"") : "";

            if (this.IsAdvanced)
            {
                if (this.Players.Any())
                {
                    query += "para " + InterfaceUtil.GetJoin(this.Players.Select(p => p.ToString())) + " jugadores, ";
                }

                if (this.Locations.Any())
                {
                    query += "en " + InterfaceUtil.GetJoin(this.Locations) + ", ";
                }

                if (this.FloorTypes.Any())
                {
                    query += "con " + CoreUtil.GetFloors(this.FloorTypes) + ", ";
                }

                if (this.Tags.Any())
                {
                    query += "con " + CoreUtil.GetTags(this.Tags) + ", ";
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

            return query;
        }
    }
}