using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml;
using Fulbaso.Contract;
using Fulbaso.Helpers;

namespace Fulbaso.Importer
{
    public class HoySeJuegaImporter : IImporter
    {
        private static string url = "http://www.hoysejuega.com/";
        private IConsoleOutput _output;
        private IPlaceService _placeService;
        private ICourtService _courtService;
        private ITerritoryService _territoryService;
        private IRegionService _regionService;
        private ILocationService _locationService;
        private WebClient _client;

        public HoySeJuegaImporter(IConsoleOutput output, IPlaceService placeService, ICourtService courtService, ITerritoryService territoryService, IRegionService regionService, ILocationService locationService)
        {
            _output = output;
            _placeService = placeService;
            _courtService = courtService;
            _territoryService = territoryService;
            _regionService = regionService;
            _locationService = locationService;

            _client = new WebClient();
        }
        
        public void Import()
        {
            _output.Write("Scanning HoySeJuega");

            
            var html = _client.DownloadString(url + "busqueda.php");

            int totalPages;
            {
                var pages = html.FindFirstByClass("txt_verde3_11");
                totalPages = Convert.ToInt32(pages.Substring(pages.IndexOf("1 de ") + 5, 2));
            }

            _output.Write("Total pages: " + totalPages);
            _output.Write("");

            for (int i = 1; i <= totalPages; i++)
            {
                _output.Write("Scanning page: " + i);
                _output.Write("");

                string placeInfo = _client.DownloadString(url + "busqueda.php?p=" + i);
                string place = placeInfo.FindFirstByClass("resultados");

                while (!string.IsNullOrEmpty(place))
                {
                    string page = place.FindFirstByAttr("href");
                                        
                    CreateFromPage(page);

                    place = place.IgnoreFirst().FindFirstByClass("titulos");
                }
            }
        }

        private void CreateFromPage(string page)
        {
            var pagename = page.Replace("-", "").Replace(".htm", "");

            if (_placeService.Get(pagename) == null)
            {
                _output.Write("Scanning place: " + page);

                try
                {
                    var buffer = _client.DownloadData(url + page);
                    var html = Encoding.UTF8.GetString(buffer);

                    var name = GetName(ref html);

                    var addressPhoneTag = "<span class=\"txt_verde_12\">";

                    string ad;
                    int locID;

                    string description = null;

                    try
                    {
                        var aux = html.IndexOf("<td class=\"txt_verde_12\">");
                        if (aux > 0)
                        {
                            var saux = html.Substring(html.IndexOf("<td class=\"txt_verde_12\">"));
                            saux = saux.Substring(saux.IndexOf(">") + 1);
                            saux = saux.Substring(0, saux.IndexOf("<"));

                            description = saux.Trim();
                        }
                    }
                    catch
                    {
                    }

                    string maplocation = null;
                    decimal? lat = null;
                    decimal? lng = null;

                    try
                    {
                        html = GetAddress(html, addressPhoneTag, out ad, out locID, out maplocation, out lat, out lng);
                    }
                    catch
                    {
                        _output.Write("Query fails.");
                        return;
                    }

                    var phone = GetPhone(ref html, addressPhoneTag);

                    var howToArrive = GetHowToArrive(ref html);

                    int ix;
                    List<Service> services;
                    html = GetServices(html, out ix, out services);

                    var place = new Place
                    {
                        Description = name,
                        Info = description,
                        Address = ad,
                        Location = new Location { Id = locID },
                        Phone = phone,
                        IsActive = true,
                        Page = pagename,
                        Services = services,
                        HowToArrive = howToArrive,
                        MapLocation = maplocation,
                        MapUa = lat,
                        MapVa = lng,
                    };

                    _placeService.Add(place);

                    CreateCourts(ref html, ref ix, place);
                }
                catch (WebException)
                {
                    _output.Write("Page doesn't exists.");
                    return;
                }
                catch (Exception ex)
                {
                    _output.Write("Page error.");
                }
            }
        }

