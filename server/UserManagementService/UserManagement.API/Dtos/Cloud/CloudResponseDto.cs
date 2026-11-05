namespace UserManagement.API.Dtos.Cloud
{
    public class CloudFileDto
    {
        public string Url { get; set; }
        public string PublicId { get; set; }
    }

    public class CloudResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<CloudFileDto> Files { get; set; } = new List<CloudFileDto>();
    }
}
