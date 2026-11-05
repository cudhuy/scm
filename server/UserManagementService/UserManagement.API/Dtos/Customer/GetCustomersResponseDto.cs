namespace UserManagement.API.Dtos.Customer
{
    public class GetCustomersResponseDto
    {
        public IEnumerable<CustomerDto> Items { get; set; }
        public int TotalItems { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
