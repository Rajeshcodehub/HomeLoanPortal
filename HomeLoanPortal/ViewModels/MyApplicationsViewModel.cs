namespace HomeLoanPortal.ViewModels
{
    public class MyApplicationsViewModel
    {
        public int Id { get; set; }
        public string? ApplicationNumber { get; set; }
        public string Status { get; set; }
        public decimal? LoanAmountRequired { get; set; }
        public DateTime? SubmittedOn { get; set; }
        public DateTime? VerificationAppointmentDate { get; set; }
    }
}
