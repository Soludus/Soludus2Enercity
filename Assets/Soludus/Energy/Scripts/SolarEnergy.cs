/*
  Related terms:
  latitude, declination
  lognitude, right ascension, hour angle
*/

using System;

namespace Soludus.Energy
{

    /// <summary>
    /// Funtions for calculating elevation and azimuth angle of the sun at a specified point on earth at a specified time.
    /// </summary>
    public static class SolarEnergy
    {
        private const double Rad2Deg = 180 / Math.PI;
        private const double Deg2Rad = Math.PI / 180;

        private const double DaysInSolarYear = 365.24219;
        private const double AngularSpeedAroundSun = (Math.PI * 2) / DaysInSolarYear;
        private const double Eccentricity = 0.0167;
        private const double AxialTilt = 23.44 * Deg2Rad;
        private const double AxialTiltSine = 0.39779; // Math.Sin(AxialTilt);

        /*
          Optimized methods. Implementation in use.
        */

        /// <summary>
        /// Optimized method.
        /// Calculate both elevation and azimuth. Use to avoid calculating them separately.
        /// </summary>
        /// <param name="dayOfYear"></param>
        /// <param name="timeOfDayHours"></param>
        /// <param name="sinLatitude"></param>
        /// <param name="cosLatitude"></param>
        /// <param name="longitude"></param>
        /// <param name="elevationAngleSine"></param>
        /// <param name="azimuthAngleSine"></param>
        public static void SolarAngleSines(int dayOfYear, double timeOfDayHours, double sinLatitude, double cosLatitude, double longitude, out double elevationAngleSine, out double azimuthAngleSine)
        {
            // source https://en.wikipedia.org/wiki/Position_of_the_Sun#Declination_of_the_Sun_as_seen_from_Earth
            double n = dayOfYear - 1 + timeOfDayHours * (1f / 24f);
            double sinDeclination = -AxialTiltSine * Math.Cos(AngularSpeedAroundSun * (n + 10) + (Math.PI * Eccentricity) * Math.Sin(AngularSpeedAroundSun * (n - 2)));
            double cosDeclination = Math.Sqrt(1 - sinDeclination * sinDeclination);
            double hourAngle = (Math.PI * 2 / 24) * (timeOfDayHours - 12) + longitude;
            double cosHourAngle = Math.Cos(hourAngle);
            double sinElevation = sinLatitude * sinDeclination + cosLatitude * cosDeclination * cosHourAngle;
            elevationAngleSine = sinElevation;

            // source https://en.wikipedia.org/wiki/Solar_azimuth_angle
            double sinHourAngle = Math.Sin(hourAngle);
            double sinZenith = Math.Sqrt(1 - sinElevation * sinElevation);
            double sinAzimuth = (-sinHourAngle * cosDeclination) / sinZenith;
            azimuthAngleSine = sinAzimuth;
        }

        /// <summary>
        /// Optimized method.
        /// Calculate both elevation and azimuth. Use to avoid calculating them separately.
        /// </summary>
        /// <param name="dayOfYear"></param>
        /// <param name="timeOfDayHours"></param>
        /// <param name="sinLatitude"></param>
        /// <param name="cosLatitude"></param>
        /// <param name="longitude"></param>
        /// <param name="elevationAngle"></param>
        /// <param name="azimuthAngle"></param>
        public static void SolarAngles(int dayOfYear, double timeOfDayHours, double sinLatitude, double cosLatitude, double longitude, out double elevationAngle, out double azimuthAngle)
        {
            // source https://en.wikipedia.org/wiki/Position_of_the_Sun#Declination_of_the_Sun_as_seen_from_Earth
            double n = dayOfYear - 1 + timeOfDayHours * (1f / 24f);
            double sinDeclination = -AxialTiltSine * Math.Cos(AngularSpeedAroundSun * (n + 10) + (Math.PI * Eccentricity) * Math.Sin(AngularSpeedAroundSun * (n - 2)));
            double cosDeclination = Math.Sqrt(1 - sinDeclination * sinDeclination);
            double hourAngle = (Math.PI * 2 / 24) * (timeOfDayHours - 12) + longitude;
            double cosHourAngle = Math.Cos(hourAngle);
            double sinElevation = sinLatitude * sinDeclination + cosLatitude * cosDeclination * cosHourAngle;
            elevationAngle = Math.Asin(sinElevation);

            // source http://www.geoastro.de/SunCompass/azimuth/index.html - using Local Hour Angle
            double sinHourAngle = Math.Sin(hourAngle);
            double tanDeclination = sinDeclination / cosDeclination;
            azimuthAngle = Math.Atan2(sinHourAngle, cosHourAngle * sinLatitude - tanDeclination * cosLatitude);
        }

