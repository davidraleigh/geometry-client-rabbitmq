using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

class RPCClient {
    private IConnection connection;
    private IModel channel;
    private string replyQueueName;
    private QueueingBasicConsumer consumer;

    public RPCClient() {
        var factory = new ConnectionFactory();
        factory.HostName = "geometry-mq.cloudapp.net";
        //@"host=geometry-mq.cloudapp.net;username=geometry-cs;password=testpassword";
        factory.UserName = "geometry-cs";
        factory.Password = "testpassword";

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        replyQueueName = channel.QueueDeclare().QueueName;
        consumer = new QueueingBasicConsumer(channel);
        channel.BasicConsume(queue: replyQueueName,
                             noAck: true,
                             consumer: consumer);
    }

	public string Call(string testGeomString) {
        var corrId = Guid.NewGuid().ToString();
        var props = channel.CreateBasicProperties();
        props.ReplyTo = replyQueueName;
        props.CorrelationId = corrId;

		var messageBytes = Encoding.UTF8.GetBytes(testGeomString);
        channel.BasicPublish(exchange: "",
                             routingKey: "rpc_queue",
                             basicProperties: props,
                             body: messageBytes);

		while (true) {
			var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
            if (ea.BasicProperties.CorrelationId == corrId) {
                return Encoding.UTF8.GetString(ea.Body);
            }
        }
    }

    public void Close() {
        connection.Close();
    }
}

class RPC {
    public static void Main() {
        var rpcClient = new RPCClient();
		//github README example: String testGeomString = "{\\n\\t\\"operator_name\": \"Union\",\n\t\"left_wkt_geometries\": [\n\t\t\"MULTIPOLYGON (((40 40, 20 45, 45 30, 40 40)), ((20 35, 45 20, 30 5, 10 10, 10 30, 20 35), (30 20, 20 25, 20 15, 30 20)))\",\n\t\t\"POLYGON ((30 10, 40 40, 20 40, 10 20, 30 10))\",\n\t\t\"MULTIPOLYGON (((30 20, 45 40, 10 40, 30 20)),((15 5, 40 10, 10 20, 5 10, 15 5)))\"\n\t]\n}"
		String testGeomString = "{operator_name:\"Buffer\", left_wkt_geometries:[\"POLYGON((1 1,5 1,5 5,1 5,1 1),(2 2, 3 2, 3 3, 2 3,2 2))\", \"POLYGON((1 1,5 1,5 5,1 5,1 1),(2 2, 3 2, 3 3, 2 3,2 2))\"], wkid_sr:4326, input_booleans:[false], input_doubles:[2.0]}";
		Console.WriteLine("Requesting {0}", testGeomString);

		var response = rpcClient.Call(testGeomString);
        Console.WriteLine(" [.] Got '{0}'", response);

        rpcClient.Close();
    }
}