namespace TFOF.Areas.Core.Services
{
	using TFOF.Areas.Core.Models;
	using Geocoding.Google;
	using GoogleMapsApi;
	using GoogleMapsApi.Entities.Common;
	using GoogleMapsApi.Entities.DistanceMatrix;
	using GoogleMapsApi.Entities.Directions.Request;
	using GoogleMapsApi.Entities.Directions.Response;
	using GoogleMapsApi.Entities.Geocoding.Request;
	using GoogleMapsApi.Entities.Geocoding.Response;
	using GoogleMapsApi.StaticMaps;
	using GoogleMapsApi.StaticMaps.Entities;
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Web;
	using GoogleMapsApi.Entities.DistanceMatrix.Request;
	using GoogleMapsApi.Entities.DistanceMatrix.Response;
	using System.Threading;

	public class GeocodeService
	{
		public class Geocode
		{
			public double Lat { get; set; }
			public double Lng { get; set; }
		}

		public class GeocodeAddress
		{
			public string Source { get; set; }

			public string SourceId { get; set; }

			public string Status { get; set; }

			public double? Latitude { get; set; }

			public double? Longitude { get; set; }

			public string FormattedAddress { get; set; }

			public string StreetNumber { get; set; }

			public string StreetName { get; set; }

			public string City { get; set; }

			public string County { get; set; }

			public string State { get; set; }

			public string Country { get; set; }

			public string PostalCode { get; set; }

			public string ApiResponse { get; set; }

		}

		public enum GeocodeStatus
		{
			Ok,
			Error,
			ZeroResults,
			OverQueryLimit,
			RequestDenied,
			InvalidRequest,
			NotFound,
		}

        public enum GeocodeSource
        {
            Google
        }

        public class AddressComponentType
        {
            public static string StreetNumber = "street_number";
            public static string Route = "route";
            public static string Locality = "locality";
            public static string AdministrativeAreaLevel1 = "administrative_area_level_1";
            public static string AdministrativeAreaLevel2 = "administrative_area_level_2";
            public static string Country = "country";
            public static string PostalCode = "postal_code";
        }

		public static Dictionary<GoogleStatus, GeocodeStatus> GoogleToGeocode = new Dictionary<GoogleStatus, GeocodeStatus>()
			{
				{ GoogleStatus.Ok, GeocodeStatus.Ok },
				{ GoogleStatus.Error, GeocodeStatus.Error},
				{ GoogleStatus.ZeroResults, GeocodeStatus.ZeroResults},
				{ GoogleStatus.OverQueryLimit, GeocodeStatus.OverQueryLimit},
				{ GoogleStatus.RequestDenied, GeocodeStatus.RequestDenied},
				{ GoogleStatus.InvalidRequest, GeocodeStatus.InvalidRequest}
			};

		//public static List<GeocodeAddress> Geocoder(string address)
		//{
		//	List<GeocodeAddress> geocodeAddresses = new List<GeocodeAddress>();
		//	GoogleGeocoder geocoder = new GoogleGeocoder(ConfigurationManager.AppSettings["GoogleMapsApiKey"].ToString());
		//	IEnumerable<GoogleAddress> addresses = geocoder.Geocode(address).ToArray();

		//	if (addresses.Count() == 0)
		//	{
		//		geocodeAddresses.Add(new GeocodeAddress()
		//		{
		//			Status = GeocodeStatus.NotFound.ToString(),
		//			Source = "Google"
		//		});
		//		return geocodeAddresses;
		//	}

