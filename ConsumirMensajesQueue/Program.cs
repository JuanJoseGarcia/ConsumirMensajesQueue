using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace ConsumirMensajesQueue
{
    class Program
    {

        public class mensaje
        {
            public string Asunto { get; set; }
            public string Contenido { get; set; }
        }

        static void Main(string[] args)
        {

            CloudStorageAccount conexion =
                CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["ConexionAzure"].ConnectionString);
            CloudQueueClient queueClient = conexion.CreateCloudQueueClient();

            CloudQueue queue = queueClient.GetQueueReference("messages");
            queue.CreateIfNotExists();

            Console.WriteLine("Consumir los mensajes de la cola...");

            while (true)
            {
                CloudQueueMessage retr = queue.GetMessage();
                if (retr != null)
                {
                    mensaje aux = new mensaje();
                    aux = JsonConvert.DeserializeObject<mensaje>(retr.AsString);
                    Console.WriteLine("Asunto: {0} - Contenido: {1} - Hora: {2}", aux.Asunto, aux.Contenido,
                        DateTime.Parse(retr.InsertionTime.ToString()).ToString("dd-MM-yyy HH:mm:ss"));
                    queue.DeleteMessage(retr);
                }
            }
        }
    }
}
