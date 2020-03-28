using api.Controllers;
using api.Models.Response;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Xunit;

namespace CocktailUnitTest
{
    public class CocktailUnitTest
    {
        [Fact]
        public async void TestCrocktailList_ShouldFindSix_Async()
        {
            //Arrange
            var httpClient = new HttpClient();
            var client = new CocktailClient(httpClient);
            var controller = new BoozeController(client);

            //Act
            var actionResult = await controller.GetIngredientSearch("Lime Juice");

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            var cocktailList = okResult.Value as CocktailList;
            Assert.Equal(6, cocktailList.meta.count);
            Assert.Equal(5, cocktailList.meta.medianIngredientCount);
            Assert.Equal(11419, cocktailList.meta.firstId);
            Assert.Equal(178315, cocktailList.meta.lastId);
        }

        [Fact]
        public async void TestCrocktailList_ShouldFindNone_Async()
        {
            //Arrange
            var httpClient = new HttpClient();
            var client = new CocktailClient(httpClient);
            var controller = new BoozeController(client);

            //Act
            var actionResult = await controller.GetIngredientSearch("Lime J");

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            var cocktailList = okResult.Value as CocktailList;
            Assert.Equal(0, cocktailList.meta.count);
        }

        [Fact]
        public async void TestRandomCrocktail_ShouldBeOk_Async()
        {
            //Arrange
            var httpClient = new HttpClient();
            var client = new CocktailClient(httpClient);
            var controller = new BoozeController(client);

            //Act
            var actionResult = await controller.GetRandom();

            //Assert
            Assert.IsType<OkObjectResult>(actionResult);
            var okResult = actionResult as OkObjectResult;
            var cocktail = okResult.Value as Cocktail;
            Assert.True(cocktail.Id > 0);
        }
    }
}
