using Microsoft.AspNetCore.Identity;

namespace WEBCOMIC_FINALPROJECT_.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int RewardPoints { get; set; }
        public bool IsVip { get; set; }
        public DateTime? VipExpiryDate { get; set; }

        public virtual ICollection<Manga> AuthoredMangas { get; set; }
    }
}
