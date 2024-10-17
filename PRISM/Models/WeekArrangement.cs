using System;
using System.Collections.Generic;

namespace PRISM.Models
{
    public partial class WeekArrangement
    {
        public long Id { get; set; }
        public long? WeekId { get; set; }
        public string? UserId { get; set; }
        public string? ColumnNo1 { get; set; }
        public string? ColumnNo2 { get; set; }
        public string? ColumnNo3 { get; set; }
        public string? ColumnNo4 { get; set; }
        public string? ColumnNo5 { get; set; }
        public string? ColumnNo6 { get; set; }
        public string? ColumnNo7 { get; set; }
        public string? ColumnNo8 { get; set; }
    }
}
