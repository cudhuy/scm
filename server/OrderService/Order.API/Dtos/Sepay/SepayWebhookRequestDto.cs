using System.ComponentModel;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Order.API.Dtos.Sepay
{
    public class SepayWebhookRequestDto
    {
        public int? Id { get; set; }  // Transaction ID on SePay
        public string? Gateway { get; set; }  // Bank brand name
        [JsonPropertyName("transactionDate")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? TransactionDate { get; set; }  // Date and time of the transaction
        public string? AccountNumber { get; set; }  // Bank account number
        public string? Code { get; set; }  // Payment code (automatically detected by SePay)
        public string? Content { get; set; }  // Transaction description
        public string? TransferType { get; set; }  // Transaction type (in: incoming, out: outgoing)
        public decimal? TransferAmount { get; set; }  // Transaction amount
        public decimal? Accumulated { get; set; }  // Account balance after the transaction
        public string? SubAccount { get; set; }  // Sub-account (if applicable)
        public string? ReferenceCode { get; set; }  // Transaction reference code (from SMS)
        public string? Description { get; set; }  // Additional details
    }
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format = "yyyy-MM-dd HH:mm:ss"; // Format của transactionDate

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), _format, CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_format, CultureInfo.InvariantCulture));
        }
    }
}
