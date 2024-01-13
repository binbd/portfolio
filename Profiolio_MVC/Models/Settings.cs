using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Profiolio_MVC.Models
{
        [Index(nameof(Key))]
        public class SettingsNumber
        {
            public int Id { get; set; }
            [Required(ErrorMessage="Please enter a description")]
            public string Key { get; set; } = string.Empty;
            public int Value { get; set; } 
        }

        public class ViewingStatistic{
            public int TotalView {get; set;}
            public int TotalViewOnline {get;set;}
        }
}

