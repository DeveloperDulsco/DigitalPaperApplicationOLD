using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DigiDoc.Helper
{
    public class DateTimeHelper
    {

        public static DateTime ConvertFromUTC(DateTime dateTime, string timeZone)
        {
            DateTime currentTimezone = dateTime;
            bool exception=false;
            try
            {
               
                TimeZoneInfo currentZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                currentTimezone = TimeZoneInfo.ConvertTimeFromUtc(dateTime, currentZone);
                var s=TimeZoneInfo.GetSystemTimeZones();
            }
            catch (Exception ex)
            {
                exception = true;
               
               
                
            }
            finally
            {
                if (exception)
                {
                    TimeZone currentZone = TimeZone.CurrentTimeZone;
                    LogHelper.Instance.Debug("Current Time Zone" + currentZone.StandardName, "DateTimeHelper", "Portal", "Document");
                    TimeZoneInfo currentZoneINFO = TimeZoneInfo.FindSystemTimeZoneById(currentZone.StandardName);
                    LogHelper.Instance.Debug("Current Time Zone info" + currentZoneINFO, "DateTimeHelper", "Portal", "Document");
                    currentTimezone = TimeZoneInfo.ConvertTimeFromUtc(dateTime, currentZoneINFO);
                    LogHelper.Instance.Debug("Current Time" + currentTimezone, "DateTimeHelper", "Portal", "Document");

                }
            }
            return currentTimezone;
        }

    }
}