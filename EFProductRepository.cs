namespace InventoryAppCloudDb
{
    public class EFProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public EFProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Product> GetAll()
        {
            return _context.Products
                .OrderBy(p => p.Id)
                .ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products
                .FirstOrDefault(p => p.Id == id);
        }

        public List<Product> GetByCategory(string category)
        {
            return _context.Products
                .Where(p => p.Category == category)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public int Insert(Product p)
        {
            _context.Products.Add(p);
            _context.SaveChanges();
            return p.Id;
        }

        public bool Update(Product p)
        {
            var existing = _context.Products.Find(p.Id);
            if (existing == null) return false;

            existing.Name = p.Name;
            existing.Price = p.Price;
            existing.Stock = p.Stock;
            existing.Category = p.Category;

            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var p = _context.Products.Find(id);
            if (p == null) return false;

            _context.Products.Remove(p);
            _context.SaveChanges();
            return true;
        }
    }
}