        /// <summary>
        /// Optimized method.
        /// Calculates a value between -1 and 1 telling the angle of the sun from the horizon. Negative value means that the sun is below horizon.
        /// </summary>
        /// <param name="dayOfYear"></param>
        /// <param name="timeOfDayHours"></param>
        /// <param name="sinLatitude">Sine of latitude.</param>
        /// <param name="cosLatitude">Cosine of latitude.</param>
        /// <param name="longitude">Longitude in radians.</param>
        /// <returns>Sine of the angle.</returns>
        public static double SolarElevationAngleSine(int dayOfYear, double timeOfDayHours, double sinLatitude, double cosLatitude, double longitude)
        {
            double n = dayOfYear - 1 + timeOfDayHours * (1f / 24f);
            double sinDeclination = -AxialTiltSine * Math.Cos(AngularSpeedAroundSun * (n + 10) + (Math.PI * Eccentricity) * Math.Sin(AngularSpeedAroundSun * (n - 2)));
            double cosDeclination = Math.Sqrt(1 - sinDeclination * sinDeclination);
            double cosHourAngle = Math.Cos((Math.PI * 2 / 24) * (timeOfDayHours - 12) + longitude);
            return sinLatitude * sinDeclination + cosLatitude * cosDeclination * cosHourAngle;
        }


        /*
          Easy to use overloads.
        */

        /// <summary>
        /// Calculate both elevation and azimuth. Use to avoid calculating them separately.
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <param name="longitude">Angle in degrees.</param>
        /// <param name="elevationAngleSine"></param>
        /// <param name="azimuthAngleSine"></param>
        public static void SolarAngleSines(DateTime utcDateTime, double latitude, double longitude, out double elevationAngleSine, out double azimuthAngleSine)
        {
            latitude *= Deg2Rad;
            longitude *= Deg2Rad;

            // old implementation
            //double timeOfDayHours = utcDateTime.TimeOfDay.TotalHours;
            //double declination = SunDeclinationAccurate(utcDateTime.DayOfYear, timeOfDayHours);
            //double hourAngle = SunHourAngle(timeOfDayHours) + longitude;
            //elevationAngleSine = SolarElevationAngleSine(declination, latitude, hourAngle);
            //azimuthAngleSine = SolarAzimuthAngleSine(declination, Math.Acos(elevationAngleSine), hourAngle);

            SolarAngleSines(utcDateTime.DayOfYear, utcDateTime.TimeOfDay.TotalHours, Math.Sin(latitude), Math.Cos(latitude), longitude, out elevationAngleSine, out azimuthAngleSine);
        }

        /// <summary>
        /// Calculate both elevation and azimuth. Use to avoid calculating them separately.
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <param name="longitude">Angle in degrees.</param>
        /// <param name="elevationAngle">Angle in degrees.</param>
        /// <param name="azimuthAngle">Angle in degrees.</param>
        public static void SolarAngles(DateTime utcDateTime, double latitude, double longitude, out double elevationAngle, out double azimuthAngle)
        {
            latitude *= Deg2Rad;
            longitude *= Deg2Rad;
            SolarAngles(utcDateTime.DayOfYear, utcDateTime.TimeOfDay.TotalHours, Math.Sin(latitude), Math.Cos(latitude), longitude, out elevationAngle, out azimuthAngle);
            elevationAngle *= Rad2Deg;
            azimuthAngle *= Rad2Deg;
        }

        /// <summary>
        /// Calculates a value between -1 and 1 telling the angle of the sun from the horizon. Negative value means that the sun is below horizon.
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <param name="longitude">Angle in degrees.</param>
        /// <returns>Sine of the angle.</returns>
        public static double SolarElevationAngleSine(DateTime utcDateTime, double latitude, double longitude)
        {
            latitude *= Deg2Rad;
            longitude *= Deg2Rad;

            // old implementation
            //double timeOfDayHours = utcDateTime.TimeOfDay.TotalHours;
            //double declination = SunDeclinationAccurate(utcDateTime.DayOfYear, timeOfDayHours);
            //double hourAngle = SunHourAngle(timeOfDayHours) + longitude;
            //return SolarElevationAngleSine(declination, latitude, hourAngle);

            return SolarElevationAngleSine(utcDateTime.DayOfYear, utcDateTime.TimeOfDay.TotalHours, Math.Sin(latitude), Math.Cos(latitude), longitude);
        }

