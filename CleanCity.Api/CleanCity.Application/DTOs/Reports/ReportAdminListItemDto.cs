namespace CleanCity.Application.DTOs.Reports
{
    public class ReportAdminListItemDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }

        // المنطقة
        public int? AreaId { get; set; }
        public string AreaName { get; set; } = "—";

        // السائق الحالي
        public int? DriverId { get; set; }
        public string DriverName { get; set; } = "—";

        // الحالة
        public string StatusKey { get; set; } = "";  
        public string StatusAr { get; set; } = "";    

        // تحليل AI
        public bool? AiGarbageDetected { get; set; }
        public decimal? AiConfidence { get; set; }

        // التوقيت
        public DateTime CreatedAt { get; set; }
    }
}