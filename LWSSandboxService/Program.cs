using Confluent.Kafka;
using k8s;
using LWSSandboxService.Filter;
using LWSSandboxService.Repository;
using LWSSandboxService.Service;

var builder = WebApplication.CreateBuilder(args);

// Add Configuration
var kafkaSection = builder.Configuration.GetSection("KafkaProducerConfig").Get<ProducerConfig>();
builder.Services.AddSingleton(kafkaSection);

// Add services to the container.
builder.Services.AddControllers(option => option.Filters.Add<CustomExceptionFilter>());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Scoped
builder.Services.AddScoped<UbuntuContainerService>();

// Add Singleton
builder.Services.AddSingleton<KubernetesRepository>();
builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
builder.Services.AddSingleton<IEventRepository, EventRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseStaticFiles(new StaticFileOptions
    {
        ServeUnknownFileTypes = true
    });
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger.yml", "SandboxAPI v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();