﻿namespace TixTrack.WebApiInterview.Dtos;

public record SalesReport
{
    public int OrderCount { get; set; }
    public double TotalSales { get; set; }
}