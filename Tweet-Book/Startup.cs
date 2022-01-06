using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Tweet_Book.Installers;
using Tweet_book.Contracts.HealthChecks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Tweet_Book
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.InstallServicesAssembly(Configuration);
            services.AddAutoMapper(typeof(Startup));
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
               // app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tweetbook v1"));
            }
            else
            {
               // app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHealthChecks("/",new HealthCheckOptions { 
                ResponseWriter = async(context, report)=>{
                    context.Response.ContentType = "application/json";
                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(x=> new HealthCheck
                        {
                            Status = x.Value.Status.ToString(),
                            Component = x.Key,
                            Description = x.Value.Description

                        }),
                        Duration = report.TotalDuration

                    };
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                }
            });
            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapControllerRoute(
                //    name: "default",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });
            //app.UseHealthChecks("/");
        }
    }
}
