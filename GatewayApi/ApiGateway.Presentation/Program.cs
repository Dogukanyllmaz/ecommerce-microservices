using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using eCommerceSharedLibrary.DependencyInjection;
using ApiGateway.Presentation.Middleware;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

//ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange:true);
builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
//JWT
JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(builder => {
        builder.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin();
        });
});

var app = builder.Build();

app.UseCors();
app.UseMiddleware<AttachSignatureToRequest>();
app.UseOcelot().Wait();
app.UseHttpsRedirection();
app.Run();
