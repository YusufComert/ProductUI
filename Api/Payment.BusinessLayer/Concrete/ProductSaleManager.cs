using Payment.BusinessLayer.Abstract;
using Payment.DataAccessLayer.Abstract;
using Payment.DataAccessLayer.EntityFramework;
using Payment.EntityLayer.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.BusinessLayer.Concrete
{
    public class ProductSaleManager : IProductSaleService
    {
        private readonly IProductSaleDal _productSaleManagerList;

        public ProductSaleManager(IProductSaleDal productSale)
        {
            _productSaleManagerList = productSale;
        }

        public void TDelete(ProductSale t)
        {
            _productSaleManagerList.Delete(t);
        }

        public ProductSale TGetByID(int id)
        {
          return  _productSaleManagerList.GetByID(id);
        }

        public List<ProductSale> TGetList()
        {
          return  _productSaleManagerList.GetList();
        }

        public void TInsert(ProductSale t)
        {
            _productSaleManagerList.Insert(t);
        }

        public void TUpdate(ProductSale t)
        {
            _productSaleManagerList.Update(t);
        }

    }
}
