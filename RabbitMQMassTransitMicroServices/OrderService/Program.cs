using MassTransit;
using OrderService.Dtos;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

/*RabbitMQ Configuration*/
builder.Services.AddMassTransit((x) =>
{
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetValue<string>("RabbitMQHost"));
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

    await bus.Publish(orderMessage);

    return Results.Created($"/orders/{order.OrderId}", orderMessage);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
