using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _9M.Work.Utility
{
    public class NetWorkUntity
    {

        public NetWorkUntity() { }

        public static bool connectState(string path)
        {
            return connectState(path, "", "");
        }

        public static bool connectState(string path, string userName, string passWord)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + path + " /User:" + userName + " " + passWord + " /PERSISTENT:YES";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    throw new Exception(errormsg);
                }
            }
            catch (Exception ex)
            {
             //   throw ex;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }


        public static bool clearState(string path)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                string dosLine = @"net use " + path + " /Delete";
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
            }

            catch (Exception ex)
            {

            }

            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return Flag;
        }
        //read file

        public static string ReadtxtFiles(string path)
        {

            try
            {

                // Create an instance of StreamReader to read from a file.

                // The using statement also closes the StreamReader.

                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {

                    String line = sr.ReadToEnd();

                    return line;

                }

            }

            catch (Exception e)
            {

                // Let the user know what went wrong.

                Console.WriteLine("The file could not be read:");

                Console.WriteLine(e.Message);

                return "";

            }

        }

        //write file

        public static void WriteFiles(string path)
        {

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.Write("This is the ");
                    sw.WriteLine("header for the file.");
                    sw.WriteLine("-------------------");
                    // Arbitrary objects can also be written to the file.
                    sw.Write("The date is: ");
                    sw.WriteLine(DateTime.Now);
                }
            }

            catch (Exception e)
            {

                // Let the user know what went wrong.

                Console.WriteLine("The file could not be read:");

                Console.WriteLine(e.Message);

            }

        }

    }
}
