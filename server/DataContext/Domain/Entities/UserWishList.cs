using Domain.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class UserWishList : BaseEntity
    {
        [ForeignKey("User")]
        public Guid? CustomerId { get; set; }
        public User User { get; set; }

        [ForeignKey("Product")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
