namespace PRISM.DTO.AppUsersModel
{
    public class UserLogModel
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? AppName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedDateString { get; set; }
        public string? ActionType { get; set; }
        public string? RecordStatus { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
		public string? Description { get; set; }
	}
}
