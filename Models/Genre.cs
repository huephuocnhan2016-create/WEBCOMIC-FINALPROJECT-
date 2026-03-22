using System;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Manga> Mangas { get; set; }
    }
}
