﻿namespace PRN222_Final_Project.ModelDto
{
    public class OrderHistoryViewModel
    {
        public int OrderId { get; set;}
        public DateTime? OrderDate { get; set;}
        public int? OrderStatusID { get; set;}
        public decimal Amount { get; set;}
        public string OrderStatusName { get; set;}
    }
}
