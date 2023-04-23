using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RedMango_API.Models.Dto
{
    public class OrderHeaderCreateDto
    {
        [Required]
        public string PickupName { get; set; }

        [Required]
        public string PickupPhoneNumber { get; set; }

        [Required]
        public string PickupEmail { get; set; }

        public string ApplicationUserId { get; set; }

        public double OrderTotal { get; set; }

        public string StripePaymentId { get; set; }
        public string Status { get; set; }
        public int TotalItems { get; set; }

        public IEnumerable<OrderDetailCreateDto> OrderDetailsDto { get; set; }
    }
}