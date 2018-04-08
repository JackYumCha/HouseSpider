using System;
using System.Diagnostics;
using System.Collections.Generic;   
using System.IO;
using System.Text;
using System.Threading;
using Renci.SshNet;
using Newtonsoft.Json;

namespace SSHMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SSH Monitor.");

            // read the settings by newtonsoft.json

            List<string> additionalCommands = new List<string>()
            {
                //"sudo crontab -r",
                //"service spider start"
            };

            DirectoryInfo baseDir = new DirectoryInfo(AppContext.BaseDirectory);

            var sshFiles = baseDir.GetFiles("*.ssh.json");
            if(sshFiles.Length ==0)
            {
                // create an example setting file
                var example = new List<UbuntuOperation>()
                {
                    new UbuntuOperation()
                    {
                        Name = "example connection",
                        Host = "host.com",
                        Username = "username",
                        Password = "password",
                        Commands = new List<BashCommand>()
                        {
                            new BashCommand()
                            {
                                Command = "ls -1 | wc -l",
                                Title = "Count Files in Root:"
                            }
                        }
                    }
                };

                string exampleFilename = "example.ssh.json";

                File.WriteAllText($"{AppContext.BaseDirectory}/{exampleFilename}", JsonConvert.SerializeObject(example, Formatting.Indented));

                Console.WriteLine($@"An Example Setting is generate to {exampleFilename}");
            }
            else
            {
                foreach (var setting in sshFiles)
                {
                    var json = File.ReadAllText(setting.FullName);
                    var list = JsonConvert.DeserializeObject<List<UbuntuOperation>>(json);

                    // access each of linux server by ssh connection and execute the bash
                    foreach (var operation in list)
                    {
                        ConnectionInfo connectionInfo = new ConnectionInfo(operation.Host, operation.Username, new AuthenticationMethod[] {
                            new PasswordAuthenticationMethod(operation.Username, operation.Password)
                        });

                        SshClient sshClient = new SshClient(connectionInfo);

                        Console.WriteLine($"Connecting to {operation.Name} ({operation.Host}):");

                        sshClient.Connect();

                        if (operation.Commands != null)
                        {
                            foreach (var cmd in operation.Commands)
                            {
                                var result = sshClient.CreateCommand(cmd.Command).Execute();
                                if (cmd.Results == null)
                                    cmd.Results = new List<BashResult>();
                                cmd.Results.Add(new BashResult()
                                {
                                    Time = DateTime.Now,
                                    Result = result
                                });
                                // print the last 3 results:
                                Console.WriteLine($"{cmd.Title}:");
                                for (var i = cmd.Results.Count -1; i >= 0 && i > cmd.Results.Count - 4; i--)
                                {
                                    Console.WriteLine($"  {cmd.Results[i].Time} : {cmd.Results[i].Result}");
                                }
                            }
                        }
                        
                        foreach(var cmd in additionalCommands)
                        {
                            var result = sshClient.CreateCommand(cmd).Execute();
                        }
                    }

                    File.WriteAllText(setting.FullName, JsonConvert.SerializeObject(list, Formatting.Indented));
                }
            }



            Console.WriteLine("Press Any Key to Exit...");
            Console.ReadKey();
        }
    }
}
