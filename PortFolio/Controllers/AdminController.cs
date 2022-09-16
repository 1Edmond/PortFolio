using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace PortFolio.Controllers;

public class AdminController : Controller
{
    private readonly PortFolioContext _context;
    public AdminController(PortFolioContext context)
    {
        _context = context;
        
    }


    [Route("admin/logout")]
    public ActionResult LogOut()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", controllerName : "Home");
    }



    [Route("admin/")]

    public IActionResult Index(string notification = null)

    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        LoadSetting();
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
        LoadSetting();
        return View("../Admin/Projet/AllProjet");
    }


    [Route("admin/projets/add")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> AddProjet()
    {
        var (libelle, description, lien, categorieId,note) =
            (Request.Form["Libelle"].ToString(), Request.Form["Description"], Request.Form["Lien"], Request.Form["Categorie"], Request.Form["Note"].ToString());
        var projet = new Projet()
        {
            Name = libelle,
            Description = description,
            DateAjout = DateTime.Now,
            Etat = 0,
            Lien = lien,
            Note = note,
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
        var projet = await _context.Projets.FindAsync(id);
        ViewBag.Categorie =  await _context.Categories.FindAsync(projet.CategorieId);
        var features = await (from p in _context.Projets
                        join pf in _context.ProjetFeatures on p.Id equals pf.ProjetId
                        join f in _context.Features on pf.FeatureId equals f.Id
                        where p.Etat != 1 && p.Id == id && f.Etat != 1
                        select f).Distinct().ToListAsync();
        ViewBag.Features = features;
        LoadSetting();
        ViewBag.Ressources = await _context.Ressources.Where(r => r.Etat == 0 && r.ProjetId == id).ToListAsync();
        return View("../Admin/Projet/DetailsProjet", projet );
    }


    [HttpGet]
    [Route("admin/projets/update/{id}")]
    public async Task<IActionResult> UpdateProjet(int id, string notification = null)
    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        var projet = await _context.Projets.FindAsync(id);
        ViewBag.Categorie =  await _context.Categories.Where(c => c.Etat == 0).ToListAsync();
        var features = await (from p in _context.Projets
                              join pf in _context.ProjetFeatures on p.Id equals pf.ProjetId
                              join f in _context.Features on pf.FeatureId equals f.Id
                              where p.Etat != 1 && f.Etat != 1 && p.Id == id
                              select f).Distinct().ToListAsync();
        ViewBag.AllFeatures = await _context.Features.Where(d => d.Etat == 0).ToListAsync();
        var listFeature = new List<int>();
        LoadSetting();
        features.ForEach(x => listFeature.Add(x.Id));
        ViewBag.ListFeatures = listFeature;
        return View("../Admin/Projet/UpdateProjet", projet );
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/projets/update")]
    public async Task<IActionResult> UpdateProjetMethode()
    {
        int id = int.Parse(Request.Form["Id"].ToString());
        var projet = await _context.Projets.FindAsync(id);
        var (name, description, lien, categorieId, note, features) = (Request.Form["Name"].ToString(), Request.Form["Description"], Request.Form["Lien"].ToString(), int.Parse(Request.Form["Categorie"].ToString()), Request.Form["Note"].ToString(), Request.Form["Features[]"]);
        string temp = null;
        int modiffe = 0;
       
        if(_context.Projets.Any(p => p.Name == name && p.Id != id))
        {
            return RedirectToAction(nameof(UpdateProjet), routeValues: new
            {
                id,
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Description = "Il existe deja un projet avec ce nom",
                    Libelle = "Erreur",
                    Type = "danger"
                })
            });
        }

        if(projet.Name != name)
        {
            projet.Name = name;
            modiffe += 1;
        }        
        if(projet.Description!= description)
        {
            projet.Description = description;
            modiffe += 1;
        }        
        if(projet.Lien != lien)
        {
            projet.Lien= lien;
            modiffe += 1;
        }
        if(projet.CategorieId != categorieId )
        {
            projet.CategorieId = categorieId;
            modiffe += 1;
        }
        if(projet.Note != note)
        {
            projet.Note = note;
            modiffe += 1;
        }
        var projetFeature = await
           (from f in _context.ProjetFeatures
            where f.ProjetId == id 
            select f)
           .Distinct()
           .ToListAsync();

        int tempFeature = 0;
        var tempListe = new List<int>();
        var realPorjetFeature = projetFeature.ToList();
        projetFeature.ForEach(f =>
        {
            if (!features.Contains(f.FeatureId.ToString()))
            {
                tempFeature++;
                _context.ProjetFeatures.Remove(f);
                 _context.SaveChanges();
                temp = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Mise à jour de projet",
                    Description = $"Le projet {projet.Name} a bien ete modifie."
                });
            }
        });

        features.ToList().ForEach(x =>
        {
            if(!projetFeature.Any(y => y.FeatureId == int.Parse(x))){
                _context.Add(new ProjetFeature()
                {
                    FeatureId = int.Parse(x),
                    ProjetId = id
                });
                 _context.SaveChanges();
                temp = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Mise à jour de projet",
                    Description = $"Le projet {projet.Name} a bien ete modifie."
                });
            }
        });
        
       

        if (modiffe > 0)
        {
            _context.Projets.Update(projet);
            await _context.SaveChangesAsync();
            temp = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Mise à jour de projet",
                Description = $"Le projet {projet.Name} a bien ete modifie."
            });
        }


        return RedirectToAction(nameof(UpdateProjet), routeValues: new
        {
            id,
            notification = temp
        });
    }


    [HttpGet]
    [Route("admin/projets/validate/{id}")]
    public async Task<ActionResult> ValidateProjet(int id)
    {
        var projet = await _context.Projets.FindAsync(id);
        projet.Etat = 2;
        _context.Update(projet);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Projets), routeValues: new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Description = $"Le projet {projet.Name} a bien ete valide",
                Libelle = "validation de projet"
            })
        });
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
        LoadSetting();
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
        LoadSetting();
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
        LoadSetting();
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

    #region Fonctionnalités

    [Route("admin/features")]
    [HttpGet]
    public async Task<IActionResult> Features(string notification = null)

    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        ViewBag.Projets = await _context.Projets.Where(p => p.Etat != 1).ToListAsync();
        ViewBag.FeatureProjet = await _context.ProjetFeatures.ToListAsync();
        return View("../Admin/Features/AllFeature",await _context.Features.Where(f => f.Etat == 0).ToListAsync());
    }


    [Route("admin/features/add")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<ActionResult> AddFeature()
    {

        var (libelle, projetId) = (Request.Form["Libelle"].ToString(), int.Parse(Request.Form["Projet"].ToString()));

        if(_context.Features.Any(x => x.Libelle == libelle && x.Etat == 0))
        {
            Feature feature = _context.Features.Where(x => x.Libelle == libelle).First();
            if (_context.ProjetFeatures.Any(pf => pf.FeatureId == feature.Id && pf.ProjetId == projetId))
                return RedirectToAction(nameof(Features), new
                {
                    notification = JsonConvert.SerializeObject(new MyNotification()
                    {
                        Libelle = "Erreur",
                        Description = $"La fonctionnalite existe deja pour ce projet.",
                        Type = "danger"
                    })
                });
            else
            {
                var projetFeateaur = _context.Add(new ProjetFeature()
                {
                    FeatureId = feature.Id,
                    ProjetId = projetId,
                });
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            var feature =  _context.Add(new Feature()
            {
                Libelle = libelle,
                DateAjout = DateTime.Now,
                Etat = 0,
            });
            await _context.SaveChangesAsync();
            var projetFeateaur = _context.Add(new ProjetFeature()
            {
                FeatureId = feature.Entity.Id,
                ProjetId = projetId,             
            });
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Features), new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Ajout de fonctionnalite",
                Description = $"La fonctionnalite {libelle} a ete ajoute avec succes"
            })
        });
    }

    [Route("admin/features/delete/{id}")]
    [HttpGet]
    public async Task<ActionResult> DeleteFeature(int id)
    {
        if(_context.Features.Any(f => f.Id == id))
        {
            var feature = _context.Features.Find(id);
            feature.Etat = 1;
            var projetFeature = await _context.ProjetFeatures.Where(pf => pf.FeatureId == id).ToListAsync();
            //projetFeature.ForEach(pf => pf.Etat = 1);
            _context.UpdateRange(projetFeature);
            _context.Update(feature);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Features), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Suppression de fonctionnalite",
                    Description = $"La fonctionnalite {feature.Libelle} a bien ete supprime."
                })
            });
        }

        return RedirectToAction(nameof(Features), new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Erreur",
                Description = $"La Fonctionnalite n'existe pas.",
                Type = "danger"
            })
        });

    }

    [Route("admin/features/update")]
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<ActionResult> UpdateFeature()
    {

        var (libelle,id) = (Request.Form["Libelle"].ToString(), int.Parse(Request.Form["Id"].ToString()));
        var feature = _context.Features.Find(id);
        if(feature.Libelle != libelle)
        {
            feature.Libelle = libelle;
            _context.Update(feature);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Features), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Modification de fonctionnalite",
                    Description = $"La fonctionnaltie a ete mise a jour.",
                })
            });
        }

        return RedirectToAction(nameof(Features));
    }


    #endregion

    #region Ressources

    [HttpGet]
    [Route("admin/ressources")]
    public async Task<IActionResult> Ressources(string notification = null)

    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        var ressources = await (from p in _context.Projets
                                join r in _context.Ressources on p.Id equals r.ProjetId 
                                join c in _context.Categories on p.CategorieId equals c.Id
                                where p.Etat != 1 && r.Etat == 0 && c.Etat == 0
                                select new
                                {
                                    p.Name,
                                    r,
                                    c.Libelle
                                }
                       ).ToListAsync();
        ViewBag.Ressources = ressources;
        ViewBag.Projets = await _context.Projets.Where(p => p.Etat != 1).ToListAsync();
        return View("../Admin/Ressource/AllRessource", await _context.Ressources.Where(r => r.Etat == 0).ToListAsync());
    }

    [Route("admin/ressources/add")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> AddRessoruce()
    {

        var (libelle, projetId, lien, type) = (Request.Form["Libelle"].ToString(), int.Parse(Request.Form["Projet"].ToString()), Request.Form["Lien"].ToString(), Request.Form["Type"].ToString());

        if(!_context.Ressources.Any(r => r.Libelle == libelle))
        {
            var ressource = new Ressource()
            {
                Libelle = libelle,
                DateAjout = DateTime.Now,
                Etat = 0,
                Lien = lien,
                ProjetId = projetId,
                Type = type,
            };
            await _context.AddAsync(ressource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Ressources), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Ajout de ressource",
                    Description = $"La ressource a ete ajoute avec succes"
                })
            });
        }
        else
            if(_context.Ressources.Any(r => r.Libelle == libelle))
                return RedirectToAction(nameof(Ressources), new
                {
                    notification = JsonConvert.SerializeObject(new MyNotification()
                    {
                        Libelle = "Erreur",
                        Description = $"La ressource existe deja.",
                        Type = "danger"
                    })
                });

        return RedirectToAction(nameof(Ressources));



    }
    
    [Route("admin/ressources/update")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> UpdateRessource()
    {

        var (libelle, projetId, lien, type, id) = (Request.Form["Libelle"].ToString(), int.Parse(Request.Form["Projet"].ToString()), Request.Form["Lien"].ToString(), Request.Form["Type"].ToString(), int.Parse(Request.Form["Id"].ToString()));

        if(!_context.Ressources.Any(r => r.Libelle == libelle && r.Id != id))
        {
            var ressource = _context.Ressources.Find(id);
            int modiffe = 0;
            if(ressource.Libelle != libelle)
            {
                ressource.Libelle = libelle;
                modiffe++;
            }

            if(ressource.ProjetId != projetId)
            {
                ressource.ProjetId = projetId;
                modiffe++;
            }

            if(ressource.Lien != lien)
            {
                ressource.Lien= lien;
                modiffe++;
            }

            if(ressource.Type != type)
            {
                ressource.Type = type;
                modiffe++;
            }

            if (modiffe > 0)
            {
                _context.Update(ressource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Ressources), new
                {
                    notification = JsonConvert.SerializeObject(new MyNotification()
                    {
                        Libelle = "Modification de ressource",
                        Description = $"La ressource a ete modifie avec succes"
                    })
                });
            }
            else
                return RedirectToAction(nameof(Ressources));
        }
        return RedirectToAction(nameof(Ressources), new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Erreur",
                Description = $"La ressource existe deja.",
                Type = "danger"
            })
        });





    }


    [HttpGet]
    [Route("admin/ressources/delete/{id}")]
    public async Task<ActionResult> RessourceDelete(int id)
    {

        var ressource = _context.Ressources.Find(id);
        ressource.Etat = 1;
        _context.Update(ressource);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Ressources), new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Suppression de ressource",
                Description = $"La ressource {ressource.Libelle} a bien ete supprime."
            })
        });
    }


    #endregion

    #region Settings


    [HttpGet]
    [Route("admin/settings")]
    public async Task<IActionResult> Settings(string notification = null)

    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        LoadSetting();
        return View("../Admin/Settings/AllSettings", await _context.Settings.Where(s => s.Etat == 0).ToListAsync());
    }


    [HttpGet]
    [Route("admin/settings/delete/{id}")]
    public async Task<ActionResult> DeleteSettings(int id)
    {
        if(_context.Settings.Any(s => s.Id == id))
        {
            var settings = _context.Settings.Find(id);
            settings.Etat = 1;
            _context.Update(settings);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Settings), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Suppression de parametre",
                    Description = $"Le parametre {settings.Name} a bien ete supprime.",
                })
            });
        }

        return RedirectToAction(nameof(Settings), new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Erreur",
                Description = $"Le parametre n'existe pas.",
                Type = "danger"
            })
        });
    }
    

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/settings/add")]
    public async Task<ActionResult> AddSettings()
    {
        var (name,valeur) = (Request.Form["Name"].ToString(),Request.Form["Value"].ToString());
        if(_context.Settings.Any(s => s.Name == name))
            return RedirectToAction(nameof(Settings), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Erreur",
                    Description = $"Le parametre {name} existe deja.",
                    Type = "danger"
                })
            });
        else
        {
            _context.Add(new Settings()
            {
                Name = name,
                Value = valeur,
                Etat = 0
            });
           await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Settings), new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Ajout de parametre",
                Description = $"Le parametre {name} a bien ete ajoute."
            })
        });
    }

    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/settings/update")]
    public async Task<ActionResult> UpdateSettings()
    {
        var (name,valeur,id) = (Request.Form["Name"].ToString(), Request.Form["Value"].ToString(), int.Parse(Request.Form["Id"].ToString()));
        if(_context.Settings.Any(s => s.Name == name && s.Id == id))
            return RedirectToAction(nameof(Settings), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Erreur",
                    Description = $"Le parametre {name} existe deja.",
                    Type = "danger"
                })
            });
        else
        {
            int modiffe = 0;
            var settings = _context.Settings.Find(id);
            if(settings.Name != name)
            {
                settings.Name = name;
                modiffe++;
            }
            if(settings.Value != valeur)
            {
                settings.Value = valeur;
                modiffe++;
            }
            if(modiffe > 0)
            {
                _context.Update(modiffe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Settings),routeValues: new
                {
                    notification = JsonConvert.SerializeObject(new MyNotification()
                    {
                        Libelle = "Mise a jour de parametre",
                        Description = $"Le parametre {name} a bien ete modifie."
                    })
                });
            }
            return RedirectToAction(nameof(Settings));
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin/settings/save")]
    public async Task<ActionResult> SaveSettings()
    {
        var (couleur, navColor, navWidht, userInfo, topPar) = (Request.Form["Color"].ToString(), Request.Form["NavColor"].ToString(), Request.Form["NavWidth"].ToString(), Request.Form["UserInfo"].ToString(), Request.Form["TopBar"].ToString());
        var temp = new List<string>()
            {
                "Couleur",
                "NavColor",
                "NavWidth",
                "TopBar",
                "UserInfo",

            };
        var settings = _context.Settings.Where(s => temp.Contains(s.Name)).ToList();
        var result = settings;
        settings.ForEach(s =>
        {
            if (s.Name == "Couleur" && couleur != "")
                result.Find(d => d.Id == s.Id).Value = couleur;
            if (s.Name == "NavColor" && navColor != "")
                result.Find(d => d.Id == s.Id).Value = navColor;
            if (s.Name == "NavWidth" && navWidht != "")
                result.Find(d => d.Id == s.Id).Value = navWidht;
            if (s.Name == "TopBar" && topPar != "")
                result.Find(d => d.Id == s.Id).Value = topPar;
            if (s.Name == "UserInfo" && userInfo != "")
                result.Find(d => d.Id == s.Id).Value = userInfo;

        });
        _context.UpdateRange(result);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new
        {
            notification = JsonConvert.SerializeObject(new MyNotification()
            {
                Libelle = "Mise a jour",
                Description = $"Le parametrage a bien ete mise a jour."
            })
        });
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


    #endregion

    #region Skills

    [HttpGet]
    [Route("admin/skills")]
    public async Task<IActionResult> Skills(string notification = null)

    {
        if (notification != null)
        {
            var temp = JsonConvert.DeserializeObject<MyNotification>(notification);
            ViewBag.Notif = temp;
        }
        LoadSetting();
        return View("../Admin/Skills/AllSkills", await _context.Skills.Where(s => s.Etat == 0).ToListAsync());
    }
    
    [HttpPost]
    [Route("admin/skills/update")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> UpdateSkills()
    {

        var (name, completion, id) = (Request.Form["Name"].ToString(), int.Parse(Request.Form["Completion"].ToString()), int.Parse(Request.Form["Id"].ToString()));

        var skill = _context.Skills.Find(id);
        if(_context.Skills.Any(x => x.Name == name && x.Id != id))
            return RedirectToAction(nameof(Skills), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Erreur",
                    Description = $"La competence {name} existe deja.",
                    Type = "danger"
                })
            });
        int modiffe = 0;
        if (skill.Name != name)
        {
            skill.Name = name;
            modiffe++;
        }
        if (skill.Completion != completion)
        {
            skill.Completion = completion;
            modiffe++;
        }
        if(modiffe > 0)
        {
            _context.Update(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Skills), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Mise a jour de competence",
                    Description = $"Les modification ont ete effectue avec succes"
                })
            });

        }



        return RedirectToAction(nameof(Skills));
    }


    [HttpPost]
    [Route("admin/skills/add")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> AddSkills()
    {

        var (name, completion) = (Request.Form["Name"].ToString(), int.Parse(Request.Form["Completion"].ToString()));

        if(_context.Skills.Any(x => x.Name == name))
            return RedirectToAction(nameof(Skills), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Erreur",
                    Description = $"La competence {name} existe deja.",
                    Type = "danger"
                })
            });

            _context.Add(new Skill()
            {
                Completion = completion,
                Etat = 0,
                Name = name,
                SkillDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Skills), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Ajout de competence",
                    Description = $"La competence {name} a ete ajoute avec succes"
                })
            });

    }

    [HttpGet]
    [Route("admin/skills/delete/{id}")]
    public async Task<ActionResult> DeleteSkill(int id)
    {

        if(_context.Skills.Any(x => x.Id == id))
        {
            var skill = _context.Skills.Find(id);
            skill.Etat = 1;
            _context.Update(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Skills), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Suppression de competence",
                    Description = $"La competence {skill.Name} a ete supprime.",
                })
            });
        }
        else
            return RedirectToAction(nameof(Skills), new
            {
                notification = JsonConvert.SerializeObject(new MyNotification()
                {
                    Libelle = "Erreur",
                    Description = $"La competence n'existe pas.",
                    Type = "danger"
                })
            });
    }





    #endregion

}
