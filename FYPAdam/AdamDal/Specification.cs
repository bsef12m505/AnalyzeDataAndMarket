//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdamDal
{
    using System;
    using System.Collections.Generic;
    using System.Web.Script.Serialization;
    
    public partial class Specification
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
}
