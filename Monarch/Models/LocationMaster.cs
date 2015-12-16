using Geocoding.Google;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Monarch.Models
{   
    public class LocationMaster
    {
        private GoogleGeocoder geocoder;
        public static readonly string ApiKey = "AIzaSyCpgsLNrZmhDoEYkJkzqBI1L7wDvcv9Fz0"; // API KEY HERE

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string PostalCode { get; private set; }


        public LocationMaster()
        {
            if (!string.IsNullOrEmpty(ApiKey))
                geocoder = new GoogleGeocoder(ApiKey);
            else
                geocoder = new GoogleGeocoder();
            clearProperties();
        }

        private void clearProperties()
        {
            Latitude = 0;
            Longitude = 0;
            City = null;
            State = null;
            Country = null;
            PostalCode = null;
        }

        public bool TryMasterLocation(dynamic latitude, dynamic longitude, string city, string state, string country, out string message)
        {
            clearProperties();
            Thread.Sleep(150);
            if (latitude == 0)
                latitude = null;
            if (longitude == 0)
                longitude = null;
            if (latitude == null || longitude == null)
            {
                if (city == null || state == null || country == null)
                {
                    message = "Could not verify location because neither a full (latitude and longitude) OR (city, state, and country) was given.";
                    return false;
                }

                // else there is a city, state, country
                var addresses = geocoder.Geocode(string.Format("{0} {1} {2}", city, state, country));
                if (addresses.Count() <= 0)
                {
                    message = string.Format("Geocode query for {0} {1} {2} return nothing. Check the location.", city, state, country);
                    return false;
                }

                setProperties(addresses.First());


                message = string.Format("Succesfully geocoded location for {0} {1} {2}", city, state, country);
                return true;
            }
            else // both latitude and longitude are not null
            {
                // check latitude and longtiude bounds
                if (Math.Abs(latitude) > 90)
                {
                    message = string.Format("Latitude {0} is out of max range of +/-90", latitude);
                    return false;
                }
                if (Math.Abs(longitude) > 180)
                {
                    message = string.Format("Longitude {0} is out of max range of +/-180", longitude);
                    return false;
                }

                IEnumerable<GoogleAddress> addresses = geocoder.ReverseGeocode(latitude, longitude);
                if (addresses.Count() <= 0)
                {
                    message = string.Format("ReverseGeocode query for ({0},{1}) return nothing. Check the location.", (double)latitude, (double)longitude);
                    return false;
                }
                setProperties(addresses.First());
                message = string.Format("Successfully reverse geocoded location for ({0},{1})", (double)latitude, (double)longitude);
                return true;
            }

        }

        private void setProperties(GoogleAddress address)
        {
            foreach (var component in address.Components)
            {
                if (component.Types.Contains(GoogleAddressType.Locality))
                    City = component.LongName;
                if (component.Types.Contains(GoogleAddressType.AdministrativeAreaLevel1))
                    State = component.LongName;
                if (component.Types.Contains(GoogleAddressType.Country))
                    Country = component.LongName;
                if (component.Types.Contains(GoogleAddressType.PostalCode))
                    PostalCode = component.LongName;
            }
            Latitude = address.Coordinates.Latitude;
            Longitude = address.Coordinates.Longitude;
        }
    }
}