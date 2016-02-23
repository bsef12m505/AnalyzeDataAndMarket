using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;


namespace DTO
{
    public class Admin
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class Customer
    {
        public Customer()
        {
            this.Customer_AreaOfInterest = new HashSet<Customer_AreaOfInterest>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Location { get; set; }

        public virtual ICollection<Customer_AreaOfInterest> Customer_AreaOfInterest { get; set; }
    }
    public class Customer_AreaOfInterest
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int AreaOfInterest { get; set; }

        public virtual Category Category { get; set; }
        public virtual Customer Customer { get; set; }
    }

   

    public class ProductReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Review { get; set; }

        [ScriptIgnore]
        public virtual Product Product { get; set; }
    }
    public class Specification
    {
        public Specification()
        {
            this.Product_Specification = new HashSet<Product_Specification>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        [ScriptIgnore]
        public virtual ICollection<Product_Specification> Product_Specification { get; set; }
    }

   
    public class Item
    {
        public int id { get; set; }
        public string SearchString { get; set; }
        public int SearchCount { get; set; }
    }

    
    public class Product_Specification
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int SpecificationId { get; set; }
        public string Value { get; set; }

        public virtual Specification Specification { get; set; }
        [ScriptIgnore]
        public virtual Product Product { get; set; }
    }
    public class Product
    {
        public Product()
        {
            this.Product_Specification = new HashSet<Product_Specification>();
            this.ProductReviews = new HashSet<ProductReview>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string ImageUrl { get; set; }
        public string Price { get; set; }
        public string ReviewSummary { get; set; }
        public string ReleaseDate { get; set; }
        public string Rating { get; set; }
        public string ProductDescription { get; set; }


        public virtual Category Category { get; set; }
        public virtual ICollection<Product_Specification> Product_Specification { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        
        [ScriptIgnore]
        public virtual Brand Brand { get; set; }
        
    }

    public partial class Brand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        [ScriptIgnore]
        public virtual Category Category { get; set; }
    }

    public class Category
    {
        public Category()
        {
            this.Brands = new HashSet<Brand>();
            this.Customer_AreaOfInterest = new HashSet<Customer_AreaOfInterest>();
            this.Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }
        public virtual ICollection<Customer_AreaOfInterest> Customer_AreaOfInterest { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
