using Microsoft.EntityFrameworkCore;
using Payment.DataAccessLayer.Abstract;
using Payment.DataAccessLayer.Concrete;
using Payment.DataAccessLayer.Repositories;
using Payment.DtoLayer.Dtos.AddressDtos;
using Payment.EntityLayer.Concrete;

namespace Payment.DataAccessLayer.EntityFramework
{
    public class EfAddressDal : GenericRepository<Address>, IAddressDal
    {
        public EfAddressDal(Context context) : base(context)
        {
        }

        public List<AddressWithUsernameDto> AddressWithUsername()
        {
            var context = new Context(); 
            var values = context.Users
                .Include(u => u.Addresses) 
                .SelectMany(u => u.Addresses, (user, address) => new AddressWithUsernameDto
                {
                    UserName = $"{user.Name} {user.Surname}",
                    AddressID = address.AddressID,
                    AddressLine = address.AddressLine,
                    City = address.City,
                    District = address.District,
                    CreateTime = address.CreateTime,
                    UpdateTime = address.UpdateTime,
                    CreateUser = address.CreateUser,
                    UpdateUser = address.UpdateUser
                })
                .ToList();

            return values;
        }

    }
}
