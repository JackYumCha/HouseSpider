using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpiderController
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            int total = 1000;
            while (true)
            {
                try
                {
                    var othf = Process.GetProcessesByName("OnTheHouseFull").FirstOrDefault();

                    if (othf == null)
                    {
                        Process.Start(AppContext.BaseDirectory + "OnTheHouseFull.exe");
                    }

                    if (count > total && othf != null)
                    {
                        KillAll();
                        count = 0;
                    }

                    Thread.Sleep(2000);
                    count += 1;
                    Console.Clear();
                    Console.WriteLine($"{(total - count + 1) * 2} seconds before restart.");
                }
                catch(Exception ex)
                {
                    KillAll();
                }
            }
        }

        static void KillAll() {
            foreach (var othf in Process.GetProcessesByName("OnTheHouseFull"))
            {
                try
                {
                    othf.Kill();
                }
                catch (Exception ex) { }
            }
            foreach (var cd in Process.GetProcessesByName("chromedriver"))
            {
                try
                {
                    cd.Kill();
                }
                catch (Exception ex) { }
            }
            foreach (var cd in Process.GetProcessesByName("chrome"))
            {
                try
                {
                    cd.Kill();
                }
                catch (Exception ex) { }
            }
        }

    }
}
