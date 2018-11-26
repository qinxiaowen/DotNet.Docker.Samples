using System;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

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


            //docker pull

            client.Images.CreateImageAsync(new Docker.DotNet.Models.ImagesCreateParameters()
            {
                FromImage = "hello-world",
                Tag = "latest",
            },
new AuthConfig(),
new Progress()).Wait();


            //docker pull
            var createContainerResponse = client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "hello-world",
                Name = "hello-world-01",

            }).Result;

            client.Containers.



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
                Console.WriteLine(value.ToString());
            }
        }
    }
}
