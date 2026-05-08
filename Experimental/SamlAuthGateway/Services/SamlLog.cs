using System.Diagnostics;
using System.Reflection;

namespace SamlAuthGateway.Services
{

    /// <summary>
    /// simple static logger via NLog
    /// </summary>
    public class SamlLog
    {

        #region static fields and properties

        private static readonly object _lock = new object(), _outerLock = new object();
        private static readonly Lazy<SamlLog> instance = new Lazy<SamlLog>(() => new SamlLog());

        private static int checkedToday = DateTime.UtcNow.Date.Day;

        private static readonly SettingsKeyReader _settingsKeys = new SettingsKeyReader();
        internal static SettingsKeyReader SettingsKeys { get => SamlLog._settingsKeys; }

        /// <summary>
        /// Get the Logger
        /// </summary>
        public static SamlLog Logger { get => instance.Value; }

        /// <summary>
        /// Checked today if logfiles and other needed resources exist
        /// </summary>
        public static bool CheckedToday
        {
            get
            {
                if (DateTime.UtcNow.Day == checkedToday)
                    return true;

                checkedToday = DateTime.UtcNow.Day;
                return false;
            }
        }

        public static string AppName { get; private set; } = string.Empty;

        /// <summary>
        /// LogFile
        /// </summary>
        public static string LogFile { get; private set; }

        #endregion static fields and properties

        #region ctor

        /// <summary>
        /// private Singelton constructor
        /// </summary>
        static SamlLog()
        {
            _settingsKeys = _settingsKeys ?? new SettingsKeyReader();
            LogFile = Path.Combine(SettingsKeys.LogFilePath, DateTime.Now.LogFileDate());
            InitLog("");
        }

        #endregion ctor

        #region static members

        /// <summary>
        /// InitLog init Log configuration
        /// </summary>
        /// <param name="appName">application name</param>
        protected internal static void InitLog(string appName = "")
        {
            if (!string.IsNullOrEmpty(appName))
                AppName = appName;

            if (!string.IsNullOrEmpty(AppName))
                LogFile = Path.Combine(SettingsKeys.LogFilePath, DateTime.Now.AppLogFile(appName));  
            else
                LogFile = Path.Combine(SettingsKeys.LogFilePath, DateTime.Now.LogFileDate());
        }

        public static void SetLogFileByAppName(string appName = "")
        {
            LogFile = (!string.IsNullOrEmpty(appName)) ?
                Path.Combine(SettingsKeys.LogFilePath, DateTime.Now.AppLogFile(appName)) :
                Path.Combine(SettingsKeys.LogFilePath, DateTime.Now.LogFileDate()); 
        }

        /// <summary>
        /// Log - static logging method
        /// </summary>
        /// <param name="msg">message to log</param>
        /// <param name="appName">application name</param>
        public static void Log(string msg, string appName = "")
        {
            string logMsg = string.Empty, errMsg = string.Empty, allLogMsg = string.Empty;

            //lock (_outerLock)
            //{
            if (string.IsNullOrEmpty(LogFile) || !CheckedToday || !File.Exists(LogFile))
            {
                LogFile = Path.Combine(SettingsKeys.LogFilePath, DateTime.Now.LogFileDate());


                if (!File.Exists(LogFile))
                {
                    lock (_lock)
                    {
                        try
                        {
                            File.Create(LogFile);
                        }
                        catch (Exception exLogFiteCreate)
                        {
                            ; // throw
                            Console.Error.WriteLine("Exception creating logfile: " + exLogFiteCreate.ToString());
                        }
                    }
                }
            }
            //}

            try
            {
                allLogMsg = GetAppDomainData("ALL_KEYS");
                if (!string.IsNullOrEmpty(allLogMsg))
                {
                    lock (_lock)
                    {
                        File.AppendAllText(LogFile, allLogMsg, System.Text.Encoding.UTF8);
                        allLogMsg = "";
                        SetAppDomainData("ALL_KEYS", allLogMsg);
                    }
                }
            }
            catch (Exception exLog)
            {
                errMsg = String.Format("{0} \tWriting to file {1} Exception {2} {3} \n{4}\n",
                    DateTime.Now.Enabler4BizDateTimeWithSeconds(), LogFile, exLog.GetType(), exLog.Message, exLog.ToString());
                SetAppDomainData("LOG_EXCEPTION_STATIC", errMsg);
                // Console.Error.WriteLine(errMsg);
            }

            logMsg = DateTime.Now.Enabler4BizDateTimeWithSeconds() + "\t " + (string.IsNullOrEmpty(msg) ? string.Empty : (msg.EndsWith("\n") ? msg : msg + "\n"));
            try
            {
                lock (_lock)
                {
                    File.AppendAllText(LogFile, logMsg, System.Text.Encoding.UTF8);
                }
            }
            catch (Exception exLogWrite)
            {
                errMsg = String.Format("{0} \tWriting to file {1} Exception {2} {3} \n{4}\n",
                    DateTime.Now.Enabler4BizDateTimeWithSeconds(), LogFile, exLogWrite.GetType(), exLogWrite.Message, exLogWrite.ToString());
                AppDomain.CurrentDomain.SetData("LOG_EXCEPTION_STATIC", errMsg);
                allLogMsg = GetAppDomainData("ALL_KEYS");
                allLogMsg += (string.IsNullOrEmpty(allLogMsg)) ? $"{logMsg}\n" : $"\n{logMsg}\n";
                SetAppDomainData("ALL_KEYS", allLogMsg);
                // Console.Error.WriteLine(errMsg);
            }
            // }
        }

