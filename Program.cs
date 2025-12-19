using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
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

var genresEndpoint =app.MapGroup("/genres")
    .WithTags("Genre Endpoints");//grouping the endpoints with common prefix /genres and adding tag for swagger documentation


genresEndpoint.MapGet("/", GetAll)
    .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genre-get"));// Cache the response for 15 seconds
//tag is used to evict or clear the cache when data is changed.


genresEndpoint.MapGet("/{id:int}", GetById);
/*async (int id, IGenreRepository genreRepository) =>
{
var genre = await genreRepository.GetById(id);
if (genre is null)
{
    return Results.NotFound();
}
return Results.Ok(genre);
});*/



genresEndpoint.MapPost("/createGenre", Create);
/*async (Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig) =>
{
await genreRepository.Create(genre);
await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
return Results.Created($"/genre/{genre.Id}", genre);
});*/


genresEndpoint.MapPut("/updateGenre/{id:int}", Update);
/* async (int id,Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig) =>
{
var existingGenre = await genreRepository.GetById(id);
if (existingGenre is null)
{
    return Results.NotFound();
}
await genreRepository.Update(genre);//here we  call the update method of repository and pass the genre object
await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
return Results.NoContent();//204 no content because we are not returning any content
});*/

genresEndpoint.MapDelete("/deleteGenre/{id:int}", Delete);
/* async (int id, IGenreRepository genreRepository, IOutputCacheStore cachecleanig) =>
{
    //var existingGenre = await genreRepository.GetById(id);

    var existingGenre=await genreRepository.ExistGenre(id);
    if (!existingGenre)
    {
        return Results.NotFound();
    }
    await genreRepository.Delete(id);
    await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
    return Results.NoContent();
});*/





//Middleware zone ends here

app.Run();


//so in the development environment, we allowed all the origins to access our api

static async Task<Ok<List<Genre>>> GetAll (IGenreRepository genreRepository)
{
    var genres = await genreRepository.GetAll();
    return TypedResults.Ok(genres);
}

static async Task<Results<Ok<Genre>,NotFound>> GetById (int id, IGenreRepository genreRepository)

//Task<Results<ok, not found>>--->it will return either ok or not found result.we mention the possible return types in the angle brackets because there are multiple return types.
{
    var genre = await genreRepository.GetById(id);
    if (genre is null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(genre);
}


static async Task<Created<Genre>> Create(Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig)
{
    await genreRepository.Create(genre);
    await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
    return TypedResults.Created($"/genre/{genre.Id}", genre);
}


static async Task<Results<NoContent,NotFound>> Update(int id, Genre genre, IGenreRepository genreRepository, IOutputCacheStore cachecleanig)
{
    var existingGenre = await genreRepository.GetById(id);
    if (existingGenre is null)
    {
        return TypedResults.NotFound();
    }
    await genreRepository.Update(genre);//here we  call the update method of repository and pass the genre object
    await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
    return TypedResults.NoContent();//204 no content because we are not returning any content
}

static async Task<Results<NotFound,NoContent>>Delete (int id, IGenreRepository genreRepository, IOutputCacheStore cachecleanig) 
{
    //var existingGenre = await genreRepository.GetById(id);

    var existingGenre = await genreRepository.ExistGenre(id);
    if (!existingGenre)
    {
        return TypedResults.NotFound();
    }
    await genreRepository.Delete(id);
    await cachecleanig.EvictByTagAsync("genre-get", default);//evict the cache with tag "genre-get"
    return TypedResults.NoContent();
};