        private void CreateCourts(ref string info, ref int ix, Place place)
        {
            // canchas
            var courtInitTag = "<td align=\"left\" class=\"txt_verde2_14_bd\">Canchas</td>";
            var courtTag = "<td class=\"txt_verde_11_bd\">Cancha:";
            info = info.Substring(info.IndexOf(courtInitTag) + courtInitTag.Length);

            ix = info.IndexOf(courtTag);
            var courts = new List<Court>();

            while (ix >= 0)
            {
                info = info.Substring(ix + courtTag.Length);
                var court = info.Substring(0, info.IndexOf("</td>")).Trim();
                if (court.Trim() != string.Empty)
                {
                    var players = GetPlayers(ref info);

                    int floorID = GetFloor(ref info);

                    bool indoor;
                    bool lighted;
                    info = GetConfig(info, out indoor, out lighted);

                    var courtDto = new Court
                    {
                        Place = Place.Create<Place>(place.Id),
                        Description = court,
                        Players = players,
                        CourtType = CourtType.Create<CourtType>(1),
                        FloorType = FloorType.Create<FloorType>(floorID),
                        IsIndoor = indoor,
                        IsLighted = lighted,
                        IsActive = true,
                    };

                    _courtService.Add(courtDto);
                }

                ix = info.IndexOf(courtTag);
            }
        }

        private string GetConfig(string info, out bool indoor, out bool lighted)
        {
            var configTag = "<img src=\"img/bullets/flechita.gif\" width=\"5\" height=\"6\" />&nbsp;";
            info = info.Substring(info.IndexOf(configTag) + configTag.Length);
            var config = info.Substring(0, info.IndexOf("</td>")).Trim();
            indoor = config.Contains("Techada");
            lighted = config.Contains("Con luz");
            return info;
        }

        private int GetPlayers(ref string info)
        {
            var playersTag = "<img src=\"img/bullets/flechita.gif\" width=\"5\" height=\"6\" />&nbsp;";
            info = info.Substring(info.IndexOf(playersTag) + playersTag.Length);
            var players = Convert.ToInt32(info.Substring(0, info.IndexOf("Jugadores</td>")).Trim());
            return players;
        }

        private int GetFloor(ref string info)
        {
            var floorTag = "&nbsp;Suelo:";
            info = info.Substring(info.IndexOf(floorTag) + floorTag.Length);
            var floor = info.Substring(0, info.IndexOf("</td>")).Trim();
            int floorID = 0;

            switch (floor)
            {
                case "Parquet":
                    floorID = 1;
                    break;
                case "Cemento":
                    floorID = 2;
                    break;
                case "Cesped Sintetico":
                    floorID = 3;
                    break;
                case "Baldosa":
                    floorID = 4;
                    break;
                case "Goma":
                    floorID = 5;
                    break;
                case "Pasto de caucho":
                    floorID = 6;
                    break;
                case "Cesped Natural":
                    floorID = 7;
                    break;
                case "Piso Flotante":
                    floorID = 8;
                    break;
            }

            if (floorID == 0) throw new Exception();

            return floorID;
        }

        private string GetServices(string info, out int ix, out List<Service> services)
        {
            var servicesTag = "<span class=\"txt_verde2_14_bd\">Otros servicios:</span><br />";
            var serviceTag = "<td valign=\"top\" class=\"txt_verde_12\">";
            info = info.Substring(info.IndexOf(servicesTag) + servicesTag.Length);

            ix = info.IndexOf(serviceTag);
            services = new List<Service>();

            while (ix >= 0)
            {
                info = info.Substring(ix + serviceTag.Length);
                var service = info.Substring(0, info.IndexOf("</td>")).Trim();
                if (service.Trim() != string.Empty)
                {
                    switch (service)
                    {
                        case "Ayuda Medica para Emergencias":
                            services.Add(Service.EmergencyMedicalAid);
                            break;
                        case "Torneos":
                            services.Add(Service.Tournament);
                            break;
                        case "Parrilla":
                            services.Add(Service.Grill);
                            break;
                        case "Bar / Restaurante":
                            services.Add(Service.Eatery);
                            break;
                        case "Vestuarios":
                            services.Add(Service.DressingRoom);
                            break;
                        case "Colegios":
                            services.Add(Service.School);
                            break;
                        case "Cumpleaños":
                            services.Add(Service.Birthday);
                            break;
                        case "Escuelita de Fútbol":
                            services.Add(Service.SoccerSchool);
                            break;
                        case "Estacionamiento":
                            services.Add(Service.Parking);
                            break;
                        case "Gimnasio":
                            services.Add(Service.Gym);
                            break;
                    }
                }

                ix = info.IndexOf(serviceTag);
            }
            return info;
        }

        private string GetHowToArrive(ref string info)
        {
            var howToArriveTag = "C&oacute;mo llegar </span><br />";

            if (info.IndexOf(howToArriveTag) > 0)
            {
                info = info.Substring(info.IndexOf(howToArriveTag) + howToArriveTag.Length);
                info = info.Substring(info.IndexOf("<span class=\"txt_verde_11\">") + "<span class=\"txt_verde_11\">".Length);
                var howToArrive = info.Substring(0, info.IndexOf("</span>")).Trim().Replace("<br />", Environment.NewLine);
                return howToArrive;
            }
            else
                return null;
        }