        /// <summary>
        /// Calculates the angle of the sun from the horizon. Negative value means that the sun is below horizon.
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <param name="longitude">Angle in degrees.</param>
        /// <returns>Angle in degrees.</returns>
        public static double SolarElevationAngle(DateTime utcDateTime, double latitude, double longitude)
        {
            return Math.Asin(SolarElevationAngleSine(utcDateTime, latitude, longitude)) * Rad2Deg;
        }

        /// <summary>
        /// Calculates the angle of the sun clockwise from due north.
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <param name="longitude">Angle in degrees.</param>
        /// <returns>Angle in degrees.</returns>
        public static double SolarAzimuthAngle(DateTime utcDateTime, double latitude, double longitude)
        {
            double e, a;
            SolarAngles(utcDateTime, latitude, longitude, out e, out a);
            return a;
        }


        /// <summary>
        /// Calculates the efficiency of the sun at the given latitude, longitude and date and time.
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <param name="longitude">Angle in degrees.</param>
        /// <returns></returns>
        public static double Efficiency(DateTime utcDateTime, double latitude, double longitude)
        {
            return SolarElevationAngleSine(utcDateTime, latitude, longitude);
            //return Math.Exp(-0.17 / SolarElevationAngleSine(utcDateTime, latitude, longitude));
        }


        /*
          Methods used by old implementation.
        */

        private static double SunDeclinationAccurate(int dayOfYear, double timeOfDayHours)
        {
            double n = dayOfYear - 1 + timeOfDayHours * (1f / 24f);
            double sinD = -AxialTiltSine * Math.Cos(AngularSpeedAroundSun * (n + 10) + (Math.PI * Eccentricity) * Math.Sin(AngularSpeedAroundSun * (n - 2)));
            return Math.Asin(sinD);
        }

        private static double SunDeclination(DateTime date)
        {
            double t = date.DayOfYear + 9 + date.TimeOfDay.TotalDays;
            return -AxialTilt * Math.Cos(AngularSpeedAroundSun * t);
        }

        private static double SunHourAngle(double hours)
        {
            return (Math.PI * 2 / 24) * (hours - 12);
        }

        private static double SolarElevationAngleSine(double declination, double latitude, double hourAngle)
        {
            return Math.Sin(latitude) * Math.Sin(declination) + Math.Cos(latitude) * Math.Cos(declination) * Math.Cos(hourAngle);
        }

        private static double SolarAzimuthAngleSine(double declination, double zenithAngle, double hourAngle)
        {
            return -Math.Sin(hourAngle) * Math.Cos(declination) / Math.Sin(zenithAngle);
        }

        /*
          Less accurate method with only local time and latitude.
        */

        /// <summary>
        /// Calculates a value between -1 and 1 telling the angle of the sun from the horizon. Negative value means that the sun is below horizon.
        /// </summary>
        /// <param name="localDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <returns>Sine of the angle.</returns>
        public static double SolarElevationAngleSine(DateTime localDateTime, double latitude)
        {
            if (localDateTime.IsDaylightSavingTime())
                localDateTime = localDateTime.AddHours(-1);
            latitude *= Deg2Rad;
            double timeOfDayHours = localDateTime.TimeOfDay.TotalHours;
            double declination = SunDeclinationAccurate(localDateTime.DayOfYear, timeOfDayHours);
            double hourAngle = SunHourAngle(timeOfDayHours);
            return SolarElevationAngleSine(declination, latitude, hourAngle);
        }

        /// <summary>
        /// Calculates the angle of the sun from the horizon. Negative value means that the sun is below horizon.
        /// </summary>
        /// <param name="localDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <returns>Angle in degrees.</returns>
        public static double SolarElevationAngle(DateTime localDateTime, double latitude)
        {
            return Math.Asin(SolarElevationAngleSine(localDateTime, latitude)) * Rad2Deg;
        }

        /// <summary>
        /// Calculates the efficiency of the sun at the given latitude and date and time.
        /// </summary>
        /// <param name="localDateTime"></param>
        /// <param name="latitude">Angle in degrees.</param>
        /// <returns></returns>
        public static double Efficiency(DateTime localDateTime, double latitude)
        {
            return SolarElevationAngleSine(localDateTime, latitude);
            //return Math.Exp(-0.17 / SolarElevationAngleSine(localDateTime, latitude));
        }