		//	foreach (GoogleAddress gaddress in addresses)
		//	{
		//		geocodeAddresses.Add(new GeocodeAddress()
		//		{
		//			Status = GoogleToGeocode[GoogleStatus.Ok].ToString(),
		//			Latitude = gaddress.Coordinates.Latitude,
		//			Longitude = gaddress.Coordinates.Longitude,
		//			Source = gaddress.Provider,
		//			SourceId = gaddress.PlaceId,
		//			FormattedAddress = gaddress.FormattedAddress,
		//			StreetNumber = GetComponentType(gaddress, GoogleAddressType.StreetNumber).LongName,
		//			StreetName = GetComponentType(gaddress, GoogleAddressType.Route).LongName,
		//			City = GetComponentType(gaddress, GoogleAddressType.Locality).LongName,
		//			County = GetComponentType(gaddress, GoogleAddressType.AdministrativeAreaLevel2).LongName,
		//			State = GetComponentType(gaddress, GoogleAddressType.AdministrativeAreaLevel1).LongName,
		//			Country = GetComponentType(gaddress, GoogleAddressType.Country).LongName,
		//			PostalCode = GetComponentType(gaddress, GoogleAddressType.PostalCode).LongName
		//		});
		//	}
  //          //Sleep the thread so we dont go over the limit per seconds with google maps.
  //          Thread.Sleep(100);
  //          return geocodeAddresses;
		//}

		public static GoogleAddressComponent GetComponentType(GoogleAddress address, GoogleAddressType type)
		{
			GoogleAddressComponent component;
			if ((component = address.Components.Where(w => w.Types.Contains(type)).SingleOrDefault()) != null)
			{
				return component;
			}
			return new GoogleAddressComponent(null, null, null);
		}

	   
		public static List<GeocodeAddress> Geocoder2 (string address)
		{
			List<GeocodeAddress> geocodeAddresses = new List<GeocodeAddress>();

			GeocodingRequest geocodeRequest = new GeocodingRequest()
			{
				Address = address,
			};
			geocodeRequest.ApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"].ToString();
			var geocoder =  GoogleMaps.Geocode;

			GeocodingResponse geocode = geocoder.Query(geocodeRequest);
			foreach(Result r in geocode.Results)
			{
				geocodeAddresses.Add(new GeocodeAddress()
				{
					Status = geocode.Status.ToString(),
					Latitude = r.Geometry.Location.Latitude,
					Longitude = r.Geometry.Location.Longitude,
					Source = "Google",
					SourceId = r.PlaceId,
					FormattedAddress = r.FormattedAddress,
					StreetNumber = GetComponentType2(r, AddressComponentType.StreetNumber).LongName,
					StreetName = GetComponentType2(r, AddressComponentType.Route).LongName,
					City = GetComponentType2(r, AddressComponentType.Locality).LongName,
					County = GetComponentType2(r, AddressComponentType.AdministrativeAreaLevel2).LongName,
					State = GetComponentType2(r, AddressComponentType.AdministrativeAreaLevel1).LongName,
					Country = GetComponentType2(r, AddressComponentType.Country).LongName,
					PostalCode = GetComponentType2(r, AddressComponentType.PostalCode).LongName
				});
			}
			//Sleep the thread so we dont go over the limit per seconds with google maps.
			Thread.Sleep(100);
			return geocodeAddresses;
		}

		public static AddressComponent GetComponentType2(Result result, string type)
		{
			AddressComponent component;
			if ((component = result.AddressComponents.Where(w => w.Types.Contains(type)).SingleOrDefault()) != null)
			{
				return component;
			}
			return new AddressComponent();
		}

		public static DistanceMatrixResponse GetDistanceMatrix(string origin, string destination, DateTime departureTime )
		{
			Time time = new Time();
			time.Value = departureTime;
            if (!string.IsNullOrWhiteSpace(origin) && !string.IsNullOrWhiteSpace(destination) && departureTime != null)
            {
                DistanceMatrixRequest distanceRequest = new DistanceMatrixRequest()
                {
                    Origins = new string[] { origin },
                    Destinations = new string[] { destination },
                    TrafficModel = DistanceMatrixTrafficModels.best_guess,
                    DepartureTime = time,
                    IsSSL = true,
                };

                distanceRequest.ApiKey = ConfigurationManager.AppSettings["GoogleMapsApiKey"].ToString();

                try
                {
                    DistanceMatrixResponse distances = GoogleMaps.DistanceMatrix.Query(distanceRequest);
                    //Sleep the thread so we dont go over the limit per seconds with google maps.
                    Thread.Sleep(100);
                    return distances;
                }
                catch
                {
                    Thread.Sleep(100);
                    return null;
                }
               
            } else
            {
                return null;
            }
		}
	}

	//public 
}