﻿using System.ComponentModel.DataAnnotations;

namespace RE360.API.Models
{
    public class MethodOfSaleViewModel
    {
        public int ID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PID { get; set; }
        public int AgencyTypeID { get; set; }
        public string? AgencyOtherTypeRemark { get; set; }
        public int MethodOfSaleID { get; set; }
        public decimal? Price { get; set; }
        public string? PriceRemark { get; set; }
        public DateTime? AuctionDate { get; set; }
        public string? AuctionTime { get; set; }
        public string? AuctionVenue { get; set; }
        public string? Auctioneer { get; set; }
        public DateTime? TenderDate { get; set; }
        public string? TenderTime { get; set; }
        public DateTime? DeadLineDate { get; set; }
        public string? DeadLineTime { get; set; }
        public bool IsMortgageeSale { get; set; }
        public bool IsAsIs { get; set; }
        public bool IsAuctionUnlessSoldPrior { get; set; }
        public bool IsTenderUnlessSoldPrior { get; set; }
        public bool IsAuctionOnSite { get; set; }
        public string? TenderVenue { get;set; }
    }
}
