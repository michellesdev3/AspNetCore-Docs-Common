#define MIDDLEWARE //  MIDDLEWARE API_CONTROLLER API_CONT_SHORT
#if NEVER
#elif MIDDLEWARE
// <snippet_middleware>
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStatusCodePages();

// Middleware to handle writing problem details to the response.
app.Use(async (context, next) =>
{
    await next(context);
    var mathErrorFeature = context.Features.Get<MathErrorFeature>();
    if (mathErrorFeature is not null)
    {
        if (context.RequestServices.GetService<IProblemDetailsService>() is
                                                           { } problemDetailsService)
        {
            (string Detail, string Type) details = mathErrorFeature.MathError switch
            {
                MathErrorType.DivisionByZeroError => ("The number you inputed is zero",
                "https://en.wikipedia.org/wiki/Division_by_zero"),
                _ => ("Negative or complex numbers are not handled", 
                "https://en.wikipedia.org/wiki/Square_root")
            };

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails =
                {
                    Title = "Bad Input",
                    Detail = details.Detail,
                    Type = details.Type
                }
            });
        }
    }
});

// /divide?numerator=2&denominator=4
app.MapGet("/divide", (HttpContext context, double numerator, double denominator) =>
{
    if (denominator == 0)
    {
        var errorType = new MathErrorFeature { MathError =
                                               MathErrorType.DivisionByZeroError };
        context.Features.Set(errorType);
        return Results.BadRequest();
    }

    return Results.Ok(numerator / denominator);
});

// /squareroot?radicand=16
app.MapGet("/squareroot", (HttpContext context, double radicand) =>
{
    if (radicand < 0)
    {
        var errorType = new MathErrorFeature { MathError =
                                               MathErrorType.NegativeRadicandError };
        context.Features.Set(errorType);
        return Results.BadRequest();
    }

    return Results.Ok(Math.Sqrt(radicand));
});

app.MapControllers();

app.Run();
// </snippet_middleware>
#elif API_CONTROLLER
// <snippet_api_controller>
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails(options =>
        options.CustomizeProblemDetails = (context) =>
        {

            var mathErrorFeature = context.HttpContext.Features
                                                       .Get<MathErrorFeature>();
            if (mathErrorFeature is not null)
            {
                (string Detail, string Type) details = mathErrorFeature.MathError switch
                {
                    MathErrorType.DivisionByZeroError =>
                    ("Divison by zero is not defined.",
                                          "https://wikipedia.org/wiki/Division_by_zero"),
                    _ => ("Negative or complex numbers are not valid input.",
                                          "https://wikipedia.org/wiki/Square_root")
                };

                context.ProblemDetails.Type = details.Type;
                context.ProblemDetails.Title = "Bad Input";
                context.ProblemDetails.Detail = details.Detail;
            }
        }
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

app.UseAuthorization();

app.MapControllers();

// /divide?numerator=2&denominator=4
app.MapGet("/divide", (HttpContext context, double numerator, double denominator) =>
{
    if (denominator == 0)
    {
        var errorType = new MathErrorFeature
        {
            MathError = MathErrorType.DivisionByZeroError
        };
        context.Features.Set(errorType);
        return Results.BadRequest();
    }

    var calculation = numerator / denominator;
    return Results.Ok(calculation);
});

// /squareroot?radicand=16
app.MapGet("/squareroot", (HttpContext context, double radicand) =>
{
    if (radicand < 0)
    {
        var errorType = new MathErrorFeature
        {
            MathError = MathErrorType.NegativeRadicandError
        };
        context.Features.Set(errorType);
        return Results.BadRequest();
    }

    return Results.Ok(Math.Sqrt(radicand));
});

app.Run();
// </snippet_api_controller>

#elif API_CONT_SHORT
// <snippet_apishort>
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapControllers();
app.Run();
// </snippet_apishort>
#endif
