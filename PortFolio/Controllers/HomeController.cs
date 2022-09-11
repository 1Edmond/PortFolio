namespace PortFolio.Controllers;


public class HomeController : Controller
{
    private readonly PortFolioContext _context;


    public HomeController(PortFolioContext context)
    {
        _context = context;
    }

    [Route("/")]
    public IActionResult Index()
    {
       
        return View();
    }

    [HttpGet]
    [Route("/Auth/Login")]
    public IActionResult Login()
    {
        return View("../Auth/Login");
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("/Auth/Login")]
    public ActionResult FormLogin()
    {

        var (name, password) = (Request.Form["Pseudo"].ToString(), Request.Form["Password"].ToString());
        HttpContext.Session.SetString("test", "Ok");
        var route = HttpContext.Session.GetString("route")[1..];
        var temp = JsonConvert.SerializeObject(new MyNotification()
        {
            Libelle = "Connexion",
            Description = "Connexion reussie."
        });
        return route switch
        {
            "admin/projets" => RedirectToAction(actionName: "Projets", controllerName: "Admin",
                            routeValues: new { notification = temp}),
            "admin/categories" => RedirectToAction(actionName: "Categories", controllerName: "Admin",
                            routeValues: new { notification = temp }),
            _ => RedirectToAction(actionName: "Index", controllerName: "Admin",
                                routeValues: new { notification = temp}),
        };

    }
       
    
}
