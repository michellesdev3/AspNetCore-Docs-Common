namespace SynchronousWithNewtonsoftJson.Controllers;

using Microsoft.AspNetCore.Mvc;

public class RootMenu : Controller
{
    [HttpGet("/menu")]
    public ContentResult GetMenu()
    {
        return
            new ContentResult()
            {
                ContentType = "text/html",
                Content =
@"<html>
  <head>
    <title>ASP.NET Core Streaming Sample - Asynchronous using System.text.Json</title>
  </head>
  <body>
    <h1>ASP.NET Core Streaming Sample</h1>
    <h2>Synchronous using Newtonsoft.Json</h2>
    <a href=""/v1/sing"" target=""_blank"">/v1/sing &ndash; Get song lyrics from a Controller</a><br />
    <a href=""/middleware/sing"" target=""_blank"">/middleware/sing &ndash; Get song lyrics from Middleware</a>
  </body>
</html>"
            };
    }
}
