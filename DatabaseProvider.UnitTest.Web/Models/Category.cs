namespace DatabaseProvider.UnitTest.Web.Models
{
    public partial class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}