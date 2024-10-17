namespace PRISM.DTO
{
    public class InputImportModel
    {
        public string TableName { get; set; }
        public int PageID { get; set; }
        public string PageName { get; set; }
        public string ParentPage { get; set; }
        public List<ColumnDetailImport> ImportColumns { get; set; }
        public string UserId { get; set; }

    }

    public class ColumnDetailImport
    {
        public int Id { get; set; }
        public string FilesColName { get; set; }
        public string DBColName { get; set; }
        public bool IsDeleted { get; set; }
    }

   
}
