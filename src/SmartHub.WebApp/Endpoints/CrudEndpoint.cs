using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using SmartHub.Logic.Data;
using SmartHub.WebApp.Util;

namespace SmartHub.WebApp.Endpoints
{
    public static class CrudEndpoint
    {

        public static IEndpointConventionBuilder MapCrud<TEntity>(this IEndpointRouteBuilder endpoints,
                                                                 string pattern,
                                                                 Func<AppDbContext, DbSet<TEntity>> dbSetFunc)
             where TEntity : class
        {

            return new MultiEndpointConventionBuilder
            {
                endpoints.Map(pattern, async httpContext => {

                    var req = httpContext.Request;
                    var dbContext = httpContext.RequestServices.GetService<AppDbContext>();
                    var cancellationToken = httpContext.RequestAborted;

                    switch (req.Method)
                    {
                        case "POST":
                            {
                                var model = await req.ReadFromJsonAsync<TEntity>(cancellationToken);
                                await dbContext.AddAsync(model, cancellationToken);
                                await dbContext.SaveChangesAsync(cancellationToken);
                                break;
                            }
                        case "GET":
                            {
                                var data = await dbSetFunc(dbContext).ToListAsync(cancellationToken);
                                await httpContext.Response.WriteAsJsonAsync(data, cancellationToken);
                                break;
                            }


                        default:

                            httpContext.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            break;

                    }

                }),
                endpoints.Map($"{pattern}/{{id}}", async httpContext => {

                    var req = httpContext.Request;
                    var dbContext = httpContext.RequestServices.GetService<AppDbContext>();
                    var cancellationToken = httpContext.RequestAborted;

                    switch (req.Method)
                    {
                       case "GET":
                            {
                                int id = int.Parse(req.RouteValues["id"].ToString());
                                var data = await dbSetFunc(dbContext).FindAsync(new object[]{ id }, cancellationToken);
                                await httpContext.Response.WriteAsJsonAsync(data, cancellationToken);
                                break;
                            }
                        case "PUT":
                            {
                                var model = await req.ReadFromJsonAsync<TEntity>(cancellationToken);
                                int id = int.Parse(req.RouteValues["id"].ToString());
                                var data = await dbSetFunc(dbContext).FindAsync(new object[]{ id }, cancellationToken);
                                dbContext.Update(model);
                                await dbContext.SaveChangesAsync(cancellationToken);
                                break;
                            }
                        case "DELETE":
                            {
                                int id = int.Parse(req.RouteValues["id"].ToString());
                                var model = await dbSetFunc(dbContext).FindAsync(new object[]{ id }, cancellationToken);
                                dbContext.Remove(model);
                                await dbContext.SaveChangesAsync(cancellationToken);
                                break;
                            }


                        default:
                            httpContext.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
                            break;

                    }

                })
            };





        }


    }
}
