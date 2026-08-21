using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Repositories.Implementations;
using Asp.Versioning;
using CollegeManagement.API.Data;
using CollegeManagement.API.Helpers;
using CollegeManagement.API.Interfaces;
using CollegeManagement.API.Middleware;
using CollegeManagement.API.Models;
using CollegeManagement.API.Profiles;
using CollegeManagement.API.Repositories;

using CollegeManagement.API.Services;
using CollegeManagement.API.Services.Implementations;
using CollegeManagement.API.Services.Interfaces;
using CollegeManagement.API.Services.Location;
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

// Register QuestPDF Community License
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

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
        mySqlOptions => mySqlOptions
            .CommandTimeout(120)
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(3),
                errorNumbersToAdd: null)));

// Dapper
builder.Services.AddSingleton<DatabaseContext>();

// Caching
builder.Services.AddMemoryCache();

#endregion

#region AutoMapper & FluentValidation

builder.Services.AddAutoMapper(
    typeof(FacultyMappingProfile),
    typeof(MarksMappingProfile),
    typeof(AttendanceProfile),
    typeof(CollegeManagement.API.Profiles.TimetableMappingProfile),
    typeof(SectionMappingProfile));

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
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Faculty
builder.Services.AddScoped<IDesignationRepository, DesignationRepository>();
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
builder.Services.AddScoped<IAssignmentSubmissionRepository, AssignmentSubmissionRepository>();
builder.Services.AddScoped<IExaminationRepository, ExaminationRepository>();
builder.Services.AddScoped<IMarksRepository, MarksRepository>();
builder.Services.AddScoped<IResultRepository, ResultRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();

// Timetable, Period, Room
builder.Services.AddScoped<ITimetableRepository, TimetableRepository>();
builder.Services.AddScoped<ITimetableBackupRepository, TimetableBackupRepository>();
builder.Services.AddScoped<IPeriodRepository, PeriodRepository>();
builder.Services.AddScoped<IPeriodStructureRepository, PeriodStructureRepository>();
builder.Services.AddScoped<IBreakTypeRepository, BreakTypeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

// Reports, Study Materials & Certificates
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IStudyMaterialRepository, StudyMaterialRepository>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();

#endregion

#region Services

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();

builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<ILookupCacheService, LookupCacheService>();
builder.Services.AddScoped<IBoardExportService, BoardExportService>();

builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IFacultyService, FacultyService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddScoped<IFeeRepository, FeeRepository>();
builder.Services.AddScoped<IFeeService, FeeService>();

// Group, Section & Subject
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();

builder.Services.AddSingleton<IAttendanceCacheService, AttendanceCacheService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

// Student & Student Admissions
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentAdmissionService, StudentAdmissionService>();

// Assignments, Exams, Marks, Results, Promotions, Fee
builder.Services.AddScoped<IAssignmentService, AssignmentService>();
builder.Services.AddScoped<IAssignmentSubmissionService, AssignmentSubmissionService>();
builder.Services.AddScoped<IExaminationService, ExaminationService>();
builder.Services.AddScoped<IMarksService, MarksService>();
builder.Services.AddScoped<IEvaluationService, EvaluationService>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();

// Timetable, Period, Room
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<IPeriodStructureService, PeriodStructureService>();
builder.Services.AddScoped<IBreakTypeService, BreakTypeService>();
builder.Services.AddScoped<IRoomService, RoomService>();

// Reports, Study Materials & Certificates
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IStudyMaterialService, StudyMaterialService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();

// Location Service
builder.Services.AddHttpClient<ILocationService, LocationService>(client =>
{
    client.BaseAddress = new Uri("https://api.postalpincode.in/");
});

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
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

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

