using Logitar.Master.EntityFrameworkCore.SqlServer;
using System.Text.Json.Serialization;

namespace Logitar.Master;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers() // TODO(fpion): Exception Handling, Logging?
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    // TODO(fpion): Cors
    // TODO(fpion): Authentication
    // TODO(fpion): Authorization
    // TODO(fpion): Session

    if (_enableOpenApi)
    {
      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen(); // TODO(fpion): OpenApiExtensions
    }

    // TODO(fpion): Monitoring
    // TODO(fpion): Health Checks

    services.AddLogitarMasterWithEntityFrameworkCoreSqlServer(_configuration);
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseSwagger();
      builder.UseSwaggerUI(); // TODO(fpion): OpenApiExtensions
    }

    builder.UseHttpsRedirection();
    // TODO(fpion): Cors
    // TODO(fpion): Logging
    //app.UseAuthentication(); // TODO(fpion): Authentication
    //app.UseAuthorization(); // TODO(fpion): Authorization
    // TODO(fpion): Session?
    // TODO(fpion): Session Renewal?

    if (builder is WebApplication application)
    {
      application.MapControllers();
      // TODO(fpion): Health Checks
    }
  }
}
