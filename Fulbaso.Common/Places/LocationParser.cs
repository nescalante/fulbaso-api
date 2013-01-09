using System.Linq;
using Fulbaso.Contract;
using Fulbaso.Helpers;

namespace Fulbaso.Common
{
    public static class LocationParser
    {
        public static Location GetLocation(this GeocodeResponse geocodeResponse)
        {
            var locationService = ContainerUtil.GetApplicationContainer().Resolve<ILocationLogic>();
            var regionService = ContainerUtil.GetApplicationContainer().Resolve<IRegionLogic>();
            var territoryService = ContainerUtil.GetApplicationContainer().Resolve<ITerritoryLogic>();

            var territory = geocodeResponse.Country;
            var location = geocodeResponse.AdministrativeAreaLevel2 ?? geocodeResponse.Neighborhood ?? geocodeResponse.Locality ?? geocodeResponse.Country;
            var region = geocodeResponse.AdministrativeAreaLevel1 ?? geocodeResponse.Country;

            // return null if any value is null
            if (new[] { territory, location, region }.Any(string.IsNullOrEmpty)) return null;
            
            // load territory
            var territoryDto = territoryService.Get(territory).Where(t => t.Description == territory).FirstOrDefault();

            if (territoryDto == null)
            {
                territoryDto = Territory.Create<Territory>(territory);
                territoryService.Add(territoryDto);
            }

            // load region
            var regionDto = regionService.Get(region).Where(t => t.Description == region && t.Territory.Id == territoryDto.Id).FirstOrDefault();

            if (regionDto == null)
            {
                regionDto = new Region { Territory = territoryDto, Description = region, };
                regionService.Add(regionDto);
            }

            // load location
            var locationDto = locationService.Get(location).Where(t => t.Description == location && t.Region.Id == regionDto.Id).FirstOrDefault();

            if (locationDto == null)
            {
                locationDto = new Location { Region = regionDto, Description = location, };
                locationService.Add(locationDto);
            }

            locationDto.Region = regionService.Get(locationDto.Region.Id);

            return locationDto;
        }
    }
}
