namespace GlobalLogisticsManagementUI.Tests
{
    public class FileValidationTests
    {
        private static bool IsValidPdfFile(string fileName, string contentType)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            bool validExtension = fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase);
            bool validContentType = contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);

            return validExtension || validContentType;
        }

        [Fact]
        public void ValidatePdfUpload_WithPdfFile_ReturnsTrue()
        {
            string fileName = "contract_agreement.pdf";
            string contentType = "application/pdf";

            bool result = IsValidPdfFile(fileName, contentType);

            Assert.True(result);
        }

        [Fact]
        public void ValidatePdfUpload_WithExeFile_ReturnsFalse()
        {
            string fileName = "malware.exe";
            string contentType = "application/octet-stream";

            bool result = IsValidPdfFile(fileName, contentType);

            Assert.False(result);
        }

        [Fact]
        public void ValidatePdfUpload_WithDocxFile_ReturnsFalse()
        {
            string fileName = "document.docx";
            string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            bool result = IsValidPdfFile(fileName, contentType);

            Assert.False(result);
        }

        [Fact]
        public void ValidatePdfUpload_WithTxtFile_ReturnsFalse()
        {
            string fileName = "readme.txt";
            string contentType = "text/plain";

            bool result = IsValidPdfFile(fileName, contentType);

            Assert.False(result);
        }

        [Fact]
        public void ValidatePdfUpload_WithUpperCasePdf_ReturnsTrue()
        {
            string fileName = "CONTRACT.PDF";
            string contentType = "application/pdf";

            bool result = IsValidPdfFile(fileName, contentType);

            Assert.True(result);
        }

        [Fact]
        public void ValidatePdfUpload_WithEmptyFileName_ReturnsFalse()
        {
            string fileName = "";
            string contentType = "application/pdf";

            bool result = IsValidPdfFile(fileName, contentType);

            Assert.False(result);
        }

        [Fact]
        public void ValidatePdfUpload_WithJpgFile_ReturnsFalse()
        {
            string fileName = "image.jpg";
            string contentType = "image/jpeg";

            bool result = IsValidPdfFile(fileName, contentType);

            Assert.False(result);
        }
    }
}
