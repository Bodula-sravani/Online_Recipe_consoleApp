using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineRecipes
{
    public class Recipe
    {
        public int id {  get; set; }
        public string name { get; set; }

        public string ingredients { get; set; }

        public string cookingTime { get; set; }

        public string instructions { get; set; }

        public DateTime createddate { get; set; }   

        public string category { get; set; }    

        public string userId { get; set; }



    }
}
