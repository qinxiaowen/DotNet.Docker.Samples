using System;
using System.Collections.Generic;
using System.IO;
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

            //client.Images.CreateImageAsync(new Docker.DotNet.Models.ImagesCreateParameters()
            //{
            //    FromImage = "mysql",
            //    Tag = "5.5",
            //},
            //new AuthConfig()
            //{

            //},
            //new Progress()).Wait();


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


            string mContainerPort = "8080";
            string mHostPort = "8081";

            var createR = client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "microsoft/dotnet-samples:aspnetapp",
                Name = "aspnetcore_sample7",
                ExposedPorts = new Dictionary<string, object> {
        {
            mContainerPort, new {
                HostPort = mHostPort
            }
        }
    },
                HostConfig = new HostConfig()
                {
                    PublishAllPorts = true,
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        { "80/tcp",
                            new List<PortBinding> {
                                new PortBinding {
                                    HostIP="0.0.0.0",
                                    HostPort ="8000"
                                }
                            }
                        }
                    }
                }
            }).Result;


            //docker start
            //943b9ef8ac03
            //var r1 = client.Containers.StartContainerAsync("82e0fb93b86a", new ContainerStartParameters()
            //{
            //    DetachKeys = "123123123123123123"
            //}).Result;



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


            ////docker pull
            //var createContainerResponse = client.Containers.CreateContainerAsync(new CreateContainerParameters()
            //{
            //    Image = "hello-world",
            //    Name = "hello-world-01",

            //}).Result;

            //var processes = client.Containers.ListProcessesAsync();



            //docker image rm [OPTIONS] IMAGE [IMAGE...]

            //var deleteR = client.Images.DeleteImageAsync("hello-world", new ImageDeleteParameters()
            //{
            //    Force = true
            //}).Result;

            //client.






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
