using System;
using System.IO;
using System.Text;

namespace WebServer.Utils
{
    public static class Logs
    {
        public static void Add(string currentMethodName, string exception)
        {
            try
            {
                FileStream fs;
                string day = (DateTime.Now.Day < 10) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
                string month = (DateTime.Now.Month < 10) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
                string dataTime = String.Format("{0}.{1}.{2}", DateTime.Now.Year, month, day);
                string message = DateTime.Now.ToString() + "Произошла ошибка в методе " + currentMethodName + ": " + exception;
                string fullName = "C:\\Logs\\{dataTime}Log_WebServer.txt";
                FileInfo fi = new FileInfo(fullName);

                if (fi.Exists)
                {
                    fs = new FileStream(fullName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    fs = new FileStream(fullName, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                }

                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(1251));
                sw.Write(message);
                sw.Close();
                fs.Close();
            }
            catch (Exception) { }
        }
    }
}