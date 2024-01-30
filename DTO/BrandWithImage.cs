namespace FullCartApi.DTO
{
    public class BrandWithImage
    {
        public string Name { get; set; } = null!;

        public IFormFile ImageFile { get; set; }
    }

}
