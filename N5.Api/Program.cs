using Confluent.Kafka;
using Elastic.Clients.Elasticsearch;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using N5.Api;
using N5.Api.Configuration;
using N5.BuildingBlocks.Behaviors;
using N5.BuildingBlocks.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.SerilogConfiguration();
builder.Services.AddRegisterService();

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

// Configurar Elasticsearch
builder.Services.AddSingleton<ElasticsearchClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var url = configuration["ElasticsSettings:Url"];
    var defaultIndex = configuration["ElasticsSettings:DefaultIndex"];

    var settings = new ElasticsearchClientSettings(new Uri(url!))
        .DefaultIndex(defaultIndex!);

    return new ElasticsearchClient(settings);
});

// Configurar Kafka Producer
builder.Services.AddSingleton<IProducer<Null, string>>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var config = new ProducerConfig
    {
        BootstrapServers = configuration["Kafka:BootstrapServers"]
    };
    return new ProducerBuilder<Null, string>(config).Build();
});



builder.Services.AddControllers();

builder.Services.AddValidatorsFromAssemblyContaining<CreatePermissionCommandValidator>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AccessManagementContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
