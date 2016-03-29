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

        //Getting Brand Id
        public int GetBrandIdToSaveProduct(Product product, int cid)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            int bId = 0;

            Brand b = new Brand();
            List<Brand> list = ed1.Brands.ToList();
            foreach (var l in list)
            {
                if (product.Title.ToLower().Contains(l.Name.ToLower()) && l.CategoryId == cid)
                {
                    bId = l.Id;
                    break;

                }
            }

            return bId;
        }

        ////Get Caretgory Id of Specific Product
        public int GetCategoryIdToSaveProduct(Product product, string type)
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
                else if (l.Name.Equals("MobilePhone") && type.Equals("Mobile"))
                {
                    cId = l.Id;
                    break;
                }
            }
            return cId;
        }

        //new
        public dynamic GetTopProductsAgainstBrandAndCategory(string catName, string bName)
        {
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            List<Product> list = new List<Product>();
            ed.Configuration.ProxyCreationEnabled = false;
            int catId = GetCategoryId(catName);
            int bId = GetBrandId(bName);
            var strlist = ed.Products.Where(y => y.CategoryId.Equals(catId) && y.BrandId.Equals(bId)).Select(m => m).Distinct().OrderByDescending(x => x.Rating).Take(5).ToList();


            return strlist;

        }
        public Product SearchedProduct(string name)
        {
            Product p = new Product();
            try
            {
                AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
                //List<Product> list = new List<Product>();
                ed.Configuration.ProxyCreationEnabled = false;
                p = ed.Products.First(x => x.Title.Equals(name));
                return p;
            }
            catch (Exception)
            {
                return p;
            }
        }
        public List<string> GetAllProductTitles()
        {
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            List<string> nameList = new List<string>();
            ed.Configuration.ProxyCreationEnabled = false;
            nameList = ed.Products.Select(x => x.Title).ToList();
            return nameList;
        }

        public List<FeatureSentiment> GetRefinedFeaturesOfProduct(int pid)
        {

            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            ed.Configuration.ProxyCreationEnabled = false;
            List<FeatureSentiment> features = ed.FeatureSentiments.Where(x => x.PId.Equals(pid)).Select(y => y).ToList();
            return features;
        }
        //new 
        public int GetCategoryId(string catName)
        {
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            ed.Configuration.ProxyCreationEnabled = false;
            Category category = ed.Categories.First(x => x.Name.Equals(catName));
            return category.Id;

        }
        //new 
        public int GetBrandId(string bName)
        {
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            ed.Configuration.ProxyCreationEnabled = false;
            Brand brand = ed.Brands.First(x => x.Name.Equals(bName));
            return brand.Id;

        }

        public List<Product> GetRelatedProducts(string prodName, int prodId)
        {
            string[] token = prodName.Split(' ');
            string series = token[1];
            List<Product> product = new List<Product>();
            try
            {
                AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
                ed.Configuration.ProxyCreationEnabled = false;

                // var temp= ed.Products.Where(x => x.Title.Contains(series)).Select(x=>x.Title).Distinct()).OrderByDescending(x => x.Rating).Take(5).ToList();
                product = ed.Products.Where(x => x.Title.Contains(series) && x.Id != prodId && (!(x.Title.Contains(prodName)))).Select(m => m).Distinct().OrderByDescending(x => x.Rating).Take(5).ToList();

            }
            catch (Exception)

            { };
            return product;
        }
        //new
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
        //new
        public List<Brand> GetAllProductsAgainstBrand(int bId)
        {
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            ed.Configuration.ProxyCreationEnabled = false;
            List<Brand> b = ed.Brands.Include("Products").Where(x => x.Id == bId).ToList();
            return b;
        }

        //Add Product Details  
        public int AddProductDetails(Product product, string type)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2(); ;

            ed1.Configuration.ProxyCreationEnabled = false;
            product.CategoryId = GetCategoryIdToSaveProduct(product, type);
            product.BrandId = GetBrandIdToSaveProduct(product, product.CategoryId);
            ed1.Products.Add(product);
            ed1.SaveChanges();
            return product.Id;
        }

        public dynamic GetAllReviewsAgainstProduct()
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            var products = ed1.Products.Include("ProductReviews").ToList(); ;
            return products;
        }

        public List<Brand> GetAllBrands()
        {
            List<Brand> b = new List<Brand>();
            try
            {
                AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
                ed1.Configuration.ProxyCreationEnabled = false;
                b = ed1.Brands.ToList();
                return b;

            }
            catch (Exception)
            {
                return b;
            }


        }

        public List<Product> CompareProduct(string prodName1, string prodName2)
        {
            Product[] arrList = new Product[2];
            List<Product> prodList = new List<Product>();
            List<Product> tempProdList = new List<Product>();
            AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
            ed.Configuration.ProxyCreationEnabled = false;
            try
            {
                tempProdList = ed.Products.Include("Product_Specification.Specification").Where(x => x.Title.Equals(prodName1) || x.Title.Equals(prodName2)).ToList();
                int count = 0;
                int i = 0;
                foreach (Product p in tempProdList)
                {
                    if (count < 2)
                    {
                        if (tempProdList[i].Title.Equals(prodName1) && arrList[0] == null)
                        {
                            arrList[0] = tempProdList[i];
                            count++;
                        }
                        else if (tempProdList[i].Title.Equals(prodName2) && arrList[1] == null)
                        {

                            arrList[1] = tempProdList[i];
                            count++;

                        }
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }
            }
            catch (Exception)
            {

            }
            prodList.Add(arrList[0]);
            prodList.Add(arrList[1]);
            return prodList;
        }


        public List<Customer_AreaOfInterest> GetAllCustomersInterest()
        {

            List<Customer_AreaOfInterest> cusAreaInterest = new List<Customer_AreaOfInterest>();
            try
            {
                AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
                ed1.Configuration.ProxyCreationEnabled = false;
                cusAreaInterest = ed.Customer_AreaOfInterest.ToList();
                return cusAreaInterest;

            }
            catch (Exception)
            {
                return cusAreaInterest;
            }

        }
        public void AddFeatureSentiment(FeatureSentiment fs)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            ed1.FeatureSentiments.Add(fs);
            ed1.SaveChanges();

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

        public void SaveReview(ProductReview pr)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            ed1.ProductReviews.Add(pr);
            ed1.SaveChanges();

        }

        public List<Specification> GetAllSpecifications()
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            return ed1.Specifications.Select(x => x).ToList();
        }
        //public List<Category> GetAllCategories()
        //{
        //    AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        //    ed.Configuration.ProxyCreationEnabled = false;
        //    //var list = ed.Categories.Select(x => new
        //    //{
        //    //    CategoryName = x.Name,
        //    //    CategoryId = x.Id,
        //    //    CategoryBrands = x.Brands.Select(y =>new 
        //    //    {
        //    //        BrandName=y.Name,
        //    //        BrandImageUrl=y.ImageUrl
        //    //    })
        //    //});
        //    List<Category> list = ed.Categories.Include("Brands").ToList();


        //    return list;
        //}
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

        public List<Product> GetAllSpecificationsAgainstProduct(int pId)
        {

            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            List<Product> productList = ed1.Products.Include("Product_Specification.Specification").Include("ProductReviews").Where(x => x.Id == pId).ToList();

            return productList;
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

        public bool CheckLoginDetails(string eamil, string password)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            try
            {
                Customer c = ed1.Customers.First(x => x.Email.Equals(eamil) && x.Password.Equals(password));
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }

        public bool AddSignUpDetail(string fname, string lname, string email, string password)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            try
            {
                Customer c = new Customer();
                c.FirstName = fname;
                c.LastName = lname;
                c.Email = email;
                c.Password = password;
                c.Location = "Pakistan";
                ed1.Customers.Add(c);
                ed1.SaveChanges();
                return true;
                //Customer c = ed1.Customers.First(x => x.Email.Equals(email) && x.Password.Equals(password) && x.LastName.Equals(lname) && x.FirstName.Equals(fname));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<Brand> GetAllBrandNamesOfLaptops()
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            List<Brand> brands = ed1.Brands.Where(y => y.CategoryId == 1).Select(x => x).ToList();
            return brands;
        }

        public List<Brand> GetAllBrandOfMobiles()
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            List<Brand> brands = ed1.Brands.Where(y => y.CategoryId == 2).Select(x => x).ToList();
            return brands;
        }

        public void UpdateBrand(Brand b)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed.Entry(b).State = System.Data.EntityState.Modified;
            ed.SaveChanges();
        }
        public void UpdateProduct(Product prod)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed.Entry(prod).State = System.Data.EntityState.Modified;
            ed.SaveChanges();
        }

        //Getting Brand Id
        public int GetBrandId(Product product, int cid)
        {
            AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
            ed1.Configuration.ProxyCreationEnabled = false;
            int bId = 0;

            Brand b = new Brand();
            List<Brand> list = ed1.Brands.ToList();
            foreach (var l in list)
            {
                if (product.Title.ToLower().Contains(l.Name.ToLower()) && l.CategoryId == cid)
                {
                    bId = l.Id;
                    break;

                }
            }

            return bId;
        }

        //Get Caretgory Id of Specific Product
        //public int GetCategoryId(Product product,string type)
        //{
        //    AdamDatabaseEntities2 ed1 = new AdamDatabaseEntities2();
        //    ed1.Configuration.ProxyCreationEnabled = false;
        //    int cId = 0; ;
        //    Category c = new Category();
        //    List<Category> list = ed1.Categories.ToList();
        //    foreach (var l in list)
        //    {
        //        if (l.Name.Equals("Laptop") && type.Equals("Laptop"))
        //        {
        //            cId = l.Id;
        //            break;
        //        }
        //        else if (l.Name.Equals("MobilePhones") && type.Equals("Mobile"))
        //        {
        //            cId = l.Id;
        //            break;
        //        }
        //    }
        //    return cId;
        //}

        //Get Specification ID 
        public int GetSpecificationId(string specName)
        {

            Specification spec = ed.Specifications.First(x => x.Name.Equals(specName));
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
