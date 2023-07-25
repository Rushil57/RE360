namespace RE360.API.Models
{
    public class EstimateViewModel
    {
        public EstimatesDetailViewModel EstimatesDetail { get; set; } = new EstimatesDetailViewModel();
        public List<EstimatesViewModel> Estimates { get; set; } = new List<EstimatesViewModel>();
    }
}
