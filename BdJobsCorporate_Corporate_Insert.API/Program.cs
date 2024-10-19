using BdJobsCorporate_Corporate_Insert.Handler.Abstraction;
using BdJobsCorporate_Corporate_Insert.Handler.Service;
using BdJobsCorporate_Corporate_Insert.Repository.Data;
using BdJobsCorporate_Corporate_Insert.Repository.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddTransient<DapperDbContext>();

// Register repositories and services
builder.Services.AddScoped<ICompanyRepository, CompanyProfileRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();


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
