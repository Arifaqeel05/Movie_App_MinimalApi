using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.DependencyInjection;
using Movie_App_MinimalApi.Endpoints;
using Movie_App_MinimalApi.Entity;
using AutoMapper;
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
builder.Services.AddAutoMapper(typeof(Program)); //automapper service
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
app.MapGroup("/genres")
    .MapGenres();//this is the same method we have created in GenresEndpoints class.this is extension method because we are extending the functionality of RouteGroupBuilder class.
    //grouping the endpoints with common prefix /genres and adding tag for swagger documentation

app.MapGet("/", () => "Movie section");

//Middleware zone ends here

app.Run();


//so in the development environment, we allowed all the origins to access our api