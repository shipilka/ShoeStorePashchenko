using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoeStoreLibrary.Models
{
    //модель фильтров
    public class ProductFilter
    {
        public string SearchTerm { get; set; }
        public string SelectedManufacturer { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool ShowDiscounted { get; set; }
        public bool InStock { get; set; }
        public string SortBy { get; set; }
    }
}
