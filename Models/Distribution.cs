﻿namespace MoviesDBManager.Models
{
    public class Distribution
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int DistributorId { get; set; }
    }
}