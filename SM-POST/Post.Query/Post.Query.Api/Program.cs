using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Post.Query.Api.Queries;
using Post.Query.Api.QueryHandler;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;
using static Post.Query.Infrastructure.Config.AppSettings;

//Environment.SetEnvironmentVariable("KAFKA_TOPIC", "SocialMediaPostEvents");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Action<DbContextOptionsBuilder> configureDbContext = (o => o.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection(nameof(ConnectionStrings)));

builder.Services.AddDbContext<DatabaseContext>((serviceProvider, options) =>
{
    var databaseSettings = serviceProvider.GetRequiredService<IOptions<ConnectionStrings>>().Value;
    options.UseSqlServer(databaseSettings.SqlServer, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});

//builder.Services.AddDbContext<DatabaseContext>(configureDbContext);
builder.Services.AddSingleton(new DatabaseContextFactory(configureDbContext));


#pragma warning disable CS8604 // Possible null reference argument.s
#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var dataContext = builder.Services?.BuildServiceProvider().GetRequiredService<DatabaseContext>();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
dataContext.Database.EnsureCreated();

#pragma warning restore CS8602 // Dereference of a possibly null reference.
builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

var queryHandler = builder.Services.BuildServiceProvider().GetRequiredService<IQueryHandler>();
var dispatcher = new QueryDispatcher();

dispatcher.RegisterHandler<FindAllPostsQuery>(queryHandler.HandleAsync);
builder.Services.AddScoped<IQueryDispatcher<PostEntity>>(_ => dispatcher);

builder.Services.AddControllers();
builder.Services.AddHostedService<ConsumerHostedService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseAuthorization();

app.MapControllers();

app.Run();
