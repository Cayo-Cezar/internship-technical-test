using Hangfire;
using Hangfire.MySql;
using Microsoft.EntityFrameworkCore;
using APICatalogo.WebAPI;
using APICatalogo.Infrastructure.Data;
using APICatalogo.Application.Services;
using FileProcessingApi.Core.Interfaces;
using FileProcessingApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Adicionar servi�os ao cont�iner
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection)));

// Configura��o do Hangfire
builder.Services.AddHangfire(configuration =>
    configuration.UseStorage(new MySqlStorage(mySqlConnection, new MySqlStorageOptions())));
builder.Services.AddHangfireServer();

// Registrar o servi�o de processamento de arquivos
builder.Services.AddTransient<FileProcessingService>();

// Registrar o servi�o de upload de arquivos
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<IArquivoRepository, ArquivoRepository>();

var app = builder.Build();

// Configurar o pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Hangfire Dashboard (opcional)
app.UseHangfireDashboard();

// Configurar o job recorrente
RecurringJob.AddOrUpdate<FileProcessingService>(
    service => service.ProcessFiles(),
    Cron.Minutely);

app.Run();
