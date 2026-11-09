using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Boardy.Backend.Autorizacion;
using Boardy.Backend.Comun;
using Boardy.Backend.Data;
using Boardy.Backend.Models;
using Boardy.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        options.ModelMetadataDetailsProviders.Add(new MensajesValidacionEsProvider());
        MensajesBindingEs.Aplicar(options.ModelBindingMessageProvider);
    })
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityCore<Usuario>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole>()
    .AddErrorDescriber<DescriptorErroresIdentity>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

var opcionesJwt = builder.Configuration.GetSection(OpcionesJwt.Seccion).Get<OpcionesJwt>();
if (opcionesJwt is null || opcionesJwt.Clave.Length < 32)
{
    throw new InvalidOperationException("Falta configurar Jwt:Clave (al menos 32 caracteres).");
}

builder.Services.Configure<OpcionesJwt>(builder.Configuration.GetSection(OpcionesJwt.Seccion));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = opcionesJwt.Emisor,
            ValidateAudience = true,
            ValidAudience = opcionesJwt.Audiencia,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(opcionesJwt.Clave)),
            ValidateLifetime = true
        };

        // Rechaza el token si el usuario fue desactivado o si su sello cambió (rol global, eliminación de cuenta).
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async contexto =>
            {
                var userManager = contexto.HttpContext.RequestServices.GetRequiredService<UserManager<Usuario>>();
                var usuarioId = contexto.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                var usuario = usuarioId is null ? null : await userManager.FindByIdAsync(usuarioId);

                if (usuario is null || !usuario.Estado
                    || usuario.SecurityStamp != contexto.Principal!.FindFirstValue(OpcionesJwt.ClaimSelloSeguridad))
                {
                    contexto.Fail("La sesión ya no es válida.");
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PoliticasTablero.VerTablero, politica => politica.AddRequirements(
        new RolTableroRequirement(RolesTablero.Propietario, RolesTablero.Administrador, RolesTablero.Operario, RolesTablero.Lector)));

    options.AddPolicy(PoliticasTablero.ColaborarEnTablero, politica => politica.AddRequirements(
        new RolTableroRequirement(RolesTablero.Propietario, RolesTablero.Administrador, RolesTablero.Operario)));

    options.AddPolicy(PoliticasTablero.AdministrarTablero, politica => politica.AddRequirements(
        new RolTableroRequirement(RolesTablero.Propietario, RolesTablero.Administrador)));

    options.AddPolicy(PoliticasTablero.GestionarTablero, politica => politica.AddRequirements(
        new RolTableroRequirement(RolesTablero.Propietario)));

    options.AddPolicy(PoliticasTarea.TrabajarEnTarea, politica => politica.AddRequirements(
        new TrabajoEnTareaRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, RolTableroHandler>();
builder.Services.AddScoped<IAuthorizationHandler, TrabajoEnTareaHandler>();

builder.Services.AddScoped<IAutenticacionService, AutenticacionService>();
builder.Services.AddScoped<IEstadoTareaService, EstadoTareaService>();
builder.Services.AddScoped<IRolMiembroService, RolMiembroService>();
builder.Services.AddScoped<ITableroService, TableroService>();
builder.Services.AddScoped<IMiembroTableroService, MiembroTableroService>();
builder.Services.AddScoped<IInvitacionService, InvitacionService>();
builder.Services.AddScoped<IListaTareaService, ListaTareaService>();
builder.Services.AddScoped<IEtiquetaService, EtiquetaService>();
builder.Services.AddScoped<ITareaService, TareaService>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAuditoriaService, AuditoriaService>();
builder.Services.AddScoped<IFotoPerfilService, FotoPerfilService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var esquema = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = JwtBearerDefaults.AuthenticationScheme }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, esquema);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement { [esquema] = Array.Empty<string>() });
});

var app = builder.Build();

await DbSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // En desarrollo el emulador de Android llama por HTTP (10.0.2.2) sin certificado de desarrollo.
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
