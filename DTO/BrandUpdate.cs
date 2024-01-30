namespace FullCartApi.DTO
{
    public class BrandUpdate
    {
        public string Name { get; set; } = null!;

        public IFormFile? NewImageFile { get; set; }
    }

}
