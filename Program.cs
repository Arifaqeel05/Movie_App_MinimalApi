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


//in this endpoint,we are using other policy named "free".it will override the default policY



//here ,we are using default policy which is defined in the middleware
app.MapGet("/getAllGenre", async(IGenreRepository genreRepository) =>
{
    return await genreRepository.GetAll();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));// Cache the response for 15 seconds

app.MapGet("/getById/{id:int}", async (int id, IGenreRepository genreRepository) =>
{
    var genre = await genreRepository.GetById(id);
    if (genre is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(genre);
});

app.MapPost("/createGenre", async (Genre genre, IGenreRepository genreRepository) =>
{
    await genreRepository.Create(genre);
    return TypedResults.Created($"/genre/{genre.Id}",genre);
});


//Middleware zone ends here

app.Run();


//so in the development environment, we allowed all the origins to access our api