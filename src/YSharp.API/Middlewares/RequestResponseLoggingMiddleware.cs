using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using YSharp.Domain;
using YSharp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YSharp.API.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            //First, get the incoming request
            var request = context.Request;
            var requestUrl = request.Path.Value.ToLower();
            if (requestUrl.Contains("api") == false)
            {
                //Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);
                return;
            }
            var requestAbsolutelyUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            var requestMethod = request.Method;
            var requestContentType = request.ContentType;
            var requestDate = DateTime.Now;
            //This line allows us to set the reader for the request back at the beginning of its stream.
            enableRewindAsync(request);

            //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
            var buffer = new byte[Convert.ToInt32(request.ContentLength)];

            //...Then we copy the entire request stream into the new buffer.
            await request.Body.ReadAsync(buffer, 0, buffer.Length);

            //We convert the byte[] into a string using UTF8 encoding...
            var requestBodyText = Encoding.UTF8.GetString(buffer);

            //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()

            request.Body.Seek(0, SeekOrigin.Begin);


            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            using (var responseBody = new MemoryStream())
            {
                //...and use that for the temporary response body
                context.Response.Body = responseBody;

                //Continue down the Middleware pipeline, eventually returning to this class
                await _next(context);

                var response = context.Response;
                var responseDate = DateTime.Now;
                using (var serviceScope = ApplicationBuilder.Instance.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    var responseStatusCode = response.StatusCode.ToString();
                    var responseContentType = response.ContentType;
                    var uow = serviceScope.ServiceProvider.GetService<IUnitOfWork>();
                    var accessor = serviceScope.ServiceProvider.GetService<IHttpContextAccessor>();
                    var ip = accessor.HttpContext.Connection.RemoteIpAddress.ToString();
                    response.Body.Seek(0, SeekOrigin.Begin);
                    string responseBodyText = await new StreamReader(response.Body).ReadToEndAsync();
                    response.Body.Seek(0, SeekOrigin.Begin);
                    var spendMilliseconds = (int)(responseDate - requestDate).TotalMilliseconds;
                    uow.Set<AccessLog>().Add(new AccessLog
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Url = requestUrl,
                        AbsolutelyUrl = requestAbsolutelyUrl,
                        RequestBody = requestBodyText,
                        RequestMethod = requestMethod,
                        RequestContentType = requestContentType,
                        RequestIP = ip,
                        ResponseBody = responseBodyText,
                        ResponseContentType = responseContentType,
                        ResponseStatusCode = responseStatusCode,
                        CreateDate = DateTime.Now,
                        RequestDate = requestDate,
                        ResponseDate = responseDate,
                        SpendMilliseconds = spendMilliseconds,
                    });
                    uow.Commit();
                }
                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream);
            }

        }


        private void enableRewindAsync(HttpRequest request)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
                request.Body.DrainAsync(CancellationToken.None);
                request.Body.Seek(0L, SeekOrigin.Begin);
            }
        }
    }
}
