using Asp.Versioning;
using CollegeManagement.API.Data;
using CollegeManagement.API.Helpers;
using CollegeManagement.API.Interfaces;
using CollegeManagement.API.Middleware;
using CollegeManagement.API.Models;
using CollegeManagement.API.Profiles;
using CollegeManagement.API.Repositories;
using CollegeManagement.API.Repositories.Implementations;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services;
using CollegeManagement.API.Services.Implementations;
using CollegeManagement.API.Services.Interfaces;
using CollegeManagement.API.Validators.FacultyModuleValidators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Controllers

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .SetIsOriginAllowed(origin => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

#endregion

#region Database

// EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 30));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        serverVersion,
        mySqlOptions => mySqlOptions.CommandTimeout(120)));

// Dapper
builder.Services.AddSingleton<DatabaseContext>();

#endregion

#region AutoMapper & FluentValidation

builder.Services.AddAutoMapper(typeof(FacultyMappingProfile));

builder.Services.AddValidatorsFromAssemblyContaining<CreateFacultyDtoValidator>();

#endregion

#region Repositories

// Authentication
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();

// Academic Year
builder.Services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();

// Board (Dapper)
builder.Services.AddScoped<IBoardRepository, BoardRepository>();

// Faculty
builder.Services.AddScoped<IFacultyRepository, FacultyRepository>();
builder.Services.AddScoped<IFacultySubjectAllocationRepository, FacultySubjectAllocationRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

// Group, Section & Subject
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();

// Attendance (Dapper)
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();

// Student & Student Admissions
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentAdmissionRepository, StudentAdmissionRepository>();

// Assignments, Exams, Marks, Results, Promotions
builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
builder.Services.AddScoped<IExaminationRepository, ExaminationRepository>();
builder.Services.AddScoped<IMarksRepository, MarksRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();

// Timetable, Period, Room
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
builder.Services.AddScoped<IPeriodRepository, PeriodRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

// Study Materials
builder.Services.AddScoped<IStudyMaterialRepository, StudyMaterialRepository>();

#endregion

#region Services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();

builder.Services.AddScoped<IBoardService, BoardService>();

builder.Services.AddScoped<IFacultyService, FacultyService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

// Group, Section & Subject
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

builder.Services.AddScoped<IAttendanceService, AttendanceService>();

// Student & Student Admissions
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentAdmissionService, StudentAdmissionService>();

// Assignments, Exams, Marks, Results, Promotions, Fee
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();
builder.Services.AddScoped<IMarksService, MarksService>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IFeeService, FeeService>();

// Timetable, Period, Room
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<IRoomService, RoomService>();

// Study Materials
builder.Services.AddScoped<IStudyMaterialService, StudyMaterialService>();

#endregion

#region Email

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

#endregion

#region JWT Authentication

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var key = Encoding.UTF8.GetBytes(
    jwtSettings["Key"] ??
    "a_very_long_secure_secret_key_of_at_least_32_characters_long");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;

        options.SaveToken = true;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(key),

                ClockSkew = TimeSpan.Zero
            };
    });

#endregion

#region API Versioning

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);

        options.AssumeDefaultVersionWhenUnspecified = true;

        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";

        options.SubstituteApiVersionInUrl = true;
    });

#endregion

#region Swagger

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Intermediate College Management System API",
        Version = "v1",
        Description = "College Management System REST APIs",
        Contact = new OpenApiContact
        {
            Name = "System Engineering Team",
            Email = "support@example.com"
        }
    });

    var xmlFile =
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    var xmlPath =
        Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme.",

            Name = "Authorization",

            In = ParameterLocation.Header,

            Type = SecuritySchemeType.Http,

            Scheme = "bearer",

            BearerFormat = "JWT"
        });

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

#endregion

var app = builder.Build();

#region Forwarded Headers

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto
});

#endregion

#region Global Exception Middleware

app.UseMiddleware<GlobalExceptionMiddleware>();

#endregion

#region Database Migration

// NOTE: Automatic EF Core migration on startup is disabled so it doesn't execute automatically every time.
// To run migrations manually, use: dotnet ef database update
// using (var scope = app.Services.CreateScope())
// {
//     var services = scope.ServiceProvider;
//     try
//     {
//         var context = services.GetRequiredService<AppDbContext>();
//         // context.Database.Migrate();
//     }
//     catch (Exception ex)
//     {
//         var logger = services.GetRequiredService<ILogger<Program>>();
//         logger.LogError(ex, "An error occurred while migrating the database.");
//     }
// }

#endregion

#region Swagger UI

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "College Management API v1");

        c.RoutePrefix = "swagger";
    });
}

#endregion

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("AllowFrontend");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();