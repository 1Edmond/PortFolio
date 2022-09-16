namespace PortFolio.Controllers;


public class HomeController : Controller
{
    private readonly PortFolioContext _context;


    public HomeController(PortFolioContext context)
    {
        _context = context;
    }


    [HttpGet]
    [Route("/")]
    public IActionResult Index()
    {
        LoadSetting();
        return View();
    }
    
    [HttpGet]
    [Route("/errorView")]
    public IActionResult Error()
    {
        var Error = JsonConvert.DeserializeObject<CustomError>(HttpContext.Session.GetString("ErrorMessage"));
        return View("../CustomErrorView",Error);
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
        var userInfo = _context.Settings.Where(d => new List<string>()
        {
            "Pseudo",
            "Password"
        }.Contains(d.Name)).ToList();

        if(name == userInfo.Where(d => d.Name == "Pseudo").First().Value && password == userInfo.Where(s => s.Name == "Password").First().Value)
        {
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
        return View("../Auth/Login");
    }


    public void LoadSetting()
    {
        var temp = new List<string>()
            {
                "Name",
            };
        var settings = _context.Settings.Where(s => temp.Contains(s.Name)).ToList();
        var data = new Dictionary<string, string>();
        settings.ForEach(s =>
        {
            data[s.Name] = s.Value;
        });
        ViewBag.CustomSetting = data;
    }


}
