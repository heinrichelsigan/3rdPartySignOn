using ThirdPartySignOn.MSIdentity.Data;
using System.Diagnostics;
using System.Reflection;

namespace ThirdPartySignOn.MSIdentity.Data
{
    
    /// <summary>
    /// simple static logger via NLog
    /// </summary>
    public class ThirdPartySignOnLog
    {

        #region static fields and properties

        private static readonly Lock _lock = new Lock(), _outerLock = new Lock();
        private static readonly Lazy<ThirdPartySignOnLog> instance = new Lazy<ThirdPartySignOnLog>(() => new ThirdPartySignOnLog());

        private static int checkedToday = DateTime.UtcNow.Date.Day;

        /// <summary>
        /// Get the Logger
        /// </summary>
        public static ThirdPartySignOnLog Logger { get => instance.Value; }

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
        static ThirdPartySignOnLog()
        {
            LogFile = Path.Combine(SettingsKeyReader.LogFilePath, DateTime.Now.LogFileDate());
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
            AppName = (!string.IsNullOrEmpty(appName)) ? appName : String.Empty;
			ThirdPartySignOnLog.LogFileByAppName(appName);			
        }

        public static string LogFileByAppName(string appName = "")
        {
            LogFile = (!string.IsNullOrEmpty(appName)) ?
                Path.Combine(SettingsKeyReader.LogFilePath, DateTime.Now.AppLogFile(appName)) :
                Path.Combine(SettingsKeyReader.LogFilePath, DateTime.Now.LogFileDate()); 
			return LogFile;				
        }

