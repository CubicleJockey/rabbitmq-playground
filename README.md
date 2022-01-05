# rabbitmq-playground
 A place to learn about RabbitMQ


Run Docker Command: 
```
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3.9-management
```

#### RabbitMQ Official Tutorials
 * [Basics](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)
   * Projects
     * RabbitMQ.BasicSend
     * RabbitMQ.BasicReceive
 * [Work Queues](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html)
    * Projects 
      * RabbitMQ.NewTask
      * RabbitMQ.Worker
 * [Publish/Subscribe](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html)
   * Projects
     * RabbitMQ.EmitLogs
     * RabbitMQ.ReceiveLogs
 * [Routing](https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html)
   * Projects
     * RabbitMQ.EmitsLogsDirect
     * RabbitMQ.ReceiveLogsDirect
