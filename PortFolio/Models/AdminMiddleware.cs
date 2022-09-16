using System.Net;

namespace PortFolio.Models;

public class AdminMiddleware
{
	private readonly RequestDelegate _next;

	public AdminMiddleware()
	{

	}
	public AdminMiddleware(RequestDelegate requestDelegate) => _next = requestDelegate;
	public async Task Invoke(HttpContext httpContext)
	{
		try
		{

			if (!httpContext.Request.Path.Value.Contains('.'))
			{
				if (Constants.AuthorizedRoute.ContainUrl(httpContext.Request.Path.Value))
				{
					if (httpContext.Request.Path.Value.Contains("admin"))
					{
						if (httpContext.Session.Keys.Contains("test"))
							await _next(httpContext);
						else
						{
							httpContext.Session.SetString("route", httpContext.Request.Path.Value);
							httpContext.Request.Path = "/Auth/Login";
							await _next(httpContext);
							return;
						}
					}
					else
						await _next(httpContext);
				}
				else
					throw new KeyNotFoundException();
			}
			else
			{
				var context = httpContext.Request.Path.Value;

				if (context.ContainExtension())
					await _next(httpContext);
				else
					throw new KeyNotFoundException();
			}

		}
		catch (Exception error)
		{
			var response = httpContext.Response;
			response.StatusCode = error switch
			{
				KeyNotFoundException e => (int)HttpStatusCode.NotFound,// not found error
				_ => (int)HttpStatusCode.InternalServerError,// unhandled error
			};
			var result = JsonConvert.SerializeObject(new CustomError()
			{
				ErrorMessage = error.Message,
				StatusCode = response.StatusCode
			});
			httpContext.Session.SetString("ErrorMessage", result);
			httpContext.Request.Path = "/errorView";
			await _next(httpContext);
			return;

		}

		

	}

}
