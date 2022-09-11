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
		if(httpContext.Request.Path.Value.Contains("admin") && !httpContext.Request.Path.Value.Contains('.'))
		{
			if (httpContext.Session.Keys.Contains("test"))
				await _next(httpContext);
			else
			{
				httpContext.Session.SetString("route",httpContext.Request.Path.Value);
				httpContext.Request.Path = "/Auth/Login";
				await _next(httpContext);
				return;
			}
		}
		else
			await _next(httpContext);

	}

}
