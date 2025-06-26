using System.ComponentModel.DataAnnotations;

namespace NguyenManhDuc.WebApp.Repository.Validation
{
    public class FileExtentionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extention = Path.GetExtension(file.FileName);
                string[] extentions = { "jpg", "png", "jpeg", "webp" };
                bool result = extentions.Any(x => extention.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("Allowed Extentions");
                }
            }
            return ValidationResult.Success;
        }
    }
}
