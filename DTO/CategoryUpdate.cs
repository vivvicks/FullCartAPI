namespace FullCartApi.DTO
{
    public class CategoryUpdate
    {
        public string Name { get; set; } = null!;

        public IFormFile? NewImageFile { get; set; }
    }

}
