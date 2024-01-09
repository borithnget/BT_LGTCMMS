using BT_KimMex.Class;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace BT_KimMex.ErrorLog
{
    public class ErrorLogger
    {


        /// <summary>
        /// To formate the log Message
        /// Created By      :    Oum Chantola
        /// Created Date    :   09/April/2018
        /// </summary>
        /// <returns></returns>

        public static string Format()
        {
            string pattern = string.Empty;
            for (int count = 0; count < 2; count++)
            {
                pattern = pattern + "---------------------------------------------------------------------------";
            }
            return pattern;
        }
        private static string GetFormattedDate()
        {
            string date = DateTime.Now.ToShortDateString();
            string formattedDate = string.Empty;
            char oldChar = '/';
            char newChar = '-';
            formattedDate = date.Replace(oldChar, newChar);
            return formattedDate;
        }
        /// <summary>
        /// To get log file Location 
        /// Created By      :   Oum Chantola
        /// Created Date    :   09/April/2018
        /// </summary>
        /// <returns></returns>
        private static string GetLogPath()
        {
            string formattedDate = GetFormattedDate();
            string TraceFileName = formattedDate + ConfigurationManager.AppSettings["TraceFileName"];
            string TraceFolderPath = ConfigurationManager.AppSettings["LogLocation"] + ConfigurationManager.AppSettings["TraceFolderPath"];

            if (!TraceFolderPath.EndsWith("\\"))
            {
                TraceFolderPath = TraceFolderPath + "\\";
            }
            string TraceFilePath = TraceFolderPath + TraceFileName;
            try
            {
                if (!Directory.Exists(TraceFolderPath))
                {
                    Directory.CreateDirectory(TraceFolderPath);
                }
            }
            catch (Exception Ex)
            {
                throw Ex;
            }
            return TraceFilePath;
        }
        /// <summary>
        /// Writting to the Log File 
        /// Created By          :   Oum Chantola
        /// Created Date    :   09/April/2018
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="EX"></param>
        private static void ProcessIncomingMessages(TextWriter writer, string className, string methodName, EnumConstants.ErrorType errorType, string exceptionString, string message)
        {
            writer.Write("TIME        :: ");
            writer.Write(DateTime.Now.ToString());
            writer.WriteLine();

            writer.Write("Class Name       :: ");
            writer.Write(className.ToString());
            writer.WriteLine();

            writer.Write("Method Name        :: ");
            writer.Write(methodName.ToString());
            writer.WriteLine();

            writer.Write("INFORMATION TYPE :: ");
            writer.Write(errorType.ToString());
            writer.WriteLine();

            writer.Write("EXCEPTION   :: ");
            if (string.Equals(errorType.ToString(), EnumConstants.ErrorType.Error.ToString()))
                writer.Write(exceptionString.ToString());
            else
                writer.Write("NULL ");
            writer.WriteLine();

            writer.Write("MESSAGE     :: ");
            writer.Write(message.ToString());
            writer.WriteLine();
            writer.Write(Format());
            writer.WriteLine();

        }
        /// <summary>
        /// Saves System.Exception or Information to Log file based on predeine location path.
        /// Created By      :   Oum Chantola
        /// Created Date    :   09/April/2018
        /// </summary>
        /// <param name="errType">type of info to be stored</param>
        /// <param name="EX">System Exception</param>
        /// <param name="errorLogFilePath">File to log</param>
        public static void LogEntry(EnumConstants.ErrorType errorType, string className, string methodName, string exceptionString, string message)
        {
            string errorLogFilePath = GetLogPath();
            if (errorLogFilePath != null)
            {
                try
                {
                    using (FileStream LogFileStream = File.Open(errorLogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        using (StreamWriter LogWriter = new StreamWriter(LogFileStream))
                        {
                            ProcessIncomingMessages(LogWriter, className, methodName, errorType, exceptionString, message);
                        }
                    }
                }
                catch (Exception Ex)
                {
                    throw Ex;
                }
                finally
                {
                }
            }
        }

        public static void submitLogEntry(EnumConstants.ErrorType errorType, string className, string methodName, string exceptionString, string message)
        {
            try
            {
                using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
                {
                    Entities.tb_error_log errorLog = new Entities.tb_error_log();
                    errorLog.error_id = Guid.NewGuid().ToString();
                    errorLog.created_at = CommonClass.ToLocalTime(DateTime.Now);
                    errorLog.class_name = className;
                    errorLog.method_name = methodName;
                    errorLog.information_type = errorType.ToString();
                    errorLog.exception = exceptionString;
                    errorLog.message = message;
                    db.tb_error_log.Add(errorLog);
                    db.SaveChanges();
                }
            }catch(Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        public static List<Models.ErrorLogViewModel> GetErrorLogs()
        {
            using(Entities.kim_mexEntities db=new Entities.kim_mexEntities())
            {
                List<Models.ErrorLogViewModel> errorLogs = new List<Models.ErrorLogViewModel>();
                errorLogs = db.tb_error_log.OrderByDescending(o => o.created_at).Select(s => new Models.ErrorLogViewModel()
                {
                    error_id=s.error_id,
                    created_at=s.created_at,
                    class_name=s.class_name,
                    method_name=s.method_name,
                    information_type=s.information_type,
                    exception=s.exception,
                    message=s.message,
                }).ToList();
                return errorLogs;
            }
        }
    }
}