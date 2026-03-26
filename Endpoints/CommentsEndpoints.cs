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
            group.MapGet("/", GetAll).
                CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).
                Tag("comment-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create);
            group.MapPut("/{id:int}", UpdateComment);
            group.MapDelete("/{id:int}", DeleteComment);
            return group;
        }

        static async Task<Results<Ok<List<CommentDTO>>, NotFound>> GetAll(int movieId,
            ICommentRepository commentRepository, IMovieRepository movieRepository, IMapper mapper)
        {
            if (!await movieRepository.IsExist(movieId))//check movie with given id exist or not.
            {
                return TypedResults.NotFound();
            }
            var comments = await commentRepository.GetAll(movieId); //fetching all comments for the given movieId from the database
            var commentsDTO = mapper.Map<List<CommentDTO>>(comments);//mapping the list of comment entities to list of commentDTOs to return the response
            return TypedResults.Ok(commentsDTO);//returning the list of commentDTOs as the response
        }

        static async Task<Results<Ok<CommentDTO>, NotFound>> GetById(int id, ICommentRepository commentRepository,
            IMovieRepository movieRepository, IMapper mapper)
        {
            if (!await movieRepository.IsExist(id))
            {
                return TypedResults.NotFound();
            }
            var comments = await commentRepository.GetById(id);
            if (comments is null)
            {
                return TypedResults.NotFound();
            }
            var commentDTO = mapper.Map<CommentDTO>(comments);
            return TypedResults.Ok(commentDTO);
        }


        static async Task<Results<Created<CommentDTO>, NotFound>> Create(int movieId, CreateCommentDTO createCommentDTO,
            ICommentRepository commentRepository, IMovieRepository movieRepository, IMapper mapper,
            IOutputCacheStore outputCacheStore)
        {
            if (!await movieRepository.IsExist(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            //this will map the properties of createCommentDTO to comment entity,simply we are converting createCommentDTO to comment entity
            comment.MovieId = movieId;//assigning movieId to comment entity, coming from the route parameter

            var id = await commentRepository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comment-get", default);//evicting the cache for the movie details when a new comment is added, so that next time when we fetch the movie details it will fetch the updated comments from the database
            //this will create the comment in the database and return the id of the created comment

            var commentDTO = mapper.Map<CommentDTO>(comment);//mapping the created comment entity to commentDTO to return the response
            return TypedResults.Created($"/comment/{id}", commentDTO);//returning the created response with the location of the created comment and the commentDTO as the response body
        }

        static async Task<Results<NoContent, NotFound>> UpdateComment(int id, int movieId, CreateCommentDTO createCommentDTO,
            ICommentRepository commentRepository, IMovieRepository movieRepository, IMapper mapper, IOutputCacheStore outputCacheStore)
        {
            if (!await movieRepository.IsExist(id))
            {
                return TypedResults.NotFound();
            }

            if (!await commentRepository.IsExist(id))
            {
                return TypedResults.NotFound();
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.Id = id;//assigning the id of the comment to be updated to the comment entity
            comment.MovieId = movieId;//assigning the movieId to the comment entity

            await commentRepository.Update(comment);//updating the comment in the database
            await outputCacheStore.EvictByTagAsync("comment-get", default);//evicting the cache for the movie details when a comment is updated, so that next time when we fetch the movie details it will fetch the updated comments from the database

            return TypedResults.NoContent();//returning no content response after successful update
        }

        static async Task<Results<NoContent, NotFound>> DeleteComment(int id, int movieId, ICommentRepository commentRepository,
            IMovieRepository movieRepository, IOutputCacheStore outputCacheStore)
        {
            if (!await movieRepository.IsExist(id))
            {
                return TypedResults.NotFound();
            }
            if (!await commentRepository.IsExist(id))
            {
                return TypedResults.NotFound();
            }
            await commentRepository.Delete(id);//deleting the comment from the database
            await outputCacheStore.EvictByTagAsync("comment-get", default);//evicting the cache for the movie details when a comment is deleted, so that next time when we fetch the movie details it will fetch the updated comments from the database
            return TypedResults.NoContent();//returning no content response after successful deletion
        }
    }
}
