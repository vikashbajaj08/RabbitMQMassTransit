using MassTransit;
using OrderService.Dtos;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

/*RabbitMQ Configuration*/
builder.Services.AddMassTransit((x) =>
{
    x.UsingRabbitMq((context, config) =>
    {
        //Setup host
        config.Host(builder.Configuration.GetValue<string>("RabbitMQHost"));

        //Setup dirct exchange
        config.Message<OrderPlaced>(x => x.SetEntityName("order-exchange"));
        config.Publish<OrderPlaced>(x =>{x.ExchangeType = "direct";});

        //Setup Fanout exchange
        //config.Message<OrderPlaced>(x => x.SetEntityName("order-exchange"));
        //config.Publish<OrderPlaced>(x => { x.ExchangeType = "fanout"; });
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

/*Endpoint*/

app.MapPost("/orders", async (OrderRequest order, IBus bus) =>
{
    var orderMessage = new OrderPlaced()
    {
        OrderId = order.OrderId,
        Quantity = order.Quantity
    };

    try
    {
        await bus.Publish(orderMessage, context =>{context.SetRoutingKey("order.created");});

        ////Fanout example
        //await bus.Publish(orderMessage);

        return Results.Created($"/orders/{order.OrderId}", orderMessage);
    }
    catch (Exception ex)
    {
        // Log the exception and handle it appropriately
        Console.WriteLine($"Error publishing message: {ex.Message}");
        return Results.StatusCode(500); // Return an appropriate HTTP status code
    }    
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
