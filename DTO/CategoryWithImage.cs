namespace FullCartApi.DTO
{
    public class CategoryWithImage
    {
        public string Name { get; set; } = null!;

        public IFormFile ImageFile { get; set; }
    }

}
