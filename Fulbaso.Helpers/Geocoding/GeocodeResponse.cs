namespace Fulbaso.Helpers
{
    public class GeocodeResponse
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public string StreetNumber { get; set; }

        public string Route { get; set; }
        
        public string Country { get; set; }

        public string Locality { get; set; }

        public string AdministrativeAreaLevel1 { get; set; }

        public string AdministrativeAreaLevel2 { get; set; }

        public string Neighborhood { get; set; }

        public string FormattedAddress { get; set; }
    }
}
