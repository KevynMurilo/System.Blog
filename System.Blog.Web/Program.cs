using System.Blog.Infrastructure.Services;
using System.Blog.Web.Configurations;
using System.Blog.Web.Configurations.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRedisConfiguration(builder.Configuration);
builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddHostedService<UnconfirmedUserCleanupService>();
builder.Services.AddUserDependencies();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.AddStaticFilesConfiguration();

app.MapControllers();

app.Run();
