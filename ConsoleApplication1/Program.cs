using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using System.Data.Linq;
using Levex.CM.BusinessLogic;
using Levex.CM.Contract;
using Levex.CM.EntityFramework;

namespace ConsoleApplication1
{
    class Program
    {
        public static List<string> floors = new List<string>();

        static void Main(string[] args)
        {
            var description = "abrancancha";
            var remark = "a";
            var className = "pepe";
            
            var result = description;
            var tagInit = "<span class=\"" + className + "\">";
            var tagEnd = "</span>";

            if (!string.IsNullOrEmpty(remark))
            {
                var val = remark.Trim().ToLower();
                var ix = description.ToLower().IndexOf(val);

                while (ix >= 0)
                {
                    result = result.Substring(0, ix) + tagInit + result.Substring(ix, remark.Trim().Length) + tagEnd +
                        result.Substring(ix + val.Length);
                    if (result.Substring(ix + (val + tagInit + tagEnd).Length).ToLower().IndexOf(val) < 0)
                        ix = -1;
                    else
                        ix = result.Substring(ix + (val + tagInit + tagEnd).Length).ToLower().IndexOf(val) + (val + tagInit + tagEnd).Length + ix;
                }
            }

            //ChangeLocations();
        }

        private static void ChangeLocations()
        {
            var c = new ObjectContextEntities();
            foreach (var p in c.Places.ToList())
            {
                var i = p.Address.IndexOf(" entre ");
                p.MapLocation = (i > 0 ? p.Address.Substring(0, i) : p.Address) + ", " + (p.Location.Region.Description == p.Location.Description ? "" : (p.Location.Description + ", ")) + p.Location.Region.Description + ", " + p.Location.Region.Territory.Description;
            }

            c.SaveChanges();
        }

        private static void HoySeJuegaScan()
        {
            var client = new WebClient();

            for (int i = 1; i < 36; i++)
            {
                String htmlCode = client.DownloadString("http://www.hoysejuega.com/busqueda.php?p=" + i + "&bus=1&pais=1");
                var text = "<div class=\"titulos\"><h1><a href=\"";
                var ix = htmlCode.IndexOf(text);
                while (ix >= 0)
                {
                    htmlCode = htmlCode.Substring(ix + text.Length);

                    // html de la cancha
                    var html = htmlCode.Substring(0, htmlCode.IndexOf("\">"));
                    GetInfo(client, html);

                    ix = htmlCode.IndexOf(text);
                }
            }

            Console.WriteLine("escaneo terminado");
        }

        private static void GetInfo(WebClient client, string html)
        {
            var page = html.Replace("-", "").Replace(".htm", "");

            if (new PlaceBusiness().Get(page) == null)
            {
                try
                {
                    Console.WriteLine("obteniendo datos para " + page);
                    var buffer = client.DownloadData("http://www.hoysejuega.com/" + html);
                    var info = Encoding.UTF8.GetString(buffer);


                    var name = GetName(ref info);

                    var addressPhoneTag = "<span class=\"txt_verde_12\">";

                    string ad;
                    int locID;
                    try
                    {
                        info = GetAddress(info, addressPhoneTag, out ad, out locID);
                    }
                    catch
                    {
                        Console.WriteLine("fallo la consulta");
                        return;
                    }

                    var phone = GetPhone(ref info, addressPhoneTag);

                    var howToArrive = GetHowToArrive(ref info);

                    int ix;
                    List<Service> services;
                    info = GetServices(info, out ix, out services);

                    var place = new PlaceDto
                    {
                        Description = name,
                        Address = ad,
                        Location = LocationDto.Create(locID),
                        Phone = phone,
                        IsActive = true,
                        Page = page,
                        Services = services,
                        HowToArrive = howToArrive,
                    };


                    new PlaceBusiness().Add(place);

                    CreateCourts(ref info, ref ix, place);
                }
                catch (WebException)
                {
                    Console.WriteLine("pagina no existe");
                    return;
                }
            }
        }

        private static void CreateCourts(ref string info, ref int ix, PlaceDto place)
        {
            // canchas
            var courtInitTag = "<td align=\"left\" class=\"txt_verde2_14_bd\">Canchas</td>";
            var courtTag = "<td class=\"txt_verde_11_bd\">Cancha:";
            info = info.Substring(info.IndexOf(courtInitTag) + courtInitTag.Length);

            ix = info.IndexOf(courtTag);
            var courts = new List<CourtDto>();

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

                    var courtDto = new CourtDto
                    {
                        Place = PlaceDto.Create(place.ID),
                        Description = court,
                        Players = players,
                        CourtType = CourtTypeDto.Create(1),
                        FloorType = FloorTypeDto.Create(floorID),
                        IsIndoor = indoor,
                        IsLighted = lighted,
                        IsActive = true,
                    };

                    new CourtBusiness().Add(courtDto);
                }

                ix = info.IndexOf(courtTag);
            }
        }

        private static string GetConfig(string info, out bool indoor, out bool lighted)
        {
            var configTag = "<img src=\"img/bullets/flechita.gif\" width=\"5\" height=\"6\" />&nbsp;";
            info = info.Substring(info.IndexOf(configTag) + configTag.Length);
            var config = info.Substring(0, info.IndexOf("</td>")).Trim();
            indoor = config.Contains("Techada");
            lighted = config.Contains("Con luz");
            return info;
        }

        private static int GetPlayers(ref string info)
        {
            var playersTag = "<img src=\"img/bullets/flechita.gif\" width=\"5\" height=\"6\" />&nbsp;";
            info = info.Substring(info.IndexOf(playersTag) + playersTag.Length);
            var players = Convert.ToInt32(info.Substring(0, info.IndexOf("Jugadores</td>")).Trim());
            return players;
        }

        private static int GetFloor(ref string info)
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

        private static string GetServices(string info, out int ix, out List<Service> services)
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

        private static string GetHowToArrive(ref string info)
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

        private static string GetPhone(ref string info, string addressPhoneTag)
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

        private static string GetAddress(string info, string addressPhoneTag, out string ad, out int locID)
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

            var regs = new RegionBusiness().Get(reg);

            int regID;
            var regDto = new RegionDto { Description = reg, IsActive = true, Territory = TerritoryDto.Create(1) };

            if (regs.Count() == 0)
            {
                new RegionBusiness().Add(regDto);
                regID = regDto.ID;
            }
            else if (regs.Count > 0 && !regs.Any(r => r.Description == reg))
            {
                new RegionBusiness().Add(regDto);
                regID = regDto.ID;
            }
            else
            {
                regID = regs.First().ID;
            }

            var locs = new LocationBusiness().Get(loc);


            var locDto = new LocationDto { Description = loc, IsActive = true, Region = RegionDto.Create(regID) };

            if (locs.Count() == 0)
            {
                new LocationBusiness().Add(locDto);
                locID = locDto.ID;
            }
            else if (locs.Count > 0 && !locs.Any(r => r.Description == loc))
            {
                new LocationBusiness().Add(locDto);
                locID = locDto.ID;
            }
            else
            {
                locID = locs.First().ID;
            }
            return info;
        }

        private static string GetName(ref string info)
        {
            // obtener nombre
            var nameTag = "<td width=\"333\" align=\"left\"><span class=\"txt_verde2_18_bd\">";
            info = info.Substring(info.IndexOf(nameTag) + nameTag.Length);
            var name = info.Substring(0, info.IndexOf("</span>")).Trim();
            return name;
        }
    }
}
