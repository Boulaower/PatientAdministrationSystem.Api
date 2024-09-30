using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PatientAdministrationSystem.Application.Entities;
using PatientAdministrationSystem.Application.Interfaces;
using PatientAdministrationSystem.Application.Repositories.Interfaces;
using PatientAdministrationSystem.Application.Services;
using PatientAdministrationSystem.Infrastructure;
using PatientAdministrationSystem.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS to allow requests from your frontend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:5173") // Add any other allowed origins here
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // Optional: use only if you need to send cookies/auth
    });
});

// Register application services and repositories
builder.Services.AddScoped<IPatientsRepository, PatientsRepository>();
builder.Services.AddScoped<IPatientsService, PatientsService>();

// Configure the in-memory database context
builder.Services.AddDbContext<HciDataContext>(options =>
    options.UseInMemoryDatabase("InMemoryDatabase"));

// Enable response compression for better performance
builder.Services.AddResponseCompression(options => { options.EnableForHttps = true; });

// Register controllers and API explorer for endpoint discovery
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add support for accessing HTTP context within the application
builder.Services.AddHttpContextAccessor();

// Configure Swagger for API documentation
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HCI Home API",
        Description = "API for managing patient administration system"
    });

    // Group endpoints by controller for better organization in Swagger UI
    options.TagActionsBy(api =>
    {
        if (api.GroupName != null) return new[] { api.GroupName };

        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            return new[] { controllerActionDescriptor.ControllerName };

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });

    options.DocInclusionPredicate((_, _) => true);

    // Optional: Include XML comments if they exist for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Add health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Seed the in-memory database with sample data
using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<HciDataContext>();

    // Seed test data only if the database is empty
    if (!dbContext.Hospitals.Any() && !dbContext.Patients.Any())
    {
        var defaultHospitalId = Guid.NewGuid();
        var defaultVisitId = Guid.NewGuid();
        var patientId1 = Guid.NewGuid();
        var patientId2 = Guid.NewGuid();
        var patientId3 = Guid.NewGuid();
        var patientId4 = Guid.NewGuid();
        var patientId5 = Guid.NewGuid();

        dbContext.Hospitals.Add(new HospitalEntity
        {
            Id = defaultHospitalId,
            Name = "Default Hospital"
        });

        dbContext.Patients.Add(new PatientEntity
        {
            Id = patientId1,
            FirstName = "John",
            LastName = "Sweeney",
            Email = "john.sweeney@hci.care.com",
            PatientHospitals = new List<PatientHospitalRelation>
            {
                new()
                {
                    PatientId = patientId1,
                    HospitalId = defaultHospitalId,
                    VisitId = defaultVisitId
                }
            }
        });

        dbContext.Patients.Add(new PatientEntity
        {
            Id = patientId2,
            FirstName = "Vinny",
            LastName = "Lawlor",
            Email = "vinny.lawlor@hci.care"
        });

        dbContext.Patients.Add(new PatientEntity
        {
            Id = patientId3,
            FirstName = "Pauline",
            LastName = "O'Connor Molloy",
            Email = "pauline.oconnor@hci.care"
        });

        dbContext.Patients.Add(new PatientEntity
        {
            Id = patientId4,
            FirstName = "Mikey",
            LastName = "Molloy",
            Email = "mikey.molloy@hci.care"
        });

        dbContext.Patients.Add(new PatientEntity
        {
            Id = patientId5,
            FirstName = "Sean",
            LastName = "Molloy",
            Email = "sean.molloy@hci.care"
        });

        dbContext.Visits.Add(new VisitEntity
        {
            Id = defaultVisitId,
            Date = new DateTime(2023, 08, 22)
        });

        dbContext.SaveChanges();
    }
}

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger only in development environment
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HCI Home API v1");
        c.RoutePrefix = string.Empty; // Optional: set Swagger UI at app's root
    });
}

app.UseCors(); // Enable CORS for API
app.UseResponseCompression(); // Use response compression middleware

app.MapHealthChecks("/health"); // Map health check endpoint
app.MapControllers(); // Map controller endpoints

app.Run(); // Run the application
