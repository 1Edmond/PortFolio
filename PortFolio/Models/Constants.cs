
namespace PortFolio.Models;
public static class Constants
{

    public static int AnnimationDuration { get => 5000; }

    public static Dictionary<string, string> Types { get => new () { { "Image" , "Image" }, { "Video","Vidéo" } }; }


    public static List<string> AuthorizedRoute { get => new()
    {
        "/",
        "",
        "/admin/logout",
        "/Auth/Login",
        "/admin",
        "/admin/projets",
        "/admin/projets/add",
        "/admin/projets/details",
        "/admin/projets/delete",
        "/admin/projets/update",
        "/admin/projets/update",
        "/admin/projets/validate",
        "/admin/categories",
        "/admin/categories/add",
        "/admin/categories/details",
        "/admin/categories/delete",
        "/admin/categories/update",
        "/admin/features",
        "/admin/features/add",
        "/admin/features/delete",
        "/admin/features/update",
        "/admin/ressources",
        "/admin/ressources/add",
        "/admin/ressources/delete",
        "/admin/ressources/update",
        "/admin/settings",
        "/admin/settings/add",
        "/admin/settings/update",
        "/admin/settings/delete",
        "/admin/settings/save",
        "/admin/skills",
        "/admin/skills/update",
        "/admin/skills/add",
        "/admin/skills/delete",

    };}

    public static List<string> Extensions { get 
            => new()
            {
                "css",
                "js",
                "jpg",
                "jpeg",
                "png",
                "ico",
                "svg",
                "gif",
                "woff2",
                "woff",
                "tff",
                "json",
            };
    }

    public static bool ContainUrl(this List<string> list, string val)
    {

        if (list.Any(x => x == val))
            return true;
        else
            if (list.Contains(val[..(val.LastIndexOf("/"))]))
            {
                string rep = val[(val.LastIndexOf("/") + 1)..];
                if(int.TryParse(rep, out int _))
                    return true;
            }
        return false;
    }



    public static bool ContainExtension(this string text)
    {
        if (Extensions.Any(x => x == text[(text.LastIndexOf(".") + 1)..]))
            return true;
        return false;
    }


}
