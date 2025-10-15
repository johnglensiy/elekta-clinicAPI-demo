using elekta_apidemo.Migrations;
using Microsoft.EntityFrameworkCore;
using MyApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("Connection string: " + connectionString); 

builder.Services.AddDbContext<ClinicDb>(options =>
    options.UseNpgsql(connectionString)
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure Swagger middleware
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "ClinicAPI";
    config.Title = "ClinicAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "TodoAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

/////////////// TEST ENDPOINTS ///////////////

app.MapGet("/", () => "Hello World!");

app.MapGet("/database/status", (ClinicDb db) =>
{
    return db != null ? "DbContext is working!" : "DbContext is null";
});

app.MapGet("/database/connection", (ClinicDb db) =>
{
    var conn = db.Database.GetConnectionString();
    return Results.Ok(conn ?? "Connection string is null");
});

/////////////// /////////////// ///////////////

app.MapGet("/patients", async (ClinicDb db) =>
{
    try
    {
        var patients = await db.Patients.ToListAsync();
        return Results.Ok(patients);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapGet("/patients/{medRecordNo}", async (int medRecordNo, ClinicDb db) =>
{
    try
    {
        var patients = await db.Patients.FindAsync(medRecordNo);
        return Results.Ok(patients);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapPost("/patients/", async (ClinicDb db, Patient incomingPatient) =>
{
    db.Patients.Add(incomingPatient);
    await db.SaveChangesAsync();

    return Results.Created($"/patients/{incomingPatient.MedicalRecordNumber}", incomingPatient);
});

app.MapPatch("/patients/{medRecordNo}", async (int medRecordNo, ClinicDb db, Patient updatedFields) =>
{
    var patient = await db.Patients.FindAsync(medRecordNo);
    if (patient == null) return Results.NotFound("Patient not found.");

    if (!string.IsNullOrWhiteSpace(updatedFields.Name))
        patient.Name = updatedFields.Name;

    if (!string.IsNullOrWhiteSpace(updatedFields.AdmittingDiagnosis))
        patient.AdmittingDiagnosis = updatedFields.AdmittingDiagnosis;

    if (!string.IsNullOrWhiteSpace(updatedFields.Department))
        patient.Department = updatedFields.Department;

    if (updatedFields.Age != 0)
        patient.Age = updatedFields.Age;

    if (!string.IsNullOrWhiteSpace(updatedFields.Gender))
        patient.Gender = updatedFields.Gender;

    if (!string.IsNullOrWhiteSpace(updatedFields.Contacts))
        patient.Contacts = updatedFields.Contacts;

    await db.SaveChangesAsync();

    return Results.Ok(patient);
});

app.MapDelete("/patients/{medRecordNo}", async (int medRecordNo, ClinicDb db) =>
{
    var patientToBeDeleted = await db.Patients.FindAsync(medRecordNo);

    if (patientToBeDeleted == null) return Results.NotFound("Patient not found.");

    if (!string.IsNullOrWhiteSpace(patientToBeDeleted.AdmittingDiagnosis))
    {
        return Results.BadRequest("Cannot delete a patient with an admitting diagnosis.");
    }

    db.Patients.Remove(patientToBeDeleted);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
