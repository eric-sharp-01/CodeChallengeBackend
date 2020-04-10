using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Response;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api.Controllers
{
    [Route("api")]
    [ApiController]
    public class BoozeController : ControllerBase
    {
        // We will use the public CocktailDB API as our backend
        // https://www.thecocktaildb.com/api.php
        //
        // Bonus points
        // - Speed improvements
        // - Unit Tests

        private readonly ICocktailClient _client;

        public BoozeController(ICocktailClient client)
        {
            this._client = client;
        }

        [HttpGet]
        [Route("search-ingredient/{ingredient}")]
        public async Task<IActionResult> GetIngredientSearch([FromRoute] string ingredient)
        {
            var cocktailList = new CocktailList();
            cocktailList.Cocktails = new List<Cocktail>();
            string json = await this._client.Get($"https://www.thecocktaildb.com/api/json/v1/1/filter.php?i={ingredient}");
            if(!string.IsNullOrWhiteSpace(json))
            {
                JToken jToken = JsonConvert.DeserializeObject<JToken>(json);
                var list = jToken["drinks"].ToObject<JArray>();
                List<Task<Cocktail>> drinkDataList = new List<Task<Cocktail>>();
                foreach (var item in list)
                {
                    drinkDataList.Add(this.GetCocktail(item));
                }
                var result = await Task.WhenAll<Cocktail>(drinkDataList);
                //filter the list and get the ones which have the ingredient
                cocktailList.Cocktails = result
                    .OrderBy(item => item.Ingredients.Count()).ToList();

                if (cocktailList.Cocktails.Any())
                {
                    //calculate the meta
                    int count = cocktailList.Cocktails.Count();

                    int firstId = cocktailList.Cocktails.Min(item => item.Id);

                    var lastId = cocktailList.Cocktails.Max(item => item.Id);

                    int medianIndex = count / 2;
                    int medianNumber = 0;
                    if (count % 2 == 0)
                    {
                        //take round down for the median number
                        medianNumber =
                            (
                                cocktailList.Cocktails[medianIndex].Ingredients.Count()
                                + cocktailList.Cocktails[medianIndex - 1].Ingredients.Count()
                            ) / 2;
                    }
                    else
                    {
                        medianNumber = cocktailList.Cocktails[medianIndex].Ingredients.Count();
                    }

                    cocktailList.meta = new ListMeta
                    {
                        count = count,
                        firstId = firstId,
                        lastId = lastId,
                        medianIngredientCount = medianNumber
                    };
                }
            }
            return Ok(cocktailList);
        }

        [HttpGet]
        [Route("random")]
        public async Task<IActionResult> GetRandom()
        {
            string strItem = await this._client.Get(@"https://www.thecocktaildb.com/api/json/v1/1/random.php");
            var cocktail = this.ConvertCocktailJsonToObject(strItem);
            return Ok(cocktail);
        }

        //get the details using drink id
        private async Task<Cocktail> GetCocktail(JToken item)
        {
            int id = item["idDrink"].Value<int>();
            string strItem = await this._client.Get($"https://www.thecocktaildb.com/api/json/v1/1/lookup.php?i={id}");
            var cocktail = this.ConvertCocktailJsonToObject(strItem);
            return cocktail;
        }

        private Cocktail ConvertCocktailJsonToObject(string strItem)
        {
            JToken jsonItem = JsonConvert.DeserializeObject<JToken>(strItem);
            var details = jsonItem["drinks"][0];
            List<string> ingredients = new List<string>();

            for (int i = 0; i < 15; i++)
            {
                var value = details[$"strIngredient{i + 1}"].ToString();
                if (string.IsNullOrWhiteSpace(value)) break;
                ingredients.Add(value);
            }

            var cocktail = new Cocktail()
            {
                Name = details["strDrink"].Value<string>(),
                Id = details["idDrink"].Value<int>(),
                ImageURL = details["strDrinkThumb"].Value<string>(),
                Instructions = details["strInstructions"].Value<string>(),
                Ingredients = ingredients
            };

            return cocktail;
        }
    }
}