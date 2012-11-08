using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BombaJob
{
    public sealed class AppSettings
    {
        private static volatile AppSettings instance;
        private static object syncRoot = new Object();

        #region Constructor
        private AppSettings()
        {
        }

        public static AppSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppSettings();
                    }
                }
                return instance;
            }
        }
        #endregion

        #region Variables
        public static string AppName = "BombaJob";
        public static string SQLiteFile = "BombaJob.sqlite";
        public static string DBConnectionString = "Data Source=" + AppSettings.SQLiteFile;
        public static string ServicesURL = "http://www.bombajob.bg/_mob_service.php";
        public static bool InDebug = true;
        public static string DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        public static int OffersPerPage = 20;
        public static int OffersPerPageMax = 500;
        public static string IconHuman = "Images/iconperson.png";
        public static string IconCompany = "Images/iconcompany.png";

        public enum ServiceOp
        {
            ServiceOpUnknown,
            ServiceOpTexts,
            ServiceOpCategories,
            ServiceOpNewestOffers,
            ServiceOpSearch,
            ServiceOpJobs,
            ServiceOpPost,
            ServiceOpSendEmail,
            ServiceOpSendPM
        }
        #endregion

        #region Helpers
        public static void LogThis(params string[] logs)
        {
            if (AppSettings.InDebug)
            {
                Console.WriteLine("[____BombaJob-Log] " + string.Join(" ", logs));
            }
        }

        public static string DoLongDate(DateTime dt)
        {
            if (dt == null)
                return "";
            string date = "";
            date += dt.ToString("HH:mm");
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("weekday_" + ((int)dt.DayOfWeek + 1));
            date += ", ";
            date += dt.Day;
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("monthFull_" + dt.Month);
            return date;
        }

        public static string DoShortDate(DateTime dt)
        {
            if (dt == null)
                return "";
            string date = "";
            date += dt.ToString("HH:mm");
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("weekday_" + ((int)dt.DayOfWeek + 1));
            date += ", ";
            date += dt.Day;
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("monthShort_" + dt.Month);
            return date;
        }

        public static string Hyperlinkify(string strvar)
        {
            string final = strvar;

            Regex regex = new Regex(@"<nolink>(.*?)</nolink>",
                          RegexOptions.IgnoreCase | RegexOptions.Singleline |
                          RegexOptions.CultureInvariant |
                          RegexOptions.IgnorePatternWhitespace |
                          RegexOptions.Compiled);

            MatchCollection theMatches = regex.Matches(strvar);

            for (int index = 0; index < theMatches.Count; index++)
                final = final.Replace(theMatches[index].ToString(), theMatches[index].ToString().Replace(".", "[[[pk:period]]]"));

            regex = new Regex(@"<a(.*?)</a>", RegexOptions.IgnoreCase |
                    RegexOptions.Singleline | RegexOptions.CultureInvariant |
                    RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            theMatches = regex.Matches(final);

            for (int index = 0; index < theMatches.Count; index++)
            {
                final = final.Replace(theMatches[index].ToString(),
                        theMatches[index].ToString().Replace(".", "[[[pk:period]]]"));
            }

            final = Regex.Replace(final, @"(?<=\d)\.(?=\d)", "[[[pk:period]]]");

            Regex tags = new Regex(@"([a-zA-Z0-9\:/\-]*[a-zA-Z0-9\-_]\" +
                         @".[a-zA-Z0-9\-_][a-zA-Z0-9\-_][a-zA-Z0-9\?\" +
                         @"=&#_\-/\.]*[^<>,;\.\s\)\(\]\[\""])");

            final = tags.Replace(final, "<a href=\"http://$&\">$&</a>");
            final = final.Replace("http://https://", "https://");
            final = final.Replace("http://http://", "http://");

            final = final.Replace("[[[pk:period]]]", ".");
            final = final.Replace("<nolink>", "");
            final = final.Replace("</nolink>", "");

            return final;
        }

        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }
        #endregion

        #region Settings
        public static bool ConfInitSync = false;
        #endregion
    }
}
