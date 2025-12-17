using Microsoft.AspNetCore.Cors;
using Movie_App_MinimalApi.Entity;
using Movie_App_MinimalApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

//service zone starts here


//add cors service
builder.Services.AddCors(myCorsSetting =>
{
    myCorsSetting.AddDefaultPolicy(permissions =>
    {
        //permissions is a parameter name, you can use any name
        //fetching allowed origin from appsettings.Develoment.json-->builder.Configuration["allowedOrigin"]!
        permissions
                   .WithOrigins(builder.Configuration["allowedOrigin"]!)
                   .AllowAnyHeader()
                   .AllowAnyMethod();
    });

    //add other policy-->we have to give policy name here it is "free"
    //define it for the understanding purpose.only. as we have allowed origin in the default policy
    /*myCorsSetting.AddPolicy("free", permissions =>
    {
             permissions
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
    });*/
});

builder.Services.AddOutputCache();

//dependency injection for repository
builder.Services.AddScoped<IGenreRepository, GenreRepository>();

//swagger service
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//service zone ends here

var app = builder.Build();


//Middleware zone starts here

//as we need the swagger in the development environment only so we are checking the environment
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(); //enable cors middleware with default policy.it will be applied globally 

app.UseOutputCache();


//in this endpoint,we are using other policy named "free".it will override the default policy
app.MapGet("/", () => "Hello World!");



//here ,we are using default policy which is defined in the middleware
app.MapGet("/genre", () =>
{
    var genres = new List<Genre>()
    {
        new Genre() { Id = 1, Name = "Action" },
        new Genre() { Id = 2, Name = "Comedy" },
        new Genre() { Id = 3, Name = "Drama" }
    };
    return genres;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));// Cache the response for 15 seconds

app.MapPost("/genre ", async (Genre genre, IGenreRepository genreRepository) =>
{
    var result = await genreRepository.Create(genre);
    return Results.Ok();
});


//Middleware zone ends here

app.Run();


//so in the development environment, we allowed all the origins to access our api