        private string GetPhone(ref string info, string addressPhoneTag)
        {
            var phoneTag = "<span class=\"txt_verde2_14_bd\">Tel&eacute;fono</span>";

            if (info.IndexOf(phoneTag) > 0)
            {
                info = info.Substring(info.IndexOf(phoneTag) + phoneTag.Length);
                info = info.Substring(info.IndexOf(addressPhoneTag) + addressPhoneTag.Length);
                var phone = info.Substring(0, info.IndexOf("</span>")).Trim();
                return phone;
            }
            else
                return null;
        }

        private string GetAddress(string info, string addressPhoneTag, out string ad, out int locID, out string maplocation, out decimal? lat, out decimal? lng)
        {
            // obtener direccion
            var addressTag = "<span class=\"txt_verde2_14_bd\">Direcci&oacute;n</span>";
            info = info.Substring(info.IndexOf(addressTag) + addressTag.Length);
            info = info.Substring(info.IndexOf(addressPhoneTag) + addressPhoneTag.Length);
            var address = info.Substring(0, info.IndexOf("</span>")).Trim();
            var values = address.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.None).Select(v => v.Trim()).ToList();

            ad = "";
            string loc = "";
            string reg = "";
            string ter = "Argentina";
            lat = null;
            lng = null;

            if (values.Count() == 11)
            {
                ad = values[0].Substring(0, values[0].Length - 1).Trim() +
                    " entre " + values[4].Substring(0, values[4].Length - 1).Trim() + " y " +
                    values[6].Substring(0, values[6].Length - 6).Trim();
                loc = values[8].Substring(0, values[8].Length - 1).Trim();
                reg = values[10];
            }
            else if (values.Count() == 5)
            {
                ad = values[0].Substring(0, values[0].Length - 6).Trim();
                loc = values[2].Substring(0, values[2].Length - 1).Trim();
                reg = values[4];
            }

            if (reg == "")
            {
                locID = 0;
                maplocation = null;
                return null;
            }

            var i = ad.IndexOf(" entre ");
            maplocation = (i > 0 ? ad.Substring(0, i) : ad) + ", " + (reg == loc ? "" : (loc + ", ")) + reg;

            try
            {
                var gr = new Geocoding().Get(maplocation);

                lat = gr.Latitude;
                lng = gr.Longitude;
                ter = gr.Country;
                maplocation = gr.FormattedAddress;
                loc = gr.AdministrativeAreaLevel2 ?? gr.Neighborhood ?? gr.Locality ?? gr.Country;
                reg = gr.AdministrativeAreaLevel1 ?? gr.Country;
            }
            catch
            {
            
            }

            var ters = _territoryService.Get(ter);

            int terID;
            var terDto = new Territory { Description = ter, IsActive = true };

            if (ters.Count() == 0)
            {
                _territoryService.Add(terDto);
                terID = terDto.Id;
            }
            else if (ters.Count() > 0 && !ters.Any(r => r.Description == ter))
            {
                _territoryService.Add(terDto);
                terID = terDto.Id;
            }
            else
            {
                terID = ters.First().Id;
            }
            
            var regs = _regionService.Get(reg);
            int regID;
            var regDto = new Region { Description = reg, IsActive = true, Territory = Territory.Create<Territory>(terID) };

            if (regs.Count() == 0)
            {
                _regionService.Add(regDto);
                regID = regDto.Id;
            }
            else if (regs.Count() > 0 && !regs.Any(r => r.Description == reg))
            {
                _regionService.Add(regDto);
                regID = regDto.Id;
            }
            else
            {
                regID = regs.First().Id;
            }

            var locs = _locationService.Get(loc);

            var locDto = new Location { Description = loc, IsActive = true, Region = Region.Create<Region>(regID) };

            if (locs.Count() == 0)
            {
                _locationService.Add(locDto);
                locID = locDto.Id;
            }
            else if (locs.Count() > 0 && !locs.Any(r => r.Description == loc))
            {
                _locationService.Add(locDto);
                locID = locDto.Id;
            }
            else
            {
                locID = locs.First().Id;
            }

            return info;
        }

        private string GetName(ref string info)
        {
            // obtener nombre
            var nameTag = "<td width=\"333\" align=\"left\"><span class=\"txt_verde2_18_bd\">";
            info = info.Substring(info.IndexOf(nameTag) + nameTag.Length);
            var name = info.Substring(0, info.IndexOf("</span>")).Trim();
            return name;
        }
    }
}
