namespace TFOF.Areas.Core.Models
{
    using System.Collections.Generic;

    public class StatModel {

        public string Title { get; set; }
        public List<StatItemModel> StatItems = new List<StatItemModel>();
        
        public string GetChart() {
            return "";// new Chart(this.StatItems);
        }  
    }  

    public class StatItemModel { 
        public string Label { get; set; }
        public string Value { get; set; }
    }
}