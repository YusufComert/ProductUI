using Payment.DataAccessLayer.Abstract;
using Payment.DataAccessLayer.Concrete;
using Payment.DataAccessLayer.Repositories;
using Payment.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.DataAccessLayer.EntityFramework
{
    public class EfProductSaleDal : GenericRepository<ProductSale>, IProductSaleDal
    {
        public EfProductSaleDal(Context context) : base(context)
        {
        }
    }
}