var tempConnStr = builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrEmpty(tempConnStr))
{
    try
    {
        using var conn = new MySqlConnector.MySqlConnection(tempConnStr);
        conn.Open();

        var colsToAdd = new string[]
        {
            "Board VARCHAR(100) NULL",
            "BoardId INT NULL",
            "AcademicYearId INT NOT NULL DEFAULT 1",
            "AcademicLevel VARCHAR(100) NULL",
            "AcademicLevelId INT NULL",
            "GroupId INT NOT NULL DEFAULT 1",
            "SectionId INT NOT NULL DEFAULT 1",
            "ExaminationId INT NOT NULL DEFAULT 0",
            "SubjectId INT NOT NULL DEFAULT 1",
            "StudentId INT NOT NULL DEFAULT 1",
            "RollNo VARCHAR(100) NULL",
            "StudentName VARCHAR(200) NULL",
            "FacultyId INT NULL",
            "InternalMarks INT NOT NULL DEFAULT 0",
            "PracticalMarks INT NOT NULL DEFAULT 0",
            "TheoryMarks INT NOT NULL DEFAULT 0",
            "TotalMarks INT NOT NULL DEFAULT 0",
            "PassingMarks INT NOT NULL DEFAULT 35",
            "IsAbsent TINYINT(1) NOT NULL DEFAULT 0",
            "Remarks VARCHAR(500) NULL",
            "IsVerified TINYINT(1) NOT NULL DEFAULT 0",
            "IsPublished TINYINT(1) NOT NULL DEFAULT 0",
            "Status INT NOT NULL DEFAULT 1",
            "IsLocked TINYINT(1) NOT NULL DEFAULT 0",
            "IsActive TINYINT(1) NOT NULL DEFAULT 1",
            "CreatedAt DATETIME(6) NOT NULL DEFAULT NOW(6)",
            "UpdatedAt DATETIME(6) NULL",
            "VerifiedBy VARCHAR(100) NULL",
            "VerifiedAt DATETIME(6) NULL",
            "ApprovedBy INT NULL",
            "ApprovedAt DATETIME(6) NULL",
            "PublishedAt DATETIME(6) NULL"
        };

        foreach (var col in colsToAdd)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `Marks` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `marks` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        var facultyCols = new string[]
        {
            "Username VARCHAR(100) NULL",
            "Password VARCHAR(255) NULL",
            "Aadhaar VARCHAR(20) NULL",
            "Mobile VARCHAR(20) NULL",
            "Qualification VARCHAR(100) NULL",
            "Designation VARCHAR(100) NULL",
            "Gender VARCHAR(20) NULL"
        };

        foreach (var col in facultyCols)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `Faculties` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `faculties` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        var groupCols = new string[]
        {
            "AcademicLevel VARCHAR(50) NOT NULL DEFAULT ''",
            "Board VARCHAR(100) NOT NULL DEFAULT ''",
            "Description VARCHAR(500) NULL",
            "GroupCode VARCHAR(30) NOT NULL DEFAULT ''"
        };

        foreach (var col in groupCols)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `Groups` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `groups` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        var sectionCols = new string[]
        {
            "AcademicLevel VARCHAR(50) NOT NULL DEFAULT ''",
            "Board VARCHAR(100) NOT NULL DEFAULT ''",
            "Group VARCHAR(100) NOT NULL DEFAULT ''",
            "Programme VARCHAR(100) NOT NULL DEFAULT ''",
            "RoomNumber VARCHAR(50) NULL",
            "ClassTeacherId INT NULL",
            "InchargeId INT NULL",
            "MaximumStrength INT NOT NULL DEFAULT 0",
            "BoardId INT NULL",
            "GroupId INT NULL",
            "RoomId INT NULL"
        };

        foreach (var col in sectionCols)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `Sections` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `sections` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `Sections` SET `InchargeId` = `ClassTeacherId` WHERE (`InchargeId` IS NULL OR `InchargeId` = 0) AND `ClassTeacherId` IS NOT NULL AND `ClassTeacherId` > 0;";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `Sections` SET `ClassTeacherId` = `InchargeId` WHERE (`ClassTeacherId` IS NULL OR `ClassTeacherId` = 0) AND `InchargeId` IS NOT NULL AND `InchargeId` > 0;";
            cmd.ExecuteNonQuery();
        }
        catch { }

        var roomCols = new string[]
        {
            "RoomCode VARCHAR(50) NULL",
            "RoomName VARCHAR(100) NULL",
            "BlockName VARCHAR(100) NULL",
            "BuildingName VARCHAR(100) NULL"
        };

        foreach (var col in roomCols)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `Rooms` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `rooms` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `Rooms` SET `BlockName` = `BuildingName` WHERE (`BlockName` IS NULL OR `BlockName` = '') AND `BuildingName` IS NOT NULL AND `BuildingName` <> '';";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `Rooms` SET `BuildingName` = `BlockName` WHERE (`BuildingName` IS NULL OR `BuildingName` = '') AND `BlockName` IS NOT NULL AND `BlockName` <> '';";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `Rooms` SET `RoomCode` = `RoomNumber` WHERE (`RoomCode` IS NULL OR `RoomCode` = '') AND `RoomNumber` IS NOT NULL;";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `Rooms` SET `RoomName` = `RoomNumber` WHERE (`RoomName` IS NULL OR `RoomName` = '') AND `RoomNumber` IS NOT NULL;";
            cmd.ExecuteNonQuery();
        }
        catch { }

        var subjectCols = new string[]
        {
            "AcademicLevel VARCHAR(100) NOT NULL DEFAULT ''",
            "Board VARCHAR(100) NOT NULL DEFAULT ''",
            "Group VARCHAR(100) NOT NULL DEFAULT ''",
            "Theory TINYINT(1) NOT NULL DEFAULT 0",
            "Practical TINYINT(1) NOT NULL DEFAULT 0",
            "Language TINYINT(1) NOT NULL DEFAULT 0",
            "Elective TINYINT(1) NOT NULL DEFAULT 0",
            "InternalMarks INT NOT NULL DEFAULT 0",
            "PracticalMarks INT NOT NULL DEFAULT 0",
            "ExternalMarks INT NOT NULL DEFAULT 0",
            "TotalMarks INT NOT NULL DEFAULT 0",
            "PassingMarks INT NOT NULL DEFAULT 0",
            "BoardId INT NULL",
            "AcademicYearId INT NULL",
            "GroupId INT NULL"
        };

        foreach (var col in subjectCols)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `Subjects` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `subjects` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        var studentCols = new string[]
        {
            "Board VARCHAR(100) NULL",
            "AcademicLevel VARCHAR(100) NULL",
            "Section VARCHAR(100) NULL",
            "AddressLine1 VARCHAR(255) NULL",
            "AddressLine2 VARCHAR(255) NULL",
            "MotherName VARCHAR(100) NULL",
            "MotherOccupation VARCHAR(100) NULL",
            "FatherOccupation VARCHAR(100) NULL",
            "AdmissionType VARCHAR(50) NULL",
            "AdmissionQuota VARCHAR(50) NULL",
            "SecondLanguage VARCHAR(50) NULL",
            "Medium VARCHAR(50) NULL",
            "District VARCHAR(100) NULL",
            "City VARCHAR(100) NULL",
            "State VARCHAR(100) NULL",
            "Pincode VARCHAR(20) NULL",
            "TransferCertificate VARCHAR(255) NULL",
            "IncomeCertificate VARCHAR(255) NULL",
            "CasteCertificate VARCHAR(255) NULL",
            "MarksMemo VARCHAR(255) NULL",
            "AadhaarDocument VARCHAR(255) NULL",
            "ScholarshipStatus VARCHAR(50) NULL"
        };

        foreach (var col in studentCols)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `Students` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"ALTER TABLE `students` ADD COLUMN {col}";
                cmd.ExecuteNonQuery();
            }
            catch { }

            var colName = col.Split(' ')[0];

            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = $"UPDATE `Students` SET `{colName}` = '' WHERE `{colName}` IS NULL";
                cmd.ExecuteNonQuery();
            }
            catch { }
        }

        try
        {
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                INSERT IGNORE INTO `Students`
                (`StudentId`, `AdmissionNo`, `RollNo`, `StudentName`,
                 `FirstName`, `LastName`, `Gender`, `DateOfBirth`,
                 `Mobile`, `Email`, `AcademicYearId`, `GroupId`,
                 `SectionId`, `IsActive`, `CreatedAt`)
                VALUES
                (1, 'ADM001', 'MPCA001', 'Rahul', 'Rahul', 'Kumar',
                 'Male', '2006-01-01', '9876543210', 'rahul@example.com',
                 1, 1, 1, 1, NOW()),
                (2, 'ADM002', 'MPCA002', 'Ramesh', 'Ramesh', 'Kumar',
                 'Male', '2006-01-01', '9876543211', 'ramesh@example.com',
                 1, 1, 1, 1, NOW()),
                (3, 'ADM003', 'MPCA003', 'Sai Kiran', 'Sai', 'Kiran',
                 'Male', '2006-01-01', '9876543212', 'saikiran@example.com',
                 1, 1, 1, 1, NOW()),
                (4, 'ADM004', 'MPCA004', 'Ananya Reddy', 'Ananya', 'Reddy',
                 'Female', '2006-01-01', '9876543213', 'ananya@example.com',
                 1, 1, 1, 1, NOW()),
                (5, 'ADM005', 'MPCA005', 'Venkatesh', 'Venkatesh', 'Rao',
                 'Male', '2006-01-01', '9876543214', 'venkatesh@example.com',
                 1, 1, 1, 1, NOW()),
                (6, 'ADM006', 'MPCA006', 'Priyanka', 'Priyanka', 'Sharma',
                 'Female', '2006-01-01', '9876543215', 'priyanka@example.com',
                 1, 1, 1, 1, NOW());
            ";

            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE `Marks` DROP FOREIGN KEY `FK_Marks_Students`";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "ALTER TABLE `marks` DROP FOREIGN KEY `FK_Marks_Students`";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `Marks` SET `IsLocked` = 0";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE `marks` SET `IsLocked` = 0";
            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();

            try
            {
                cmd.CommandText = "ALTER TABLE Groups ADD COLUMN BoardId INT NOT NULL DEFAULT 1;";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                cmd.CommandText = "ALTER TABLE Groups ADD COLUMN AcademicLevelId INT NOT NULL DEFAULT 1;";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                cmd.CommandText = "UPDATE Groups g JOIN Boards b ON (b.BoardName = g.Board OR b.BoardCode = g.Board OR CAST(b.BoardId AS CHAR) = g.Board) SET g.BoardId = b.BoardId WHERE g.BoardId = 1 OR g.BoardId = 0;";
                cmd.ExecuteNonQuery();
            }
            catch { }

            try
            {
                cmd.CommandText = "UPDATE Groups g JOIN AcademicLevels al ON (al.LevelName = g.AcademicLevel OR CAST(al.AcademicLevelId AS CHAR) = g.AcademicLevel) SET g.AcademicLevelId = al.AcademicLevelId WHERE g.AcademicLevelId = 1 OR g.AcademicLevelId = 0;";
                cmd.ExecuteNonQuery();
            }
            catch { }

            cmd.CommandText = @"
                DROP PROCEDURE IF EXISTS sp_GetAllGroups;
                CREATE PROCEDURE sp_GetAllGroups(
                    IN p_Search VARCHAR(150),
                    IN p_BoardId INT,
                    IN p_AcademicYearId INT,
                    IN p_AcademicLevelId INT,
                    IN p_IsActive BOOLEAN
                )
                BEGIN
                    SELECT
                        g.GroupId,
                        g.BoardId,
                        COALESCE(b.BoardName, '') AS BoardName,
                        g.AcademicYearId,
                        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                        g.AcademicLevelId,
                        COALESCE(al.LevelName, '') AS AcademicLevelName,
                        g.GroupName,
                        g.GroupCode,
                        g.Description,
                        (SELECT COUNT(*) FROM Subjects sub
                         WHERE sub.GroupId = g.GroupId
                         AND sub.IsActive = 1) AS TotalSubjects,
                        g.IsActive,
                        CASE WHEN g.IsActive = 1 THEN 'Active'
                             ELSE 'Inactive' END AS Status,
                        g.CreatedAt,
                        g.UpdatedAt
                    FROM Groups g
                    LEFT JOIN Boards b ON b.BoardId = g.BoardId
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = g.AcademicYearId
                    LEFT JOIN AcademicLevels al
                        ON al.AcademicLevelId = g.AcademicLevelId
                    WHERE (p_Search IS NULL OR TRIM(p_Search) = ''
                           OR g.GroupName LIKE CONCAT('%', TRIM(p_Search), '%')
                           OR g.GroupCode LIKE CONCAT('%', TRIM(p_Search), '%'))
                      AND (p_BoardId IS NULL OR g.BoardId = p_BoardId)
                      AND (p_AcademicYearId IS NULL
                           OR g.AcademicYearId = p_AcademicYearId)
                      AND (p_AcademicLevelId IS NULL
                           OR g.AcademicLevelId = p_AcademicLevelId)
                      AND (p_IsActive IS NULL OR g.IsActive = p_IsActive)
                    ORDER BY g.GroupId DESC;
                END;

                DROP PROCEDURE IF EXISTS sp_GetGroupById;
                CREATE PROCEDURE sp_GetGroupById(IN p_GroupId INT)
                BEGIN
                    SELECT
                        g.GroupId,
                        g.BoardId,
                        COALESCE(b.BoardName, '') AS BoardName,
                        g.AcademicYearId,
                        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                        g.AcademicLevelId,
                        COALESCE(al.LevelName, '') AS AcademicLevelName,
                        g.GroupName,
                        g.GroupCode,
                        g.Description,
                        (SELECT COUNT(*) FROM Subjects sub
                         WHERE sub.GroupId = g.GroupId
                         AND sub.IsActive = 1) AS TotalSubjects,
                        g.IsActive,
                        CASE WHEN g.IsActive = 1 THEN 'Active'
                             ELSE 'Inactive' END AS Status,
                        g.CreatedAt,
                        g.UpdatedAt
                    FROM Groups g
                    LEFT JOIN Boards b ON b.BoardId = g.BoardId
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = g.AcademicYearId
                    LEFT JOIN AcademicLevels al
                        ON al.AcademicLevelId = g.AcademicLevelId
                    WHERE g.GroupId = p_GroupId
                    LIMIT 1;
                END;

                DROP PROCEDURE IF EXISTS sp_GetGroupsByBoard;
                CREATE PROCEDURE sp_GetGroupsByBoard(IN p_BoardId INT)
                BEGIN
                    SELECT
                        g.GroupId,
                        g.BoardId,
                        COALESCE(b.BoardName, '') AS BoardName,
                        g.AcademicYearId,
                        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                        g.AcademicLevelId,
                        COALESCE(al.LevelName, '') AS AcademicLevelName,
                        g.GroupName,
                        g.GroupCode,
                        g.Description,
                        (SELECT COUNT(*) FROM Subjects sub
                         WHERE sub.GroupId = g.GroupId
                         AND sub.IsActive = 1) AS TotalSubjects,
                        g.IsActive,
                        CASE WHEN g.IsActive = 1 THEN 'Active'
                             ELSE 'Inactive' END AS Status,
                        g.CreatedAt,
                        g.UpdatedAt
                    FROM Groups g
                    LEFT JOIN Boards b ON b.BoardId = g.BoardId
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = g.AcademicYearId
                    LEFT JOIN AcademicLevels al
                        ON al.AcademicLevelId = g.AcademicLevelId
                    WHERE g.BoardId = p_BoardId
                      AND g.IsActive = 1
                    ORDER BY g.GroupId DESC;
                END;

                DROP PROCEDURE IF EXISTS sp_GetGroupDropdown;
                CREATE PROCEDURE sp_GetGroupDropdown()
                BEGIN
                    SELECT
                        g.GroupId,
                        g.GroupName,
                        g.GroupCode,
                        g.BoardId,
                        COALESCE(b.BoardName, '') AS BoardName,
                        g.AcademicYearId,
                        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                        g.AcademicLevelId,
                        COALESCE(al.LevelName, '') AS AcademicLevelName
                    FROM Groups g
                    LEFT JOIN Boards b ON b.BoardId = g.BoardId
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = g.AcademicYearId
                    LEFT JOIN AcademicLevels al
                        ON al.AcademicLevelId = g.AcademicLevelId
                    WHERE g.IsActive = 1
                    ORDER BY g.GroupName ASC;
                END;

                DROP PROCEDURE IF EXISTS sp_GetGroupSummary;
                CREATE PROCEDURE sp_GetGroupSummary(IN p_GroupId INT)
                BEGIN
                    SELECT
                        g.GroupId,
                        g.GroupName,
                        g.GroupCode,
                        g.BoardId,
                        COALESCE(b.BoardName, '') AS BoardName,
                        g.AcademicLevelId,
                        COALESCE(al.LevelName, '') AS AcademicLevelName,
                        g.AcademicYearId,
                        COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                        (SELECT COUNT(*) FROM Students st
                         WHERE st.GroupId = g.GroupId) AS TotalStudents,
                        (SELECT COUNT(*) FROM Students st
                         WHERE st.GroupId = g.GroupId
                         AND st.IsActive = 1) AS ActiveStudents,
                        (SELECT COUNT(*) FROM Subjects sub
                         WHERE sub.GroupId = g.GroupId) AS TotalSubjects,
                        (SELECT COUNT(*) FROM Subjects sub
                         WHERE sub.GroupId = g.GroupId
                         AND sub.IsActive = 1) AS ActiveSubjects
                    FROM Groups g
                    LEFT JOIN Boards b ON b.BoardId = g.BoardId
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = g.AcademicYearId
                    LEFT JOIN AcademicLevels al
                        ON al.AcademicLevelId = g.AcademicLevelId
                    WHERE g.GroupId = p_GroupId
                    LIMIT 1;
                END;
            ";

            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();

            cmd.CommandText = "DROP PROCEDURE IF EXISTS sp_GetAdminByEmail;";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"
CREATE PROCEDURE sp_GetAdminByEmail(
    IN p_Email VARCHAR(255)
)
BEGIN
    SELECT *
    FROM `admins`
    WHERE Email = p_Email
    LIMIT 1;
END;";

            cmd.ExecuteNonQuery();
        }
        catch { }

        try
        {
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
                DROP PROCEDURE IF EXISTS sp_GetAllSections;

                CREATE PROCEDURE sp_GetAllSections(
                    IN p_Board VARCHAR(100),
                    IN p_AcademicYearId INT,
                    IN p_Group VARCHAR(100),
                    IN p_GroupId INT,
                    IN p_Programme VARCHAR(100),
                    IN p_AcademicLevel VARCHAR(50),
                    IN p_SearchTerm VARCHAR(100),
                    IN p_IsActive TINYINT(1)
                )
                BEGIN
                    SELECT s.SectionId,
                           s.BoardId,
                           s.Board,
                           s.AcademicYearId,
                           COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                           s.GroupId,
                           s.`Group`,
                           COALESCE(s.Programme, '') AS Programme,
                           s.AcademicLevel,
                           s.SectionName,
                           s.RoomNumber,
                           s.RoomId,
                           COALESCE(r.RoomName, r.RoomNumber, s.RoomNumber, '') AS RoomName,
                           COALESCE(r.BlockName, '') AS BlockName,
                           COALESCE(r.BlockName, '') AS BuildingName,
                           COALESCE(r.BlockName, '') AS Building,
                           COALESCE(r.BlockName, '') AS Block,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS InchargeId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS ClassTeacherId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS TeacherId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS FacultyId,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS InchargeName,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Incharge,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Teacher,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS FacultyName,
                           COALESCE(f.EmployeeId, '') AS FacultyEmployeeId,
                           s.MaximumStrength,
                           s.IsActive,
                           s.CreatedAt,
                           s.UpdatedAt
                    FROM Sections s
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = s.AcademicYearId
                    LEFT JOIN Faculties f
                        ON f.Id = COALESCE(s.InchargeId, s.ClassTeacherId)
                    LEFT JOIN Rooms r
                        ON r.RoomId = s.RoomId
                    WHERE (p_Board IS NULL
                           OR p_Board = ''
                           OR s.Board = p_Board)
                      AND (p_AcademicYearId IS NULL
                           OR p_AcademicYearId = 0
                           OR s.AcademicYearId = p_AcademicYearId)
                      AND (p_Group IS NULL
                           OR p_Group = ''
                           OR s.`Group` = p_Group)
                      AND (p_GroupId IS NULL
                           OR p_GroupId = 0
                           OR s.GroupId = p_GroupId)
                      AND (p_Programme IS NULL
                           OR p_Programme = ''
                           OR s.Programme = p_Programme)
                      AND (p_AcademicLevel IS NULL
                           OR p_AcademicLevel = ''
                           OR s.AcademicLevel = p_AcademicLevel)
                      AND (p_IsActive IS NULL
                           OR s.IsActive = p_IsActive)
                      AND (
                           p_SearchTerm IS NULL
                           OR p_SearchTerm = ''
                           OR s.SectionName LIKE CONCAT('%', p_SearchTerm, '%')
                           OR s.`Group` LIKE CONCAT('%', p_SearchTerm, '%')
                           OR s.Programme LIKE CONCAT('%', p_SearchTerm, '%')
                           OR CONCAT(f.FirstName, ' ', f.LastName)
                              LIKE CONCAT('%', p_SearchTerm, '%')
                           OR s.RoomNumber LIKE CONCAT('%', p_SearchTerm, '%')
                           OR r.RoomName LIKE CONCAT('%', p_SearchTerm, '%')
                           OR r.RoomNumber LIKE CONCAT('%', p_SearchTerm, '%')
                      )
                    ORDER BY s.SectionId DESC;
                END;

                DROP PROCEDURE IF EXISTS sp_GetSectionById;

                CREATE PROCEDURE sp_GetSectionById(IN p_SectionId INT)
                BEGIN
                    SELECT s.SectionId,
                           s.BoardId,
                           s.Board,
                           s.AcademicYearId,
                           COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                           s.GroupId,
                           s.`Group`,
                           COALESCE(s.Programme, '') AS Programme,
                           s.AcademicLevel,
                           s.SectionName,
                           s.RoomNumber,
                           s.RoomId,
                           COALESCE(r.RoomName, r.RoomNumber, s.RoomNumber, '') AS RoomName,
                           COALESCE(r.BlockName, '') AS BlockName,
                           COALESCE(r.BlockName, '') AS BuildingName,
                           COALESCE(r.BlockName, '') AS Building,
                           COALESCE(r.BlockName, '') AS Block,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS InchargeId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS ClassTeacherId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS TeacherId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS FacultyId,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS InchargeName,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Incharge,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Teacher,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS FacultyName,
                           COALESCE(f.EmployeeId, '') AS FacultyEmployeeId,
                           s.MaximumStrength,
                           s.IsActive,
                           s.CreatedAt,
                           s.UpdatedAt
                    FROM Sections s
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = s.AcademicYearId
                    LEFT JOIN Faculties f
                        ON f.Id = COALESCE(s.InchargeId, s.ClassTeacherId)
                    LEFT JOIN Rooms r
                        ON r.RoomId = s.RoomId
                    WHERE s.SectionId = p_SectionId;
                END;

                DROP PROCEDURE IF EXISTS sp_CreateSection;

                CREATE PROCEDURE sp_CreateSection(
                    IN p_Board VARCHAR(100),
                    IN p_BoardId INT,
                    IN p_AcademicYearId INT,
                    IN p_Group VARCHAR(100),
                    IN p_GroupId INT,
                    IN p_Programme VARCHAR(100),
                    IN p_AcademicLevel VARCHAR(50),
                    IN p_SectionName VARCHAR(50),
                    IN p_RoomNumber VARCHAR(50),
                    IN p_InchargeId INT,
                    IN p_MaximumStrength INT,
                    IN p_IsActive TINYINT(1),
                    IN p_RoomId INT
                )
                BEGIN
                    INSERT INTO Sections (
                        Board,
                        BoardId,
                        AcademicYearId,
                        `Group`,
                        GroupId,
                        Programme,
                        AcademicLevel,
                        SectionName,
                        RoomNumber,
                        InchargeId,
                        ClassTeacherId,
                        MaximumStrength,
                        IsActive,
                        RoomId,
                        CreatedAt
                    )
                    VALUES (
                        p_Board,
                        p_BoardId,
                        p_AcademicYearId,
                        p_Group,
                        p_GroupId,
                        COALESCE(p_Programme, ''),
                        p_AcademicLevel,
                        p_SectionName,
                        p_RoomNumber,
                        p_InchargeId,
                        p_InchargeId,
                        p_MaximumStrength,
                        p_IsActive,
                        p_RoomId,
                        UTC_TIMESTAMP()
                    );

                    SELECT LAST_INSERT_ID();
                END;

                DROP PROCEDURE IF EXISTS sp_UpdateSection;

                CREATE PROCEDURE sp_UpdateSection(
                    IN p_SectionId INT,
                    IN p_Board VARCHAR(100),
                    IN p_BoardId INT,
                    IN p_AcademicYearId INT,
                    IN p_Group VARCHAR(100),
                    IN p_GroupId INT,
                    IN p_Programme VARCHAR(100),
                    IN p_AcademicLevel VARCHAR(50),
                    IN p_SectionName VARCHAR(50),
                    IN p_RoomNumber VARCHAR(50),
                    IN p_InchargeId INT,
                    IN p_MaximumStrength INT,
                    IN p_IsActive TINYINT(1),
                    IN p_RoomId INT
                )
                BEGIN
                    UPDATE Sections
                    SET Board = p_Board,
                        BoardId = COALESCE(p_BoardId, BoardId),
                        AcademicYearId = p_AcademicYearId,
                        `Group` = p_Group,
                        GroupId = COALESCE(p_GroupId, GroupId),
                        Programme = COALESCE(p_Programme, ''),
                        AcademicLevel = p_AcademicLevel,
                        SectionName = p_SectionName,
                        RoomNumber = p_RoomNumber,
                        InchargeId = p_InchargeId,
                        ClassTeacherId = p_InchargeId,
                        MaximumStrength = p_MaximumStrength,
                        IsActive = p_IsActive,
                        RoomId = p_RoomId,
                        UpdatedAt = UTC_TIMESTAMP()
                    WHERE SectionId = p_SectionId;
                END;

                DROP PROCEDURE IF EXISTS sp_DeleteSection;

                CREATE PROCEDURE sp_DeleteSection(IN p_SectionId INT)
                BEGIN
                    DELETE FROM Sections
                    WHERE SectionId = p_SectionId;
                END;

                DROP PROCEDURE IF EXISTS sp_ValidateSectionName;

                CREATE PROCEDURE sp_ValidateSectionName(
                    IN p_Board VARCHAR(100),
                    IN p_AcademicYearId INT,
                    IN p_Group VARCHAR(100),
                    IN p_Programme VARCHAR(100),
                    IN p_AcademicLevel VARCHAR(50),
                    IN p_SectionName VARCHAR(50),
                    IN p_ExcludeSectionId INT
                )
                BEGIN
                    SELECT COUNT(1)
                    FROM Sections
                    WHERE Board = p_Board
                      AND AcademicYearId = p_AcademicYearId
                      AND `Group` = p_Group
                      AND (
                          Programme = p_Programme
                          OR (Programme IS NULL AND p_Programme = '')
                          OR (Programme = '' AND p_Programme IS NULL)
                      )
                      AND AcademicLevel = p_AcademicLevel
                      AND SectionName = p_SectionName
                      AND (
                          p_ExcludeSectionId IS NULL
                          OR SectionId <> p_ExcludeSectionId
                      );
                END;

                DROP PROCEDURE IF EXISTS sp_GetSectionsByGroup;

                CREATE PROCEDURE sp_GetSectionsByGroup(IN p_GroupId INT)
                BEGIN
                    SELECT s.SectionId,
                           s.BoardId,
                           s.Board,
                           s.AcademicYearId,
                           COALESCE(ay.AcademicYearName, '') AS AcademicYearName,
                           s.GroupId,
                           s.`Group`,
                           COALESCE(s.Programme, '') AS Programme,
                           s.AcademicLevel,
                           s.SectionName,
                           s.RoomNumber,
                           s.RoomId,
                           COALESCE(r.RoomName, r.RoomNumber, s.RoomNumber, '') AS RoomName,
                           COALESCE(r.BlockName, '') AS BlockName,
                           COALESCE(r.BlockName, '') AS BuildingName,
                           COALESCE(r.BlockName, '') AS Building,
                           COALESCE(r.BlockName, '') AS Block,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS InchargeId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS ClassTeacherId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS TeacherId,
                           COALESCE(s.InchargeId, s.ClassTeacherId) AS FacultyId,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS InchargeName,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Incharge,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS ClassTeacherName,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS Teacher,
                           COALESCE(CONCAT(f.FirstName, ' ', f.LastName), '') AS FacultyName,
                           COALESCE(f.EmployeeId, '') AS FacultyEmployeeId,
                           s.MaximumStrength,
                           s.IsActive,
                           s.CreatedAt,
                           s.UpdatedAt
                    FROM Sections s
                    LEFT JOIN AcademicYears ay
                        ON ay.AcademicYearId = s.AcademicYearId
                    LEFT JOIN Faculties f
                        ON f.Id = COALESCE(s.InchargeId, s.ClassTeacherId)
                    LEFT JOIN Rooms r
                        ON r.RoomId = s.RoomId
                    WHERE s.GroupId = p_GroupId
                       OR s.`Group` = (
                           SELECT GroupName
                           FROM `Groups`
                           WHERE GroupId = p_GroupId
                           LIMIT 1
                       )
                    ORDER BY s.SectionName ASC;
                END;

                DROP PROCEDURE IF EXISTS sp_GetRooms;

                CREATE PROCEDURE sp_GetRooms()
                BEGIN
                    SELECT 
                        RoomId,
                        COALESCE(RoomCode, RoomNumber, '') AS RoomCode,
                        COALESCE(RoomName, RoomNumber, '') AS RoomName,
                        COALESCE(RoomNumber, RoomCode, '') AS RoomNumber,
                        COALESCE(BlockName, BuildingName, '') AS BlockName,
                        COALESCE(BlockName, BuildingName, '') AS Block,
                        COALESCE(BlockName, BuildingName, '') AS Building,
                        COALESCE(BlockName, BuildingName, '') AS BuildingName,
                        Floor,
                        Capacity,
                        RoomType,
                        IsActive,
                        CreatedAt,
                        UpdatedAt
                    FROM Rooms
                    ORDER BY COALESCE(RoomCode, RoomNumber) ASC;
                END;

                DROP PROCEDURE IF EXISTS sp_GetRoomById;

                CREATE PROCEDURE sp_GetRoomById(IN p_RoomId INT)
                BEGIN
                    SELECT 
                        RoomId,
                        COALESCE(RoomCode, RoomNumber, '') AS RoomCode,
                        COALESCE(RoomName, RoomNumber, '') AS RoomName,
                        COALESCE(RoomNumber, RoomCode, '') AS RoomNumber,
                        COALESCE(BlockName, BuildingName, '') AS BlockName,
                        COALESCE(BlockName, BuildingName, '') AS Block,
                        COALESCE(BlockName, BuildingName, '') AS Building,
                        COALESCE(BlockName, BuildingName, '') AS BuildingName,
                        Floor,
                        Capacity,
                        RoomType,
                        IsActive,
                        CreatedAt,
                        UpdatedAt
                    FROM Rooms
                    WHERE RoomId = p_RoomId;
                END;

                DROP PROCEDURE IF EXISTS sp_CreateRoom;

                CREATE PROCEDURE sp_CreateRoom(
                    IN p_RoomCode VARCHAR(50),
                    IN p_RoomName VARCHAR(100),
                    IN p_Capacity INT,
                    IN p_RoomType VARCHAR(50),
                    IN p_Building VARCHAR(100),
                    IN p_Floor VARCHAR(50),
                    IN p_IsActive TINYINT(1)
                )
                BEGIN
                    INSERT INTO Rooms (
                        RoomNumber,
                        RoomCode,
                        RoomName,
                        BlockName,
                        Floor,
                        Capacity,
                        RoomType,
                        IsActive,
                        CreatedAt
                    )
                    VALUES (
                        p_RoomCode,
                        p_RoomCode,
                        COALESCE(p_RoomName, p_RoomCode),
                        p_Building,
                        p_Floor,
                        IFNULL(p_Capacity, 60),
                        IFNULL(p_RoomType, 'Classroom'),
                        IFNULL(p_IsActive, 1),
                        UTC_TIMESTAMP()
                    );
                    SELECT LAST_INSERT_ID();
                END;

                DROP PROCEDURE IF EXISTS sp_UpdateRoom;

                CREATE PROCEDURE sp_UpdateRoom(
                    IN p_RoomId INT,
                    IN p_RoomCode VARCHAR(50),
                    IN p_RoomName VARCHAR(100),
                    IN p_Capacity INT,
                    IN p_RoomType VARCHAR(50),
                    IN p_Building VARCHAR(100),
                    IN p_Floor VARCHAR(50),
                    IN p_IsActive TINYINT(1)
                )
                BEGIN
                    UPDATE Rooms
                    SET RoomNumber = p_RoomCode,
                        RoomCode = p_RoomCode,
                        RoomName = COALESCE(p_RoomName, p_RoomCode),
                        BlockName = p_Building,
                        Floor = p_Floor,
                        Capacity = p_Capacity,
                        RoomType = p_RoomType,
                        IsActive = p_IsActive,
                        UpdatedAt = UTC_TIMESTAMP()
                    WHERE RoomId = p_RoomId;
                END;
            ";

            cmd.ExecuteNonQuery();
        }
        catch { }
    }
    catch { }
}

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

app.UseCors("AllowFrontend");

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";

    if (path.Equals("/detail", StringComparison.OrdinalIgnoreCase) ||
        path.Equals("/api/detail", StringComparison.OrdinalIgnoreCase) ||
        path.Equals("/api/Evaluations/detail", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/Evaluations/admin/detail";
    }
    else if (path.Equals("/status", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/status", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/Evaluations/status", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/Evaluations/admin/status";
    }
    else if (path.Equals("/global-approval", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/global-approval", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/Evaluations/global-approval", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/Evaluations/admin/global-approval";
    }
    else if (path.Equals("/student-matrix", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/student-matrix", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/Evaluations/student-matrix", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/Evaluations/admin/student-matrix";
    }
    else if (path.Equals("/subject-analysis", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/subject-analysis", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/Evaluations/subject-analysis", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/Evaluations/admin/subject-analysis";
    }
    else if (path.Equals("/faculty/entry", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/faculty/entry", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/Evaluations/faculty/entry";
    }
    else if (path.Equals("/list", StringComparison.OrdinalIgnoreCase) ||
             path.Equals("/api/list", StringComparison.OrdinalIgnoreCase))
    {
        context.Request.Path = "/api/Evaluations/admin/list";
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
