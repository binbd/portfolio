using Microsoft.EntityFrameworkCore;
namespace Profiolio_MVC.Models
{
     [Index(nameof(ClientId))]
    public class ViewerCounting
    {
        public int Id { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public bool IsCurrentViewing { get; set; } = false;

        public DateTime FirstViewing { get; set; }
        public DateTime LastViewing { get; set; }
        public ICollection<ViewerLoggin> ViewerLoggins { get; } = new List<ViewerLoggin>(); 
        //public ViewerLoggin? viewerLoggin {get;set;}
        
    }

    public class ViewerLoggin {
        public int Id { get; set; }
        public int ViewerCountingId {get;set;}
        public ViewerCounting viewerCounting {get;set;} = null!;
        public string ClientIp { get; set; } = string.Empty;
        public DateTime LogginTime { get; set; }
        public string Note1 { get; set; } = string.Empty;
        public string Note2 { get; set; } = string.Empty;
        public string Note3 { get; set; } = string.Empty;
        public string userId {get; set; } = string.Empty;
                     
    }
}

