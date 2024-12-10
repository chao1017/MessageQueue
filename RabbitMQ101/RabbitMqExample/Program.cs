using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class Program
{
    static void Main(string[] args)
    {
        var factory = new ConnectionFactory()
        {
            HostName = "localhost",  // Change this if RabbitMQ is on a different machine
            Port = 5672,             // Default RabbitMQ port
            UserName = "myuser",      // Default RabbitMQ username
            Password = "myuserpwd",       // Default RabbitMQ password
            VirtualHost = "/"        // Default RabbitMQ virtual host
        };
        
        // Establish a connection to RabbitMQ
        using (var connection = factory.CreateConnection())
        {
            using (var channel = connection.CreateModel())
            {
                // Declare a queue
                string queueName = "MMS-Queue1";
                channel.QueueDeclare(queue: queueName,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                
                // Publish a message to the queue
                string message = "Hello RabbitMQ123456!";
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: queueName,
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine($"[x] Sent: {message}");
                
                
                // Create a consumer to receive messages from the queue
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var receivedMessage = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"[x] Received: {receivedMessage}");
                };
                
                // Start consuming messages from the queue
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);

                // Keep the application running to listen for messages
                Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
                
            }
        }
    }
}
