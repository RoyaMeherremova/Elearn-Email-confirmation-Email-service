using System.ComponentModel.DataAnnotations;

namespace Elearn.ViewModel.Account
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]  
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]  
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password), Compare(nameof(Password))]   //Compare-muqayise ele Pasword propertisi ile eyni dimi
        public string ConfirmPassword { get; set; }
    }
}
