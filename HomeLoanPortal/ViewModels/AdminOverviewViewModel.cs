namespace HomeLoanPortal.ViewModels
{
    public class AdminOverviewViewModel
    {
        public int Total { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Verified { get; set; }
        public int Pending { get; set; }
        public int Draft { get; set; }
    }
}
