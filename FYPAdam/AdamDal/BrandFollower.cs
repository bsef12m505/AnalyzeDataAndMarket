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
    
    public partial class BrandFollower
    {
        public int Id { get; set; }
        public int BrandId { get; set; }
        public int FollowersCount { get; set; }
        public System.DateTime Date { get; set; }
       
    
        public virtual Brand Brand { get; set; }
    }
}
