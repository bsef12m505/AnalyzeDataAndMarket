using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamDal
{
   public class DbWrappers
    {
        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();

        //Add Product Details  
        public int AddProductDetails(Product product,string type )
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();;
            
            ed1.Configuration.ProxyCreationEnabled = false;
            product.CategoryId = GetCategoryId(product,type);
            product.BrandId = GetBrandId(product, product.CategoryId);
            ed1.Products.Add(product);
            ed1.SaveChanges();
            return product.Id;
        }


        public List<Product> GetAllProducts()
        {
            //int bId = 0;
            //string bName = "";
            //Brand b = new Brand();
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            List<Product> list = ed1.Products.ToList();
           

            return list;
        }
        public List<Category> GetAllCategories()
        {
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            ed.Configuration.ProxyCreationEnabled = false;
            //var list = ed.Categories.Select(x => new
            //{
            //    CategoryName = x.Name,
            //    CategoryId = x.Id,
            //    CategoryBrands = x.Brands.Select(y =>new 
            //    {
            //        BrandName=y.Name,
            //        BrandImageUrl=y.ImageUrl
            //    })
            //});
            List<Category> list = ed.Categories.Include("Brands").ToList();


            return list;
        }
        public List<Brand> GetAllProductsAgainstBrands()
        {
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            ed.Configuration.ProxyCreationEnabled = false;
            //var list = ed.Categories.Select(x => new
            //{
            //    CategoryName = x.Name,
            //    CategoryId = x.Id,
            //    CategoryBrands = x.Brands.Select(y =>new 
            //    {
            //        BrandName=y.Name,
            //        BrandImageUrl=y.ImageUrl
            //    })
            //});
            List<Brand> list = ed.Brands.Include("Products").ToList();


            return list;
        }

        public List<Product> GetAllSpecificationsAgainstProduct()
        {

            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            List<Product> list = ed1.Products.Include("Product_Specification.Specification").Include("ProductReviews").ToList();


            return list;
        }

        public List<Product> GetTopRatedProducts()
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            List<Product> list = new List<Product>();
            return list;
        }

        public Product GetSpecificProduct(int prodId)
        {
           
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            Product prod = ed.Products.First(x => x.Id == prodId);
            return prod;

            
        }

        public List<Product> GetProductsToDisplay(string brand)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            Brand b = ed1.Brands.First(x => x.Name.ToLower().Equals(brand.ToLower()));
            int brandId = b.Id;
            List<Product> products = ed1.Products.Where(x => x.BrandId == brandId).ToList();
            return products;
            
        }
       public void UpdateProduct(Product prod)
       {
           AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
           ed.Entry(prod).State = System.Data.EntityState.Modified;
           ed.SaveChanges();
       }

        //Getting Brand Id
        public int GetBrandId(Product product,int cid)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            int bId = 0;
           
            Brand b = new Brand();
            List<Brand> list = ed1.Brands.ToList();
            foreach (var l in list)
            {
                if (product.Title.ToLower().Contains(l.Name.ToLower()) && l.CategoryId==cid)
                {
                    bId = l.Id;
                    break;

                }
            }

            return bId;
        }

        //Get Caretgory Id of Specific Product
        public int GetCategoryId(Product product,string type)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            int cId = 0; ;
            Category c = new Category();
            List<Category> list = ed1.Categories.ToList();
            foreach (var l in list)
            {
                if (l.Name.Equals("Laptop") && type.Equals("Laptop"))
                {
                    cId = l.Id;
                    break;
                }
                else if (l.Name.Equals("MobilePhones") && type.Equals("Mobile"))
                {
                    cId = l.Id;
                    break;
                }
            }
            return cId;
        }

        //Get Specification ID 
        public int GetSpecificationId(string specName)
        {
           
           Specification spec = ed.Specifications.First(x=>x.Name.Equals(specName));
           return spec.Id;
            
        }

        public void AddProductReviews(ProductReview review)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.ProductReviews.Add(review);
            ed1.SaveChanges();
        }

        //Getting Product Id
        public int GetProductId(Product product)
        {
            Product p = new Product();

            p = ed.Products.First(x => x.Title.Equals(product.Title));

            return p.Id;
        }

        //Saving the Specification of Products
        public void SaveSpecification(Product_Specification ps)
        {
            ed.Product_Specification.Add(ps);
            ed.SaveChanges();
        }
    }
}
