using eCommerce.BusinessLogicLayer;
using eCommerce.DataAcessLayer;
using FluentValidation.AspNetCore;
using productMicroservice.API;
using productMicroservice.API.Middleware;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddBusinessLogicLayer();
//builder.Services.addda();
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.ConfigureHttpJsonOptions(p=>p.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();
// 1.add endpoint api exploerer
builder.Services.AddEndpointsApiExplorer();
//2.swaggergen
builder.Services.AddSwaggerGen(x => x.Equals("productMicroservice API"));
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => {
        builder.WithOrigins("http://localhost:4200").
        AllowAnyMethod().
        AllowAnyHeader();
  });
   });

var app = builder.Build();

app.MapProductionApiEndpoints();  // ← Add this line

app.UseSwagger();
app.UseSwaggerUI();
app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseAuthentication();
app.UseAuthorization();
//app.MapGet("/", () => "Hello World!");
app.MapControllers();
app.Run();
