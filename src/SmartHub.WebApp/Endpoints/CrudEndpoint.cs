using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using SmartHub.WebApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace SmartHub.WebApp.Endpoints
{
    public static class CrudEndpoint
    {

        public static IEndpointConventionBuilder MapCrud<TEntity>(this IEndpointRouteBuilder endpoints, 
                                                                 string pattern,
                                                                 Func<AppDbContext, DbSet<TEntity>> dbSetFunc)
             where TEntity : class
        {
            return endpoints.Map(pattern, async httpContext => {

                var req = httpContext.Request;
                var dbContext = httpContext.RequestServices.GetService<AppDbContext>();

                switch (req.Method)
                {
                    case "POST":
                        {
                            var model = await req.ReadFromJsonAsync<TEntity>();
                            await dbContext.AddAsync(model);
                            await dbContext.SaveChangesAsync();
                            break;
                        }
                    case "GET":
                        {
                            string id = req.Query["id"];
                            var data = await dbSetFunc(dbContext).FindAsync(id);
                            await httpContext.Response.WriteAsJsonAsync(data);
                            break;
                        }
                    case "PUT":
                        {
                            var model = await req.ReadFromJsonAsync<TEntity>();
                            string id = req.Query["id"];
                            var data = await dbSetFunc(dbContext).FindAsync(id);
                            if (data is null)
                                await dbContext.AddAsync(model);
                            else
                                dbContext.Update(model);
                            await dbContext.SaveChangesAsync();
                            break;
                        }

                    default:

                        httpContext.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                        break;

                }
            
            
            });
        }


    }
}
