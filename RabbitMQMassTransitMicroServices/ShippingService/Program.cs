using MassTransit;
using ShippingService.Consumers;

var builder = WebApplication.CreateBuilder(args);

/*RabbitMQ Configuration*/
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(builder.Configuration.GetValue<string>("RabbitMQHost"));

        config.ReceiveEndpoint("shipping-order-queue", e =>
        {
            e.Consumer<OrderPlacedConsumer>();
            e.Bind("order-exchange", x =>
            {
                x.RoutingKey = "order.created";
                x.ExchangeType = "direct";
            });
            //Fanout example (Ignore Routing Key)
            //e.Bind("order-exchage", x => { x.ExchangeType = "fanout";});

            //Hearder Exchange example
            //e.Bind("order-exchange", x =>
            //{
            //    x.ExchangeType = "headers";
            //    x.SetBindingArgument("Product", "Laptop");
            //    x.SetBindingArgument("ProductType", "Electronics");
            //    x.SetBindingArgument("x-match", "all");

            //});

            //Topics Example

            //e.Consumer<OrderPlacedConsumer>();
            //e.Bind("order-exchange", x =>
            //{
            //    x.RoutingKey = "order.*"; //* match exactly one word and # match zero or more words
            //    x.ExchangeType = "topic";
            //});
        });
        
    });

    
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
