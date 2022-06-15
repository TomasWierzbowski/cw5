using System;
using System.Collections.Generic;

namespace cwiczenia_5.Models.DTO
{
    public class SomeSortOfTrip
    {
        public int IdTrip { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int MaxPeople { get; set; }
        public IEnumerable<SomeSortOfCountry> Countries { get; set; }
        public IEnumerable<SomeSortOfClient> Clients { get; set; }
    }
}
