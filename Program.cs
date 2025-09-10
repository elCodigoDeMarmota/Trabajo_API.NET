using Microsoft.EntityFrameworkCore;
using Trabajo_API_NET.Data;
using Trabajo_API_NET.Servicios;

namespace Trabajo_API_NET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddSingleton<IGeneradorID, GeneradorID>();
            builder.Services.AddScoped<BuscarSnippet>();

            builder.Services.AddHttpClient("Highlighter", c =>
            {
                c.BaseAddress = new Uri("http://localhost:5032");
                c.Timeout = TimeSpan.FromSeconds(10);
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebUI", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") 
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowWebUI"); 

            app.UseAuthorization();

            app.MapControllers();  

            app.Run();

            builder.Services.AddHttpClient("Highlighter", (sp, c) =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                c.BaseAddress = new Uri(cfg["Highlighter:BaseUrl"]!);
            });
        }
    }
}
