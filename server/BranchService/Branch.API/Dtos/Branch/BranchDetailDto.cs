using System.ComponentModel.DataAnnotations;

namespace Branch.API.Dtos.Branch
{
    public class BranchDetailDto
    {
        public string Name { get; set; }

      
        public string Address { get; set; }

        
        public string Phone { get; set; }

       
        public string? Email { get; set; }

        public string Type { get; set; }

        public bool IsActive { get; set; } = true;

        public string? IpAddress { get; set; }
    }
}
