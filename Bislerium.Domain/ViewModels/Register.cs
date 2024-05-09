using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Week21.Domain
{
    public class Register
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm password")]
        [ComplexPassword(ErrorMessage = "Invalid password.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string? Role { get; set; } // Property to specify the role of the user
    }

    public class ComplexPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || !(value is string))
            {
                return false;
            }

            string password = (string)value;

            // Define individual regex patterns and error messages
            var patterns = new[]
            {
                (Pattern: @"[a-z]", ErrorMessage: "Password must have at least one lowercase letter."),
                (Pattern: @"[A-Z]", ErrorMessage: "Password must have at least one uppercase letter."),
                (Pattern: @"\d", ErrorMessage: "Password must have at least one digit."),
                (Pattern: @"[@$!%*?&]", ErrorMessage: "Password must have at least one special character.")
            };

            foreach (var (pattern, errorMessage) in patterns)
            {
                if (!Regex.IsMatch(password, pattern))
                {
                    ErrorMessage = errorMessage;
                    return false;
                }
            }

            // Additional length check
            if (password.Length < 8 || password.Length > 15)
            {
                ErrorMessage = "Password must be between 8 and 15 characters long.";
                return false;
            }

            return true;
        }
    }
}
