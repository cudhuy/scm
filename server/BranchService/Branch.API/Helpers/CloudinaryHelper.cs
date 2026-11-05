using Branch.API.Dtos.Cloud;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Branch.API.Helpers
{
    public class CloudinaryHelper
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryHelper(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<CloudResponseDto> UploadImage(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return new CloudResponseDto { IsSuccess = false, Message = "Không có file ảnh nào." };

            var response = new CloudResponseDto();
            try
            {
                foreach (var file in files)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = $"{file.FileName}_{Guid.NewGuid()}"
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    response.Files.Add(new CloudFileDto
                    {
                        Url = uploadResult.SecureUrl.ToString(),
                        PublicId = uploadResult.PublicId
                    });
                }

                response.IsSuccess = true;
                response.Message = "Upload thành công.";
                return response;
            }
            catch (Exception ex)
            {
                return new CloudResponseDto
                {
                    IsSuccess = false,
                    Message = $"Lỗi upload ảnh: {ex.Message}"
                };
            }
        }
    }
}
