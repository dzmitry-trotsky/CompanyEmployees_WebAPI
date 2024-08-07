using CompanyEmployees.Extensions;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Utility;
using Contracts;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;
using NLog;
using Service.DataShaping;
using Shared.DTO;

NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter() =>
    new ServiceCollection().AddLogging()
                           .AddMvc()
                           .AddNewtonsoftJson().Services
                           .BuildServiceProvider()
                           .GetRequiredService<IOptions<MvcOptions>>().Value.InputFormatters
                           .OfType<NewtonsoftJsonPatchInputFormatter>()
                           .First();


var builder = WebApplication.CreateBuilder(args);

LogManager.Setup().LoadConfigurationFromFile(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.ConfigureCors();

builder.Services.ConfigureIISIntegration();

builder.Services.ConfigureLoggerService();

builder.Services.ConfigureRepositoryManager();

builder.Services.ConfigureServiceManager();

builder.Services.ConfigureSqlContext(builder.Configuration);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    //suppressing a default model state validation for enabling our custom responses from the actions 
    options.SuppressModelStateInvalidFilter = true; 
});

builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.AddScoped<ValidateMediaTypeAttribute>();

builder.Services.AddScoped<IEmployeeLinks, EmployeeLinks>();

builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();

builder.Services.AddControllers(config => 
    { 
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
        config.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
    }).AddXmlDataContractSerializerFormatters()
      .AddCustomCsvFormatter()
      .AddApplicationPart(typeof(CompanyEmployees.Presentation.AssemblyReference).Assembly);

builder.Services.AddCustomMediaTypes();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();

app.ConfigureExceptionHandler(logger);

if(app.Environment.IsProduction())
    app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
