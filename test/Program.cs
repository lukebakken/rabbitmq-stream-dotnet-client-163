using System.Net;
using System.Text;
using RabbitMQ.Stream.Client;
using RabbitMQ.Stream.Client.Reliable;

var config = new StreamSystemConfig()
{
    UserName = "test",
    Password = "test",
    Endpoints = new EndPoint[]
    {
        new DnsEndPoint("node0", 5552),
        new DnsEndPoint("node1", 5552),
        new DnsEndPoint("node2", 5552),
    }
};

var system = await StreamSystem.Create(config);
var streamName = Guid.NewGuid().ToString();
await system.CreateStream(new StreamSpec(streamName));

var producer = await ReliableProducer.CreateReliableProducer(new ReliableProducerConfig()
{
    Stream = streamName,
    StreamSystem = system,
    ConfirmationHandler = confirmation =>
    {
        Console.WriteLine($"Confirmation: {confirmation.Status} {confirmation.PublishingId}");
        return Task.CompletedTask;
    },
});

var t= Task.Run(async () =>
{
    for (int i = 0; i < 1000; i++)
    {
        if (!producer.IsOpen())
        {
            Console.WriteLine("Producer is not connected");
            return;
        }
        await producer.Send(new Message(Encoding.UTF8.GetBytes($"hello {i}")));
        Console.WriteLine($"Sent message {i}");
        Thread.Sleep(1 * 1000);
    }
});

var consumer = await ReliableConsumer.CreateReliableConsumer(new ReliableConsumerConfig()
{
    Stream = streamName,
    StreamSystem = system,
    MessageHandler = (_, _, message) =>
    {
        Console.WriteLine($"Consumed message {Encoding.UTF8.GetString(message.Data.Contents)}");
        return Task.CompletedTask;
    },
});

Console.WriteLine("Press enter to stop");
Console.ReadKey();
t.Dispose();
await producer.Close();
await consumer.Close();
await system.DeleteStream(streamName);
await system.Close();
Console.WriteLine("Closed");
