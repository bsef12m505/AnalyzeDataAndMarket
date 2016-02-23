using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdamDal
{
    public class Class1
    {
        AdamDatabaseEntities2 ed = new AdamDatabaseEntities2();
        public IEnumerable<Admin> adminname()
        {
            IEnumerable < Admin > p= ed.Admins.Select(x => x);
            
            return p;
        }
    }
}
