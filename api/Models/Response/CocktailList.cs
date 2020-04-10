using System.Collections.Generic;

namespace api.Models.Response
{
    public class CocktailList
    {
        public List<Cocktail> Cocktails { get; set; }
        public ListMeta meta { get; set; }

        public CocktailList()
        {
            this.meta = new ListMeta
            {
                count = 0,
                firstId = 0,
                lastId = 0,
                medianIngredientCount = 0
            };
        }
    }

    public class ListMeta
    {
        public int count { get; set; }    // number of results
        public int firstId { get; set; }    // smallest Id of the results
        public int lastId { get; set; }    // largest Id of the results
        public int medianIngredientCount { get; set; }    // median of the number of ingredients per cocktail
    }
}