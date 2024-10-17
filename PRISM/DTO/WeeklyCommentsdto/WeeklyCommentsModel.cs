using PRISM.Models;

namespace PRISM.DTO.WeeklyCommentsdto
{
    public class WeeklyCommentsModel
    {
        public long Id { get; set; }
        public long? WeekId { get; set; }
        public string? EngineeringSupport { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UserId { get; set; }
        public string? CoursesAndOthers { get; set; }
        public List<WeekArrangement> ListArrangements { get; set; }
    }
}
