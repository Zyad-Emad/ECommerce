using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class ProductQueryParams
    {
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string? SearchValue { get; set; }
        public ProductSortingOptions Sort { get; set; }
        private int _pageSize = DefaultPageSize;
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 10;
        public int PageSize
        {
             get => _pageSize;
            set => _pageSize = (value > MaxPageSize ? MaxPageSize : (value < 1 ? DefaultPageSize : value)); 
        }
        public int PageIndex { get; set; } = 1;
    }
}
