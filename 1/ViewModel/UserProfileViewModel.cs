using InkCanvas.Models;

namespace _1.ViewModel
{
    // For toggling the "following" button label
    public class UserProfileViewModel
    {
        public User User { get; set; }
        public bool IsFollowing { get; set; }
    }
}
