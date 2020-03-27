using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using api.Models.Response;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            // TODO - Search the CocktailDB for cocktails with the ingredient given, and return the cocktails
            // https://www.thecocktaildb.com/api/json/v1/1/filter.php?i=Gin
            // You will need to populate the cocktail details from
            // https://www.thecocktaildb.com/api/json/v1/1/lookup.php?i=11007
            // The calculate and fill in the meta object

            cocktailList.Cocktails = new List<Cocktail>();
            string json = await this._client.Get("https://www.thecocktaildb.com/api/json/v1/1/filter.php?i=Gin");
            JToken jToken = JsonConvert.DeserializeObject<JToken>(json);
            var list = jToken["drinks"].ToObject<JArray>();
            List<Task<Cocktail>> filteredList = new List<Task<Cocktail>>();

            foreach (var item in list)
            {
                filteredList.Add(CheckCocktail(item, ingredient));
            }
            var result = await Task.WhenAll<Cocktail>(filteredList);
            cocktailList.Cocktails = result.Where(item => item != null).ToList();
            return Ok(cocktailList);
        }

        private async Task<Cocktail> CheckCocktail(JToken item, string ingredient)
        {
            int id = Convert.ToInt32(item["idDrink"].ToString());
            string strItem = await this._client.Get($"https://www.thecocktaildb.com/api/json/v1/1/lookup.php?i={id}");
            JToken jsonItem = JsonConvert.DeserializeObject<JToken>(strItem);
            var details = jsonItem["drinks"][0];
            List<string> ingredients = new List<string>();

            for (int i = 0; i < 15; i++)
            {
                var value = details[$"strIngredient{i + 1}"].ToString();
                if (string.IsNullOrWhiteSpace(value)) break;
                ingredients.Add(value);
            }

            if (ingredients.Contains(ingredient))
            {
                var cocktail = new Cocktail()
                {
                    Name = item["strDrink"].ToString(),
                    Id = id,
                    ImageURL = item["strDrinkThumb"].ToString(),
                    Ingredients = ingredients
                };
                return cocktail;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("random")]
        public async Task<IActionResult> GetRandom()
        {
            var cocktail = new Cocktail();
            // TODO - Go and get a random cocktail
            // https://www.thecocktaildb.com/api/json/v1/1/random.php
            return Ok(cocktail);
        }
    }
}