        /// <summary>
        /// Log - static logging method
        /// </summary>
        /// <param name="exLog"><see cref="Exception"/> to log</param>
        /// <param name="appName">application name</param>
        public static void Log(Exception exLog, string appName = "")
        {
            string methodBase = "unknown";
            try
            {
                MethodBase? mBase = new StackFrame(1)?.GetMethod();
                methodBase = mBase?.ToString() ?? "unknown";
            }
            catch
            {
                methodBase = "unknown";
            }

            string excMsg = String.Format("{0} throwed {1} ⇒ {2}\t{3}\nStacktrace: \t{4}\n",
                methodBase,
                exLog.GetType(),
                exLog.Message,
                exLog.ToString().Replace("\r", "").Replace("\n", " "),
                exLog.StackTrace?.Replace("\r", "").Replace("\n", " "));

            Log(excMsg, appName);
        }

        public static void LogStatic(string msg, string appName = "") => SamlLog.Log(msg + appName, msg);

        public static void LogStatic(string prefix, Exception xZpd, string appName) => SamlLog.LogOriginMsgEx(appName, prefix, xZpd);

        public static void LogStatic(Exception ex, string appName = "") => SamlLog.Log(ex, appName);

        /// <summary>
        /// Log origin with message to NLog
        /// </summary>
        /// <param name="origin">origin of message</param>
        /// <param name="message">enabler message to log</param>
        /// <param name="level">log level: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public static void LogOriginMsg(string origin, string message, int level = 2)
        {
            string logMsg = (string.IsNullOrEmpty(origin) ? "  \t" : origin + " \t") + message;
            LogStatic(logMsg);
        }

        public static void LogOriginEx(string origin, Exception ex, int level = 2)
        {
            string logPrefix = string.IsNullOrEmpty(origin) ? "   " : origin;
            LogStatic($"{logPrefix} \tException {ex.GetType()}: \t{ex.Message}");
            LogStatic($"{logPrefix} \tException {ex.GetType()}: \t{ex.ToString()}");
            if (level < 2)
                LogStatic($"{logPrefix} \t{ex.GetType()} StackTrace: \t{ex.StackTrace}");
        }

        /// <summary>
        /// Log origin with message and thrown exception to NLog
        /// </summary>
        /// <param name="origin">origin of message</param>
        /// <param name="message">logging <see cref="string">string message</see></param>
        /// <param name="ex">logging <see cref="Exception">Exception ex</see></param>
        /// <param name="level"><see cref="int">int log level</see>: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public static void LogOriginMsgEx(string origin, string message, Exception ex, int level = 2)
        {
            string logPrefix = string.IsNullOrEmpty(origin) ? "   " : origin;
            LogStatic($"{logPrefix} \t{message} {ex.GetType()}: \t{ex.Message}");
            LogStatic($"{logPrefix} \tException {ex.GetType()}: \t{ex.ToString()}");
            if (level < 2)
                LogStatic($"{logPrefix} \t{ex.GetType()} StackTrace: \t{ex.StackTrace}");
        }


        public static string GetAppDomainData(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            object? o = null;
            try
            {
                o = AppDomain.CurrentDomain.GetData(key);
            }
            catch (Exception apDomREx)
            {
                string appDomExMsg = String.Format("{0} \tReading from domain id {1} Exception {2} {3} \n{4}\n",
                   DateTime.Now.Enabler4BizDateTimeWithSeconds(), AppDomain.CurrentDomain.Id, apDomREx.GetType(), apDomREx.Message, apDomREx.ToString());
                Console.Error.WriteLine(appDomExMsg);
            }
            return (o != null) ? o?.ToString() ?? "" : "";
        }

        public static void SetAppDomainData(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            try
            {
                AppDomain.CurrentDomain.SetData(key, value);
            }
            catch (Exception apDomWEx)
            {
                string appDomExWMsg = String.Format("{0} \tWriting to domain id {1} Exception {2} {3} \n{4}\n",
                   DateTime.Now.Enabler4BizDateTimeWithSeconds(), AppDomain.CurrentDomain.Id, apDomWEx.GetType(), apDomWEx.Message, apDomWEx.ToString());
                Console.Error.WriteLine(appDomExWMsg);
            }
        }

        #endregion static members

    }


    /// <summary>
    /// Extension methods for logger
    /// </summary>
    public static class Extensions
    {
        #region DateTime extensions

        /// <summary>
        /// Area23Date extension method for DateTime
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/></param>
        /// <returns>formatted date <see cref="string"/></returns>
        public static string Enabler4BizDate(this DateTime dt) => dt.ToString("yyyy-MM-dd");

        public static string LogFileDate(this DateTime dt) => String.Concat(dt.Enabler4BizDate(), "_log.txt");

        public static string AppLogFile(this DateTime dt, string appName)  => String.Concat(dt.Enabler4BizDate(), $"_{appName}.log");

        /// <summary>
        /// Area23DateTime extension method for DateTime
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/></param>
        /// <returns>formatted date time <see cref="string"/> </returns>
        public static string Enabler4BizDateTime(this DateTime dt) => dt.ToString("yyyy-MM-dd HH:mm ");

        /// <summary>
        /// Area23DateTimeWithSeconds extension method for DateTime
        /// </summary>
        /// <param name="dateTime">d</param>
        /// <returns><see cref="string"/> formatted date time including seconds</returns>
        public static string Enabler4BizDateTimeWithSeconds(this DateTime dt) => dt.ToString("yyyy-MM-dd_HH:mm:ss");
        
        public static string Enabler4BizDateTimeWithMillis(this DateTime dt) => String
            .Format("{0:yyyyMMdd_HHmmss}_{1}", dt, dt.Millisecond);

        public static string Enabler4BizDateTimePrecise(this DateTime dt) => String
            .Format("{0:yyyyMMdd_HHmmss}", dt);

        #endregion DateTime extensions

    }

}