        /// <summary>
        /// Log - static logging method
        /// </summary>
        /// <param name="msg">message to log</param>
        /// <param name="appName">application name</param>
        public static void Log(string msg, string appName = "")
        {
            string logMsg = string.Empty, errMsg = string.Empty, allLogMsg = string.Empty;

			// Create Logfile, if logfile doesn't exist or new day
            if (string.IsNullOrEmpty(LogFile) || !CheckedToday || !File.Exists(LogFile))
            {
                LogFile = LogFileByAppName(appName);
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
                            errMsg = String.Format("{0} \tCreating logfile {1} Exception {2} {3} \n\t{4}\n",
								DateTime.Now.ThirdPartySignOnDateTimeWithSeconds(), 
								LogFile, exLogFiteCreate.GetType(), exLogFiteCreate.Message, exLogFiteCreate.ToString());
							Console.Error.WriteLine(errMsg);
							AppDomain.CurrentDomain.SetData("LOG_EXCEPTION_STATIC", errMsg);                            
                        }
                    }
                }
            }

			// Write buffered log lines first
			try
			{
				if ((AppDomain.CurrentDomain.GetData("ALL_KEYS") != null) &&
					((allLogMsg = (string)AppDomain.CurrentDomain.GetData("ALL_KEYS")) != null) && 
					!string.IsNullOrEmpty(allLogMsg))
				{
					lock (_lock)
					{
						File.AppendAllText(LogFile, allLogMsg, System.Text.Encoding.UTF8);
						allLogMsg = ""; // empty allLogMsg and set Buffer to empty
						AppDomain.CurrentDomain.SetData("ALL_KEYS", allLogMsg);
					}
				}
			}
			catch (Exception exLog)
			{
				lock (_lock)
				{
					errMsg = String.Format("{0} \tWriting to file {1} Exception {2} {3} \n{4}\n",
						DateTime.Now.ThirdPartySignOnDateTimeWithSeconds(), LogFile, exLog.GetType(), exLog.Message, exLog.ToString());
					AppDomain.CurrentDomain.SetData("LOG_EXCEPTION_STATIC", errMsg);
					Console.Error.WriteLine(errMsg);
				}
			}

			// create logMsg and write it to LogFile, when failed => buffer logMsg
			logMsg = DateTime.Now.ThirdPartySignOnDateTimeWithSeconds() + "\t " + 
				(string.IsNullOrEmpty(msg) ? string.Empty : (msg.EndsWith("\n") ? msg : msg + "\n"));
			try
			{
				lock (_lock)
				{
					File.AppendAllText(LogFile, logMsg, System.Text.Encoding.UTF8);
				}
			}
			catch (Exception exLogWrite)
			{
				lock (_lock)
				{
					errMsg = String.Format("{0} \tWriting to file {1} Exception {2} {3} \n{4}\n",
						DateTime.Now.ThirdPartySignOnDateTimeWithSeconds(), LogFile, exLogWrite.GetType(), exLogWrite.Message, exLogWrite.ToString());
					AppDomain.CurrentDomain.SetData("LOG_EXCEPTION_STATIC", errMsg);
					Console.Error.WriteLine(errMsg);
					
					if (AppDomain.CurrentDomain.GetData("ALL_KEYS") != null)
						allLogMsg = (string)AppDomain.CurrentDomain.GetData("ALL_KEYS") ?? string.Empty;
					allLogMsg += "\n" + logMsg;
					AppDomain.CurrentDomain.SetData("ALL_KEYS", allLogMsg);						
				}
			}
        }

        /// <summary>
        /// Log - static logging method
        /// </summary>
        /// <param name="exLog"><see cref="Exception"/> to log</param>
        /// <param name="appName">application name</param>
        public static void Log(Exception exLog, string appName = "")
        {            
            string excMsg = String.Format("{0} throwed {1} ⇒ {2}\t{3}\n\tStacktrace: \t{4}\n",
                typeof(ThirdPartySignOnLog).GetCallerInfo(1),
                exLog.GetType(),
                exLog.Message,
                exLog.ToString().Replace("\r", "").Replace("\n", " "),
                exLog.StackTrace?.Replace("\r", "").Replace("\n", " "));

            Log(excMsg, appName);
        }

        public static void LogStatic(string msg, string appName = "") => ThirdPartySignOnLog
			.Log(msg + appName, msg);

        public static void LogStatic(string prefix, Exception xZpd, string appName) => ThirdPartySignOnLog
			.LogOriginMsgEx(appName, prefix, xZpd);

        public static void LogStatic(Exception ex, string appName = "") => ThirdPartySignOnLog
			.Log(ex, appName);

        /// <summary>
        /// Log origin with message to NLog
        /// </summary>
        /// <param name="origin">origin of message</param>
        /// <param name="message">enabler message to log</param>
        /// <param name="level">log level: 0 for Trace, 1 for Debug, ..., 4 for Error, 5 for Fatal</param>
        public static void LogOriginMsg(string origin, string message, int level = 2) => ThirdPartySignOnLog
			.LogStatic((string.IsNullOrEmpty(origin) ? "  \t" : origin + " \t") + message);
        

        public static void LogOriginEx(string origin, Exception ex, int level = 2)
        {
            string logPrefix = string.IsNullOrEmpty(origin) ? "   " : origin;
            LogStatic($"{logPrefix} \t{ex.GetType()}: \t{ex.Message}");
            LogStatic($"{logPrefix} \t{ex.GetType()}: \t{ex.ToString()}");
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

        #endregion static members

    }


    /// <summary>
    /// Extension methods for logger
    /// </summary>
    public static class Extensions
    {
        #region DateTime extensions

        /// <summary>
        /// ThirdPartySignOnDate extension method for DateTime
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> to format</param>
        /// <returns>formatted date <see cref="string"/></returns>
        public static string ThirdPartySignOnDate(this DateTime dt) => dt
			.ToString("yyyy-MM-dd");

        public static string LogFileDate(this DateTime dt) => String
			.Concat(dt.ThirdPartySignOnDate(), "_log.txt");
        public static string AppLogFile(this DateTime dt, string appName)  => String
			.Concat(dt.ThirdPartySignOnDate(), $"_{appName}.log");


        /// <summary>
        /// ThirdPartySignOnDateTime extension method for DateTime
        /// </summary>
        /// <param name="dateTime"><see cref="DateTime"/> to format</param>
        /// <returns>formatted date time <see cref="string"/> </returns>
        public static string ThirdPartySignOnDateTime(this DateTime dt) => dt
			.ToString("yyyy-MM-dd HH:mm ");

        /// <summary>
        /// ThirdPartySignOnDateTimeWithSeconds extension method for DateTime
        /// </summary>
        /// <param name="dateTime">d</param>
        /// <returns><see cref="string"/> formatted date time including seconds</returns>
        public static string ThirdPartySignOnDateTimeWithSeconds(this DateTime dt) => dt
			.ToString("yyyy-MM-dd_HH:mm:ss");
        
        public static string ThirdPartySignOnDateTimeWithMillis(this DateTime dt) => String
            .Format("{0:yyyyMMdd_HHmmss}_{1}", dt, dt.Millisecond);

        public static string ThirdPartySignOnDateTimePrecise(this DateTime dt) => String
			.Format("{0:yyyyMMdd_HHmmss}", dt);

        #endregion DateTime extensions


        /// <summary>
        /// GetCallerInfo extension method for Type to get caller information for logging
        /// </summary>
        /// <param name="type"><see cref="Type">calling Type</see></param>
        /// <param name="skipFrames">skip last n frames</param>
        /// <returns>caller information as <see cref="string"/></returns>
        public static string GetCallerInfo(this Type type, int skipFrames = 1)
        {
            string fullName = "unknown";
            try
            {                
                if (type != null)
                {
                    if (type.DeclaringType != null)
                        fullName = (type.DeclaringType.FullName != null) ? type.DeclaringType.FullName.ToString() : type.DeclaringType.Name.ToString();
                    else 
                        fullName = (type.FullName != null) ? type.FullName.ToString() : type.Name.ToString();
                } 
            } 
            catch
            {
                fullName = "unknown";
            }
            try
            {
                StackFrame frame = new StackFrame(skipFrames + 1);
                MethodBase method = frame.GetMethod();
                fullName = $"{method?.DeclaringType?.FullName}.{method?.Name}";
            }
            catch (Exception ex)
            {
                ThirdPartySignOnLog.LogOriginMsgEx("ThirdPartySignOn.Saml.Extensions", 
                    $"Exception in GetCallerInfo(this Type type = {type.ToString()}, int skipFrames = {skipFrames})",
                    ex);
            }

            return fullName;
        }            

    }

}
