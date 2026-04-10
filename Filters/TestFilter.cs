using AutoMapper;
using Movie_App_MinimalApi.Repositories;

namespace Movie_App_MinimalApi.Filters
{
    public class TestFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, 
            EndpointFilterDelegate next)
        {

            /*to receive parameters: in this way , we have to take care the order of parameter
            
                var param1 = (int)context.Arguments[0]!;
                var param2 = (IActorRepository)context.Arguments[1]!;
                var param3 = (IMapper)context.Arguments[2]!; 
            */

            //method2:
            var param1 = context.Arguments.OfType<int>().FirstOrDefault();
            var param2 = context.Arguments.OfType<IActorRepository>().FirstOrDefault();
            var param3 = context.Arguments.OfType<IMapper>().FirstOrDefault();

            //this code will execute before the endpoint handler is executed
            var result = await next(context);

            //this code will execute after the endpoint handler is executed
            return result;

        }
    }
}
