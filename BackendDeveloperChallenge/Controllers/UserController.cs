using BackendDeveloperChallenge.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace BackendDeveloperChallenge.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		// Cosmos DB details
		private readonly string CosmosDbAccountUri = "https://localhost:8081";
		private readonly string CosmosDbAccountPrimaryKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
		private readonly string DbName = "UserProduct";
		private readonly string ContainerName = "User";
		private readonly QueryDefinition GetUsersQuery = new("SELECT * FROM User");

		[HttpPost]
		public async Task<IActionResult> AddUser(UserModel user)
		{
			try
			{
				var client = new CosmosClient(CosmosDbAccountUri, CosmosDbAccountPrimaryKey);
				Database database = await client.CreateDatabaseIfNotExistsAsync(DbName);
				Container container = await database.CreateContainerIfNotExistsAsync(ContainerName, "/username");
				var response = await container.CreateItemAsync<UserModel>(user);
				return Ok(response.Resource);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetUsers()
		{
			try
			{
				var users = new List<UserModel>();
				var client = new CosmosClient(CosmosDbAccountUri, CosmosDbAccountPrimaryKey);
				Database database = await client.CreateDatabaseIfNotExistsAsync(DbName);
				Container container = await database.CreateContainerIfNotExistsAsync(ContainerName, "/username");

				using FeedIterator<UserModel> feed = container.GetItemQueryIterator<UserModel>(GetUsersQuery);
				while (feed.HasMoreResults)
				{
					FeedResponse<UserModel> response = await feed.ReadNextAsync();
					foreach (UserModel user in response)
					{
						users.Add(user);
					}
				}
				
				return Ok(users);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
