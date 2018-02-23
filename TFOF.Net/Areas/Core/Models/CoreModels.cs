using System;
using System.Data.Entity.Spatial;

namespace TFOF.Areas.Core.Models
{
	public class Address
    {
     
        public string StreetNumber { get; set; }
        public string StreetName { get; set; }
        public string UnitNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public DbGeography Coordinates { get; set; }
        public string FormattedAddress
        {
            get
            {
                string formattedAddress =
                    (!string.IsNullOrWhiteSpace(StreetNumber) ? StreetNumber + " " : "") +
                    (!string.IsNullOrWhiteSpace(StreetName) ? StreetName + " " : "") +
                    (!string.IsNullOrWhiteSpace(City) ? City + " " : "") +
                    (!string.IsNullOrWhiteSpace(State) ? State + " " : "") +
                    (!string.IsNullOrWhiteSpace(County) ? County + " " : "") +
                    (!string.IsNullOrWhiteSpace(StreetNumber) ? StreetNumber + " " : "") +
                    (!string.IsNullOrWhiteSpace(Country) ? Country + " " : "") +
                    (!string.IsNullOrWhiteSpace(PostalCode) ? PostalCode + " " : "");

                return formattedAddress.Trim();

            }
        }

        public string FormattedAddressWithUnit
        {
            get
            {
                string formattedAddress =
                    (!string.IsNullOrWhiteSpace(StreetNumber) ? StreetNumber + " " : "") +
                    (!string.IsNullOrWhiteSpace(StreetName) ? StreetName + " " : "" )+
                    (!string.IsNullOrWhiteSpace(UnitNumber) ? UnitNumber + " " : "") +
                    (!string.IsNullOrWhiteSpace(City) ? City + " " : "") +
                    (!string.IsNullOrWhiteSpace(State) ? State + " " : "") +
                    (!string.IsNullOrWhiteSpace(County) ? County + " " : "") +
                    (!string.IsNullOrWhiteSpace(StreetNumber) ? StreetNumber + " " : "") +
                    (!string.IsNullOrWhiteSpace(Country) ? Country + " " : "") +
                    (!string.IsNullOrWhiteSpace(PostalCode) ? PostalCode + " " : "");

                return formattedAddress.Trim();

            }
        }

        public string FormattedCoordinates
        {
            get
            {
                return Convert.ToString(Coordinates.Latitude) + " " + Convert.ToString(Coordinates.Longitude);

            }
        }
        public static DbGeography CreateGeography(double lat, double lng, int srid = 4326)
        {
            string wkt = String.Format("POINT({0} {1})", lng, lat);

            return DbGeography.PointFromText(wkt, srid);
        }
    } 

}


