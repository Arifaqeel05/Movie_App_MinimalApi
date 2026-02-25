using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Movie_App_MinimalApi.DTOs;
using Movie_App_MinimalApi.Entity;
using Movie_App_MinimalApi.Repositories;

namespace Movie_App_MinimalApi.Endpoints
{
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapCommentsEndpoints(this RouteGroupBuilder group)
        {
            //group.MapGet("/comments", GetComments);
            group.MapPost("/", Create);
            //group.MapPut("/comments/{id}", UpdateComment);
            //group.MapDelete("/comments/{id}", DeleteComment);
            return group;
        }

        static async Task<Results<Created<CommentDTO>, NotFound>>Create(int movieId, CreateCommentDTO createCommentDTO,
            ICommentRepository commentRepository, IMovieRepository movieRepository , IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            if(!await movieRepository.IsExist(movieId))
            {
                return TypedResults.NotFound();
            }
            
            var comment = mapper.Map<Comment>(createCommentDTO);
            //this will map the properties of createCommentDTO to comment entity,simply we are converting createCommentDTO to comment entity
            comment.MovieId = movieId;//assigning movieId to comment entity, coming from the route parameter

            var id= await commentRepository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comment-get", default);//evicting the cache for the movie details when a new comment is added, so that next time when we fetch the movie details it will fetch the updated comments from the database
            //this will create the comment in the database and return the id of the created comment

            var commentDTO = mapper.Map<CommentDTO>(comment);//mapping the created comment entity to commentDTO to return the response
            return TypedResults.Created($"/comment/{id}", commentDTO);//returning the created response with the location of the created comment and the commentDTO as the response body
        }
    }
}
