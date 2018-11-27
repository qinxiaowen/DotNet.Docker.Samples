using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;

namespace Sample1
{
    class Program
    {
        //https://docs.docker.com/v17.09/engine/api/v1.24/#31-containers
        /// <summary>
        /// image-->container->run
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            DockerClient client = new DockerClientConfiguration(new Uri("tcp://localhost:2375"))
     .CreateClient();


            //bash command
            var echo = Encoding.UTF8.GetBytes("ls -al\n");
            //docker exec -it aspnetcore_sample15 bash
            Task<ContainerExecCreateResponse> execCreateRsp = client.Containers.ExecCreateContainerAsync("0eeb1f915c66", new ContainerExecCreateParameters()
            {
                AttachStderr = true,
                AttachStdin = true,
                AttachStdout = true,
                Cmd = new List<string>() {"bash" },
                Detach = false,
                Tty = false,
                User = "root",
                Privileged = true
            });

            execCreateRsp.Wait();

            string execId = execCreateRsp.Result.ID;
            Task<MultiplexedStream> execStartRsp = client.Containers.StartAndAttachContainerExecAsync(execId, false, default(CancellationToken));
            execStartRsp.Wait();
            byte[] result = new byte[1024];
            execStartRsp.Result.WriteAsync(echo, 0, echo.Length, CancellationToken.None).Wait();
            MultiplexedStream.ReadResult readResult = execStartRsp.Result.ReadOutputAsync(result, 0, 1024, default(CancellationToken)).Result;
            Console.Write(Encoding.UTF8.GetString(result, 0, readResult.Count));

            //response:

            //-rw-r--r-- 1 root root    137 Nov 14 20:02 appsettings.Development.json
            //-rw-r--r-- 1 root root     97 Nov 14 20:02 appsettings.json
            //-rw-r--r-- 1 root root  77824 Nov 14 20:04 aspnetapp.Views.dll
            //-rw-r--r-- 1 root root   5472 Nov 14 20:04 aspnetapp.Views.pdb
            //-rw-r--r-- 1 root root 223805 Nov 14 20:04 aspnetapp.deps.json
            //-rw-r--r-- 1 root root   9728 Nov 14 20:04 aspnetapp.dll
            //-rw-r--r-- 1 root root   1624 Nov 14 20:04 aspnetapp.pdb
            //-rw-r--r-- 1 root root    213 Nov 14 20:04 aspnetapp.runtimeconfig.json
            //-rw-r--r-- 1 root root    458 Nov 14 20:04 web.config
            //drwxr-xr-x 6 root root   4096 Nov 14 20:04 wwwroot

            Console.ReadKey();


            //docker stop container
            client.Containers.StopContainerAsync("943b9ef8ac03", new ContainerStopParameters()
            {

            });

            //docker images
            var images = client.Images.ListImagesAsync(new ImagesListParameters()
            {

            }).Result;
            foreach (var i in images)
            {
                Console.WriteLine($"image:{string.Join(',', i.RepoTags)}");
            }


            //docker containers
            // docker ps -a
            var containerList = client.Containers.ListContainersAsync(new Docker.DotNet.Models.ContainersListParameters()
            {
                Limit = 10,
                All = true
            }).Result;

            foreach (var c in containerList)
            {
                Console.WriteLine($"container:{c.Image} ,Names:{string.Join(',', c.Names)}");
            }



            ////docker pull

            client.Images.CreateImageAsync(new Docker.DotNet.Models.ImagesCreateParameters()
            {
                FromImage = "mysql",
                Tag = "5.5",
            },
            new AuthConfig()
            {

            },
            new Progress()).Wait();


            //docker create container 
            // docker create
            //var createContainerResponse = client.Containers.CreateContainerAsync(new CreateContainerParameters()
            //{
            //    Image = "nginx:latest",
            //    Name = "nginx-01",
            //     //Cmd=new string[] {"" }
            //}).Result;
            client.Containers.StartContainerAsync("943b9ef8ac03", new ContainerStartParameters());


            //Config createContainerParameters = new Config
            //{
            //    Image = "mysql:5.5",
            //    ArgsEscaped = false,
            //    AttachStderr = false,
            //    AttachStdin = true,
            //    AttachStdout = true,
            //    Cmd = new string[] {
            //         "--name","some-mysql2",
            //            "-e","MYSQL_ROOT_PASSWORD=my-secret-pw",
            //            "-d","mysql:5.5"
            //    },

            //    OpenStdin = true,
            //    StdinOnce = true,
            //};

            //var createContainerResponse = client.Containers.CreateContainerAsync(new CreateContainerParameters(createContainerParameters)).Result;


            //docker run
            var createR = client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "microsoft/dotnet-samples:aspnetapp",
                Name = "aspnetcore_sample16",
                ExposedPorts = new Dictionary<string, EmptyStruct>() {
                    { "80", new EmptyStruct() }
                },
                HostConfig = new HostConfig()
                {
                    //PublishAllPorts = true,
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        //contariner port
                        { "80",
                            new List<PortBinding> {
                                new PortBinding {
                                    HostPort ="8000"
                                }
                            }
                        }
                    }
                }
            }).Result;


            //docker start
            //943b9ef8ac03
            var r1 = client.Containers.StartContainerAsync("82e0fb93b86a", new ContainerStartParameters()
            {
                //DetachKeys = ""
            }).Result;



            //var createContainerResponse = client.Containers.StartWithConfigContainerExecAsync("0c460e08b817", new ContainerExecStartParameters()
            //{
            //    AttachStderr = true,
            //    AttachStdin = true,
            //    AttachStdout = true,
            //    Cmd = new string[] { "env", "TERM=xterm-256color", "bash" },
            //    Detach = false,
            //    Tty = false,
            //    User = "root",
            //    Privileged = true,


            //    Cmd = new string[] {
            //          "--name","some-mysql2",
            //          "-e","MYSQL_ROOT_PASSWORD=my-secret-pw",
            //          "-d","mysql:5.5"
            //      }
            //}).Result;


            //docker create  image
            var createContainerResponse = client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "hello-world",
                Name = "hello-world-01",
            }).Result;




            //docker image rm[OPTIONS] IMAGE[IMAGE...]

            var deleteR = client.Images.DeleteImageAsync("hello-world", new ImageDeleteParameters()
            {
                Force = true
            }).Result;

            Console.ReadKey();
        }
        public class Progress : IProgress<JSONMessage>
        {
            public void Report(JSONMessage value)
            {
                Console.WriteLine(JsonConvert.SerializeObject(value));
            }
        }
    }
}
