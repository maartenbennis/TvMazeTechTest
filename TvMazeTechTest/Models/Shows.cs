using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TvMazeTechTest.Models
{
    public class Shows
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Language { get; set; }
        public string? Summary { get; set; }        
        public DateTime? Premiered { get ; set; }        

        [NotMapped]
        public List<String> Genres { get; set; }

        public string? Genrestring
        {
            get { return String.Join(",", Genres); }
            set { Genres = value.Split(',').ToList(); }
        }

    }

}

