using ImageCompressApi.Endpoints;
using ImageCompressApi.Model;
using ImageCompressApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Image Compress API", Version = "v1" });
});

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<ImageKitSettings>(builder.Configuration.GetSection("ImageKitSettings"));
builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("SmsSettings"));

builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddSingleton<PdfService>();
builder.Services.AddSingleton<ImageCompressionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapImageCompressEndpoints();
app.MapCloudinaryEndpoints();
app.MapImageKitEndpoints();
app.MapPdfEndpoints();
app.MapSmsEndpoints();

app.Run();
