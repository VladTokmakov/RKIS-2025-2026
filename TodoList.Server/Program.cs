using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TodoList.Server
{
    internal static class Program
    {
        private const string Prefix = "http://localhost:5000/";
        private const string ProfilesFileName = "server_profiles.dat";
        private static readonly string DataDir = Path.Combine(AppContext.BaseDirectory, "data");

        private static async Task Main()
        {
            Directory.CreateDirectory(DataDir);
            using var listener = new HttpListener();
            listener.Prefixes.Add(Prefix);
            listener.Start();
            
            Console.WriteLine($"TodoList.Server started at {Prefix}");
            Console.WriteLine("Press Ctrl+C to stop.");
            
            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                _ = ProcessRequestAsync(context);
            }
        }

        private static async Task ProcessRequestAsync(HttpListenerContext context)
        {
            try
            {
                HttpListenerRequest request = context.Request;
                string[] segments = GetSegments(request.Url?.AbsolutePath);
                
                if (segments.Length == 1 && segments[0].Equals("profiles", StringComparison.OrdinalIgnoreCase))
                {
                    await HandleProfilesAsync(context);
                    return;
                }
                
                if (segments.Length == 2 && segments[0].Equals("todos", StringComparison.OrdinalIgnoreCase))
                {
                    await HandleTodosAsync(context, segments[1]);
                    return;
                }
                
                if (segments.Length == 0 && request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
                {
                    await WriteTextResponseAsync(context.Response, HttpStatusCode.OK, "TodoList.Server is running.");
                    return;
                }
                
                await WriteTextResponseAsync(context.Response, HttpStatusCode.NotFound, "Endpoint not found.");
            }
            catch (Exception ex)
            {
                await WriteTextResponseAsync(context.Response, HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                context.Response.Close();
            }
        }

        private static async Task HandleProfilesAsync(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string path = Path.Combine(DataDir, ProfilesFileName);
            
            if (request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                byte[] payload = await ReadRequestBodyAsync(request);
                await File.WriteAllBytesAsync(path, payload);
                response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }
            
            if (request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                byte[] payload = File.Exists(path) ? await File.ReadAllBytesAsync(path) : Array.Empty<byte>();
                await WriteBinaryResponseAsync(response, payload);
                return;
            }
            
            await WriteMethodNotAllowedAsync(response);
        }

        private static async Task HandleTodosAsync(HttpListenerContext context, string rawUserId)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            
            if (!Guid.TryParse(Uri.UnescapeDataString(rawUserId), out Guid userId))
            {
                await WriteTextResponseAsync(response, HttpStatusCode.BadRequest, "Invalid userId.");
                return;
            }
            
            string path = Path.Combine(DataDir, $"server_todos_{userId}.dat");
            
            if (request.HttpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                byte[] payload = await ReadRequestBodyAsync(request);
                await File.WriteAllBytesAsync(path, payload);
                response.StatusCode = (int)HttpStatusCode.NoContent;
                return;
            }
            
            if (request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                byte[] payload = File.Exists(path) ? await File.ReadAllBytesAsync(path) : Array.Empty<byte>();
                await WriteBinaryResponseAsync(response, payload);
                return;
            }
            
            await WriteMethodNotAllowedAsync(response);
        }

        private static async Task<byte[]> ReadRequestBodyAsync(HttpListenerRequest request)
        {
            await using var buffer = new MemoryStream();
            await request.InputStream.CopyToAsync(buffer);
            return buffer.ToArray();
        }

        private static async Task WriteBinaryResponseAsync(HttpListenerResponse response, byte[] payload)
        {
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "application/octet-stream";
            response.ContentLength64 = payload.Length;
            if (payload.Length > 0)
                await response.OutputStream.WriteAsync(payload.AsMemory(0, payload.Length));
        }

        private static async Task WriteTextResponseAsync(HttpListenerResponse response, HttpStatusCode code, string text)
        {
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(text ?? string.Empty);
            response.StatusCode = (int)code;
            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = payload.Length;
            if (payload.Length > 0)
                await response.OutputStream.WriteAsync(payload.AsMemory(0, payload.Length));
        }

        private static async Task WriteMethodNotAllowedAsync(HttpListenerResponse response)
        {
            response.Headers["Allow"] = "GET, POST";
            await WriteTextResponseAsync(response, HttpStatusCode.MethodNotAllowed, "Method not allowed.");
        }

        private static string[] GetSegments(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return Array.Empty<string>();
            return path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}