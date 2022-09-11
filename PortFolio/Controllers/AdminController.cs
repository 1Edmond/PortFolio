using PortFolio.Migrations;

namespace PortFolio.Controllers;

public class AdminController : Controller
{
    private readonly PortFolioContext _context;
    public AdminController(PortFolioContext context) => _context = context;


    [Route("admin/")]

#pragma warning disable CS1998 
    public async Task<IActionResult> Index(string notification = null)
#pragma warning restore CS1998 
    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        return View("../Admin/Index");
    }



    #region Projets

    [Route("admin/projets")]

    public async Task<IActionResult> Projets(string notification = null)
    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        var projets = await (from p in _context.Projets
                             join c in _context.Categories on p.CategorieId equals c.Id
                             where p.Etat != 1
                             select new
                             {
                                 p,
                                 c.Libelle
                             }
                       ).ToListAsync();
        ViewBag.Categories = await _context.Categories.Where(x => x.Etat == 0).ToListAsync();
        ViewBag.Projets = projets;
        return View("../Admin/Projet/AllProjet");
    }


    [Route("admin/projets/addd")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> AddProjet()
    {
        var (libelle, description, lien, categorieId) =
            (Request.Form["Libelle"].ToString(), Request.Form["Description"], Request.Form["Lien"], Request.Form["Categorie"]);
        var projet = new Projet()
        {
            Name = libelle,
            Description = description,
            DateAjout = DateTime.Now,
            Etat = 0,
            Lien = lien,
            CategorieId = int.Parse(categorieId)
        };

        var message = new MyNotification();
        if (_context.Projets.Any(p => p.Name == libelle))
            message = new MyNotification()
            {
                Description = $"Le projet {libelle} existe deja.",
                Libelle = "Erreur",
                Type = "danger"
            };
        else
        {
            _context.Add(projet);
            await _context.SaveChangesAsync();
            message = new MyNotification()
            {
                Description = $"Le projet {libelle} a bien ete ajoute.",
                Libelle = "Ajout de projet"
            };
        }


        return RedirectToAction(actionName: nameof(Projets), routeValues: new
        {
            notification = JsonConvert.SerializeObject(message)
        });
    }


    [Route("admin/projets/details/{id}")]
    public async Task<IActionResult> ProjetDetails(int id)
    {

        return View("../Admin/Projet/DetailsProjet", await _context.Projets.FindAsync(id));
    }


    #endregion

    #region Catégorie

    [Route("admin/categories")]
    public async Task<IActionResult> Categories(string notification = null)
    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        return View("../Admin/Categorie/AllCategorie",
            await _context.Categories.Where(c => c.Etat == 0).ToListAsync()
            );
    }
    

    [Route("admin/categories/add")]
#pragma warning disable CS1998 
    public async Task<IActionResult> AddCategorie(string notification = null)
#pragma warning restore CS1998 
    {
        
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        return View("../Admin/Categorie/AddCategorie");
    }
    
    [Route("admin/categories/details/{id}")]
    public async Task<IActionResult> CategorieDetails(int id, string notification = null)
    {

        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        return View("../Admin/Categorie/DetailsCategorie", await _context.Categories.FindAsync(id));
    }
    
    
    
    [Route("admin/categories/delete/{id}")]
    public async Task<IActionResult> CategorieDelete(int id)
    {

        var temp = new MyNotification()
        {
            Description = "La categorie n'existe pas.",
            Libelle = "Erreur",
            Type = "danger"
        };
        if(_context.Categories.Any(c => c.Id == id))
        {
            var cat = _context.Categories.Find(id);
            cat.Etat = 1;
            _context.Update(cat);
            await _context.SaveChangesAsync();
            temp = new MyNotification()
            {
                Description = "Suppression de la categorie reussie.",
                Libelle = "Reussie",
            };
        }

        return RedirectToAction(nameof(Categories), routeValues: new
        {
            notification = JsonConvert.SerializeObject(temp)
        });

    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/categories/add")]

    public async Task<ActionResult> AddCategorieMethode()
    {

        var (libelle, description ) = ( Request.Form["Libelle"].ToString() , Request.Form["Description"].ToString());

        var temp = "";
        if(_context.Categories.Any(c => c.Libelle == libelle))
            temp = JsonConvert.SerializeObject(new MyNotification()
            {
                Description = $"La categorie {Request.Form["Libelle"]} existe deja.",
                Libelle = "Erreur",
                Type = "danger"
            });
        else
        {
            _context.Add(new Categorie()
            {
                DateAjout = DateTime.Now,
                Etat = 0,
                Libelle = libelle,
                Description = description
            });
            await _context.SaveChangesAsync();
            temp = JsonConvert.SerializeObject(new MyNotification()
            {
                Description = $"La categorie {Request.Form["Libelle"]} a bien ete ajoutee.",
                Libelle = "Ajout de categorie"
            });
        }
        return RedirectToAction(nameof(AddCategorie), routeValues : new
        {
            notification = temp
        });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/categories/update")]
    public async Task<ActionResult> UpdateCategorie()

    {

        var (libelle, description ) = ( Request.Form["Libelle"].ToString() , Request.Form["Description"].ToString());
        var temp = "";
        var oldCategorie = _context.Categories.Find(int.Parse(Request.Form["Id"].ToString()));
        if(libelle != oldCategorie.Libelle)
        {
            oldCategorie.Libelle = libelle;
            temp = "Modification apporte avec succes.";
        }

        if(description != oldCategorie.Description)
        {
            oldCategorie.Description = description;
            temp = "Modification apporte avec succes.";
        }
        if (temp != "")
        {
            _context.Categories.Update(oldCategorie);
            await _context.SaveChangesAsync();
        }


        return RedirectToAction(nameof(CategorieDetails), routeValues : new
        {
            oldCategorie.Id,
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Description = temp,
                Libelle = "Modification de categorie"
            })
        });
    }

    #endregion

}
