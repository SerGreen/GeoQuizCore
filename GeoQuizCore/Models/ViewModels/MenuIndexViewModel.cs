using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GeoQuizCore.Models
{
    public class MenuIndexViewModel : PageModel
    {
        public GameSettings Settings { get; set; }
        public MenuIndexModalViewModel ModalViewModel { get; set; }
    }
}