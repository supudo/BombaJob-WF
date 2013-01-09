using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

using BombaJob.Database.Domain;
using BombaJob.Utilities.Controls;

using Caliburn.Micro;

using Facebook;

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
        public static bool CaliburnDebug = false;
        public static bool SyncDebug = false;
        public static string DateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        public static int OffersPerPage = 20;
        public static int OffersPerPageMax = 500;
        public static string IconHuman = "/BombaJob;component/Images/iconperson.png";
        public static string IconCompany = "/BombaJob;component/Images/iconcompany.png";
        public static int ConnectivityCheckTimer = 60000;

        public static string FacebookAppID = "162884250446512";
        public static string FacebookPermissions = "publish_stream";

        public static string TwitterOAuth = "https://api.twitter.com/oauth";
        public static string TwitterRequestTokenUri = "https://api.twitter.com/oauth/request_token";
        public static string TwitterAuthorizeUri = "https://api.twitter.com/oauth/authorize";
        public static string TwitterAccessTokenUri = "https://api.twitter.com/oauth/access_token";
        public static string TwitterCallbackUri = "oob";
        public static string TwitterStatusUpdateUrl { get { return "http://api.twitter.com"; } }
        public static string TwitterConsumerKey = "OVvHQ1wio8LZklS5mRUuA";
        public static string TwitterConsumerKeySecret = "zZm0RsfzkLpF3FYnxcM3BDZdxHA6sPLoPiTcBvohUEo";
        public static string TwitterOAuthVersion = "1.0a";

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
        [Conditional("DEBUG")]
        public static void LogThis(params string[] logs)
        {
            if (AppSettings.InDebug)
            {
                Console.WriteLine("[____BombaJob-Log] [" + DateTime.Now.ToString() + "] " + string.Join(" ", logs));
                Trace.WriteLine("[____BombaJob-Log] [" + DateTime.Now.ToString() + "] " + string.Join(" ", logs));
            }
        }

        [Conditional("DEBUG")]
        public static void SyncLogThis(params string[] logs)
        {
            if (AppSettings.SyncDebug && logs != null && logs.Length > 0)
            {
                Console.WriteLine("[____BombaJob-Log-Sync] [" + DateTime.Now.ToString() + "] " + string.Join(" ", logs));
                Trace.WriteLine("[____BombaJob-Log] [" + DateTime.Now.ToString() + "] " + string.Join(" ", logs));
            }
        }

        public static string DoLongDate(DateTime dt)
        {
            if (dt == null)
                return "";
            string date = "";
            date += dt.ToString("HH:mm");
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("weekday_" + ((int)dt.DayOfWeek));
            date += ", ";
            date += dt.Day;
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("monthFull_" + dt.Month);
            if (dt.Year != DateTime.Now.Year)
                date += ", " + dt.Year;
            return date;
        }

        public static string DoShortDate(DateTime dt)
        {
            if (dt == null)
                return "";
            string date = "";
            date += dt.ToString("HH:mm");
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("weekday_" + ((int)dt.DayOfWeek));
            date += ", ";
            date += dt.Day;
            date += " ";
            date += Properties.Resources.ResourceManager.GetString("monthShort_" + dt.Month);
            if (dt.Year != DateTime.Now.Year)
                date += ", " + dt.Year;
            return date;
        }

        public static string Hyperlinkify(string strvar)
        {
            if (strvar == null || strvar.Trim().Equals(""))
                return "";

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

        public static string HyperlinkifyAll(string strvar, string param)
        {
            if (strvar == null || strvar.Trim().Equals(""))
                return "";

            string final = strvar;
            string section = String.Empty;
            Regex regex;
            MatchCollection theMatches;
 
            if (InStr(final, "<nolink>") > 0)
            {          
                regex = new Regex(@"<nolink>(.*?)</nolink>", RegexOptions.IgnoreCase |
                                                             RegexOptions.Singleline |
                                                             RegexOptions.CultureInvariant |
                                                             RegexOptions.IgnorePatternWhitespace |
                                                             RegexOptions.Compiled);
          
                theMatches = regex.Matches(final);
          
                for (int index = 0; index < theMatches.Count; index++)
                {
                    final = final.Replace(theMatches[index].ToString(), theMatches[index].ToString().Replace(".", "[pk:period]"));
                }
            }
     
            if (InStr(final, "@") > 0)
            {          
                regex = new Regex(@"([a-zA-Z_0-9.-]+\@[a-zA-Z_0-9.-]+\.\w+)", RegexOptions.IgnoreCase |
                                                                              RegexOptions.Singleline |
                                                                              RegexOptions.CultureInvariant |
                                                                              RegexOptions.IgnorePatternWhitespace |
                                                                              RegexOptions.Compiled);
          
                theMatches = regex.Matches(final);
          
                for (int index = 0; index < theMatches.Count; index++)
                {
                    final = final.Replace(theMatches[index].ToString(), "<a href=\"mailto:" + theMatches[index].ToString() + "\">" + theMatches[index].ToString() + "</a>");
                }
            }
          
            if (InStr(final, "<a") > 0)
            {          
                regex = new Regex(@"<a(.*?)</a>", RegexOptions.IgnoreCase |
                                                  RegexOptions.Singleline |
                                                  RegexOptions.CultureInvariant |
                                                  RegexOptions.IgnorePatternWhitespace |
                                                  RegexOptions.Compiled);
          
                theMatches = regex.Matches(final);
          
                for (int index = 0; index < theMatches.Count; index++)
                {
                    final = final.Replace(theMatches[index].ToString(), theMatches[index].ToString().Replace(".", "[pk:period]"));
                }
            }
     
            final = Regex.Replace(final, @"(?<=\d)\.(?=\d)", "[pk:period]");
     
            Regex tags = new Regex(@"([a-zA-Z\:/]*[a-zA-Z_0-9.-]+\.\w+[a-zA-Z0-9\?\=&#_\-/\.]*[^<>,;\.\s\)\(\]\[\""])", RegexOptions.IgnoreCase |
                                                                                                                        RegexOptions.Singleline |
                                                                                                                        RegexOptions.CultureInvariant |
                                                                                                                        RegexOptions.IgnorePatternWhitespace |
                                                                                                                        RegexOptions.Compiled);
     
            theMatches = tags.Matches(final);
     
            for (int index = 0; index < theMatches.Count; index++)
            {
                section = theMatches[index].ToString();
          
                if (InStr(section, "://") < 1)
                    section = "http://" + section;
               
                final = final.Replace(theMatches[index].ToString(), "<a href=\"" + section + "\" " + param + ">" + theMatches[index].ToString() + "</a>");
            }
 
            final = final.Replace("[pk:period]", ".");
            final = final.Replace("<nolink>", "");
            final = final.Replace("</nolink>", "");
 
            return final;
        }

        public static int InStr(int Start, string str1, string str2)
        {
            if (Start > 0 && str1.Length >= Start)
                return str1.IndexOf(str2, Start - 1) + 1;
            else
                return -1;
        }

        public static int InStr(string str1, string str2)
        {
            return InStr(1, str1, str2);
        }

        public static bool ValidateEmail(string email)
        {
            return Regex.IsMatch(email, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
        }
        #endregion

        #region SocNet
        public static void FacebookPost(FacebookOAuthResult facebookOAuthResult, JobOffer jobOffer)
        {
            if (facebookOAuthResult == null)
                return;

            if (facebookOAuthResult.IsSuccess)
            {
                AppSettings.LogThis("Facebook token = " + facebookOAuthResult.AccessToken);
                string _accessToken = facebookOAuthResult.AccessToken;
                var fb = new FacebookClient(_accessToken);

                fb.PostCompleted += (o, e) =>
                {
                    if (e.Cancelled)
                        return;
                    else if (e.Error != null)
                        Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(Properties.Resources.share_FacebookError + "\n" + e.Error.Message, Properties.Resources.errorTitle, MessageBoxButton.OK));
                    else
                        Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(Properties.Resources.share_FacebookOK, Properties.Resources.share_Facebook, MessageBoxButton.OK));
                };

                dynamic parameters = new ExpandoObject();
                parameters.picture = "http://bombajob.bg/images/ibombajob.png";
                parameters.name = jobOffer.Title;
                parameters.link = "http://bombajob.bg/offer/" + jobOffer.OfferID;
                parameters.caption = jobOffer.Positivism;
                parameters.description = jobOffer.Negativism;

                fb.PostAsync("me/feed", parameters);
            }
            else
                return;// Caliburn.Micro.Execute.OnUIThread(() => IoC.Get<IWindowManager>().ShowMessageBox(facebookOAuthResult.ErrorDescription, Properties.Resources.errorTitle, MessageBoxButton.OK));
        }

        public static void TwitterPost(JobOffer jobOffer)
        {
            string tw = "BombaJob.bg - ";
            tw += jobOffer.Title + " - ";
            tw += "http://bombajob.bg/offer/" + jobOffer.OfferID;
            tw += " #bombajobbg";
        }
        #endregion
    }
}
