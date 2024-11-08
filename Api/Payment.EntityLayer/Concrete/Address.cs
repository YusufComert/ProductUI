using System.ComponentModel.DataAnnotations;

namespace Payment.EntityLayer.Concrete;

public class Address
{
    [Key]
    public int AddressID { get; set; }
    public string AddressLine { get; set; }
    public string City { get; set; }
    public string District { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public string CreateUser { get; set; }
    public string UpdateUser { get; set; }
    public int AppUserId { get; set; }
    
    public AppUser appUser { get; set; }
}