        /*
          TESTING
        */

#if false //(UNITY_EDITOR && DEBUG)

        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void TEST()
        {
            var localDate = new DateTime(2017, 12, 18, 14, 0, 0, DateTimeKind.Local);
            //var date = DateTime.Now;
            var utcDate = localDate.ToUniversalTime();
            var name1 = "Helsinki";
            var lat1 = 60.17;
            var lon1 = 24.94;
            var name2 = "Sodankylä";
            var lat2 = 67.42;
            var lon2 = 26.56;

            TEST_SolarElevation(name1, lat1, lon1, localDate, utcDate);
            TEST_SolarPower(name1, lat1, lon1, localDate, utcDate);

            TEST_SolarElevation(name2, lat2, lon2, localDate, utcDate);
            TEST_SolarPower(name2, lat2, lon2, localDate, utcDate);

            TEST_SolarAngleSines(lat1, lon1);
            TEST_SolarAngleSines_Perf(lat1, lon1);
        }

        private static string GetLocationTimeString(string name, double latitude, double longitude, DateTime localDate)
        {
            return name + " (" + latitude + ", " + longitude + ") " + localDate;
        }

        private static void TEST_SolarElevation(string location, double lat, double lon, DateTime localDate, DateTime utcDate)
        {
            UnityEngine.Debug.Log(GetLocationTimeString(location, lat, lon, localDate) + " - Elevation of the sun (deg): " + SolarElevationAngle(utcDate, lat, lon));
        }

        private static void TEST_SolarPower(string location, double lat, double lon, DateTime localDate, DateTime utcDate)
        {
            UnityEngine.Debug.Log(GetLocationTimeString(location, lat, lon, localDate) + " - Power of the sun (W/m^2): " + WattsPerSquare(utcDate, lat, lon, 1000));
        }

        private static void TEST_SolarAngleSines(double lat = 0, double lon = 0)
        {
            DateTime startDate = new DateTime(2000, 1, 1);
            TimeSpan timeSpan = new TimeSpan(365, 0, 0, 0);
            double deltaHours = timeSpan.TotalHours;
            int iters = 100000;
            double delta = 1f / iters * deltaHours;

            double sinLat = Math.Sin(lat * Deg2Rad);
            double cosLat = Math.Cos(lat * Deg2Rad);
            double radLon = lon * Deg2Rad;

            for (int i = 0; i < iters; ++i)
            {
                var date = startDate.AddHours((i + 1) * delta);
                double elevationSine, azimuthSine;
                SolarAngleSines(date.DayOfYear, date.TimeOfDay.TotalHours, sinLat, cosLat, radLon, out elevationSine, out azimuthSine);

                double elevationSine2, azimuthSine2;
                SolarAngleSines(date, lat, lon, out elevationSine2, out azimuthSine2);

                double elevDiff = Math.Abs(elevationSine - elevationSine2);
                double azimDiff = Math.Abs(azimuthSine - azimuthSine2);

                if (elevDiff > 0.000001)
                {
                    UnityEngine.Debug.Log("Elevation differs: " + elevDiff);
                    break;
                }
                if (azimDiff > 0.000001)
                {
                    UnityEngine.Debug.Log("Azimuth differs: " + azimDiff);
                    break;
                }
            }
        }

        private static void TEST_SolarAngleSines_Perf(double lat = 0, double lon = 0)
        {
            DateTime startDate = new DateTime(2000, 1, 1);
            TimeSpan timeSpan = new TimeSpan(365, 0, 0, 0);
            double deltaHours = timeSpan.TotalHours;
            int iters = 100000;
            double delta = 1f / iters * deltaHours;

            double sinLat = Math.Sin(lat * Deg2Rad);
            double cosLat = Math.Cos(lat * Deg2Rad);
            double radLon = lon * Deg2Rad;

            var sw = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iters; ++i)
            {
                var date = startDate.AddHours((i + 1) * delta);
                double elevationSine, azimuthSine;
                SolarAngleSines(date.DayOfYear, date.TimeOfDay.TotalHours, sinLat, cosLat, radLon, out elevationSine, out azimuthSine);
            }
            sw.Stop();

            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < iters; ++i)
            {
                var date = startDate.AddHours((i + 1) * delta);
                double elevationSine, azimuthSine;
                SolarAngleSines(date, lat, lon, out elevationSine, out azimuthSine);
            }
            sw2.Stop();

            UnityEngine.Debug.Log("SolarAngleSines (Optimized): " + sw.ElapsedMilliseconds + " ms");
            UnityEngine.Debug.Log("SolarAngleSines (Non optimized): " + sw2.ElapsedMilliseconds + " ms");
        }

#endif
    }

}
